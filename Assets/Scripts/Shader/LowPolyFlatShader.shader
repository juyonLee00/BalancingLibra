Shader "Custom/URP_LowPolyFlatShader"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (1, 0.8, 0.2, 1) // 기본 황금색 세팅
    }
    
    SubShader
    {
        // URP 파이프라인을 사용함을 명시
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" "Queue"="Geometry" }
        LOD 100

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            // 스탠다드의 CGPROGRAM 대신 HLSLPROGRAM 사용
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // URP 코어 및 라이팅 라이브러리 포함
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION; // 오브젝트 공간 정점
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION; // 클립 공간 정점 (화면 출력용)
                float3 positionWS : TEXCOORD0;   // 월드 공간 정점 (미분 연산용)
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
            CBUFFER_END

            // 1. 정점 셰이더 (Vertex Shader)
            Varyings vert(Attributes input)
            {
                Varyings output;
                // 오브젝트 좌표를 월드 좌표로 변환
                output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                // 월드 좌표를 화면(클립) 좌표로 변환
                output.positionCS = TransformWorldToHClip(output.positionWS);
                return output;
            }

            // 2. 픽셀(프래그먼트) 셰이더 (Fragment Shader)
            half4 frag(Varyings input) : SV_Target
            {
                // 🚨 [최적화 핵심] 픽셀 단위로 월드 좌표의 변화량을 추적해 직각 노말(Flat Normal) 생성
                float3 dpdx = ddx(input.positionWS);
                float3 dpdy = ddy(input.positionWS);
                float3 normalWS = normalize(cross(dpdy, dpdx));

                // URP 메인 조명(Directional Light) 가져오기
                Light mainLight = GetMainLight();
                
                // 빛의 방향과 노말의 각도를 계산 (Lambert 연산)
                half NdotL = saturate(dot(normalWS, mainLight.direction));
                half3 diffuse = mainLight.color * NdotL;

                // 환경광(Ambient) 가져오기 (어두운 부분도 자연스럽게 보이도록)
                half3 ambient = SampleSH(normalWS);

                // 최종 색상 조합
                half3 finalColor = _BaseColor.rgb * (diffuse + ambient);

                return half4(finalColor, _BaseColor.a);
            }
            ENDHLSL
        }
    }
}