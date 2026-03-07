using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DangerWarningUI : MonoBehaviour
{
    [Header("Warning Settings")]
    public float pulseSpeed = 5f; // 깜빡이는 속도
    public float maxAlpha = 0.6f; // 최대 투명도
    public float minAlpha = 0.1f; // 최소 투명도
    public Color warningColor = new Color(0.8f, 0f, 0f); // 기본 검붉은색

    private Image warningImage;

    void Start()
    {
        // 1. 내 오브젝트에 붙어있는 Image 컴포넌트를 가져옴
        warningImage = GetComponent<Image>();

        // 2. 외부 이미지 파일 대신, 코드로 직접 그린 스프라이트를 집어넣음
        warningImage.sprite = CreateRadialGradientSprite();
        
        // 3. 색상 적용
        warningImage.color = warningColor;
    }

    void Update()
    {
        // 수학(Sin 함수)을 이용해 부드럽게 점멸하는 심장 박동 효과
        float wave = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f; 
        float currentAlpha = Mathf.Lerp(minAlpha, maxAlpha, wave);

        Color c = warningImage.color;
        c.a = currentAlpha;
        warningImage.color = c;
    }

    // [핵심 기술] 코드로 투명한 비네트 이미지를 창조하는 함수
    private Sprite CreateRadialGradientSprite()
    {
        int size = 256; // 해상도 (256x256)
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float radius = size / 2f;

        // 픽셀 하나하나를 돌면서 가운데는 투명하게, 테두리는 불투명하게 칠함
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center);
                
                // 중심부에서 40%까지는 완전 투명(0), 그 이후부터 테두리(100%)까지 서서히 진해짐
                float alpha = Mathf.Clamp01((dist - (radius * 0.4f)) / (radius * 0.6f)); 
                
                texture.SetPixel(x, y, new Color(1, 1, 1, alpha));
            }
        }
        texture.Apply(); // 텍스처 적용

        // 그려진 텍스처를 UI용 Sprite로 변환해서 반환
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }
}