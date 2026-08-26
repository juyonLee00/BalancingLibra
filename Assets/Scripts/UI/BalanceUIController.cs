using UnityEngine;
using UnityEngine.UI;
using JusticeScale.Scripts;

public class BalanceUIController : MonoBehaviour
{
    [Header("Data & Logic References")]
    [Tooltip("씬 내부 저울 연결")]
    public ScaleController scaleController;
    [Tooltip("기획 데이터 SO 연결")]
    public ScoreTierSettings tierSettings;

    [Header("UI References")]
    [Tooltip("바의 배경 RectTransform")]
    public RectTransform barBackgroundRect; 
    [Tooltip("포인터 UI")]
    public RectTransform pointerRect;       
    [Tooltip("포인터의 이미지 컴포넌트")]
    public Image pointerImage;              

    [Header("Juice Settings")]
    [Tooltip("UI 바늘이 움직이는 속도 (높을수록 빠릿함)")]
    public float pointerMoveSpeed = 15f;
    [Tooltip("색상 전환 속도")]
    public float colorChangeSpeed = 10f;

    void Update()
    {
        if (scaleController == null || tierSettings == null || pointerRect == null || barBackgroundRect == null) return;
        if (GameManager.Instance != null && GameManager.Instance.isGameOver) return;

        // 방향을 포함한 저울의 실제 기울기 (음수 = 왼쪽, 양수 = 오른쪽)
        float rawZ = scaleController.transform.eulerAngles.z;
        float signedAngle = rawZ > 180f ? rawZ - 360f : rawZ;

        // SO 데이터에서 현재 각도에 맞는 배율과 색상을 가져옴
        (_, Color targetColor) = tierSettings.GetTierInfo(Mathf.Abs(signedAngle));

        // 포인터 색상 부드럽게 전환
        if (pointerImage != null)
        {
            pointerImage.color = Color.Lerp(pointerImage.color, targetColor, Time.deltaTime * colorChangeSpeed);
        }

        // 게임오버 각도 기준으로 현재 기울기를 -1.0 ~ 1.0 비율로 변환
        float tiltRatio = Mathf.Clamp(signedAngle / tierSettings.gameOverAngle, -1f, 1f);

        // 배경 바의 실제 너비를 기준으로 이동할 X 좌표 계산
        // 바늘이 바의 절반 길이(width / 2) 안에서 좌우로 움직이도록 계산
        float maxUIWidth = barBackgroundRect.rect.width / 2f;
        float targetX = tiltRatio * -maxUIWidth; 

        // 포인터를 목표 위치로 부드럽게 이동
        Vector2 currentPos = pointerRect.anchoredPosition;
        pointerRect.anchoredPosition = Vector2.Lerp(currentPos, new Vector2(targetX, currentPos.y), Time.deltaTime * pointerMoveSpeed);
    }
}