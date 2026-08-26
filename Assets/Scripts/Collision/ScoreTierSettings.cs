using UnityEngine;

[System.Serializable]
public struct ScoreZone
{
    public string zoneName;
    [Range(0, 90)] public float maxAngle;
    public float multiplier;
    public Color zoneColor;
}

[CreateAssetMenu(fileName = "ScoreTierSettings", menuName = "BalancingLibra/ScoreTierSettings")]
public class ScoreTierSettings : ScriptableObject 
{
    [Header("저울 상태별 점수 및 UI 설정 (중앙부터 순서대로 작성하시오)")]
    public ScoreZone[] scoreZones;

    [Header("게임 오버 각도(빨간색 구역 시작)")]
    public float gameOverAngle = 45f;

    [Header("예외 상황 기본값 설정")]
    [Tooltip("기획 데이터 세팅 오류 시 반환될 기본 점수 배율")]
    public float fallbackMultiplier = 1.0f;
    [Tooltip("기획 데이터 세팅 오류시 UI에 표시될 기본 색상")]
    public Color fallbackColor = Color.magenta;

    public (float multiplier, Color color) GetTierInfo(float currentAngle)
    {
        currentAngle = Mathf.Abs(currentAngle);

        foreach (var zone in scoreZones)
        {
            if (currentAngle <= zone.maxAngle)
                return (zone.multiplier, zone.zoneColor);
        }

        Logger.LogWarning($"[ScoreTierSettings] Insufficient zone data to precess angle {currentAngle}");
        return (fallbackMultiplier, fallbackColor);
    }    
}
