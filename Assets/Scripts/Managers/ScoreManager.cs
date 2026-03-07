using UnityEngine;
using TMPro;

public class ScoreManager : SingletonBehaviour<ScoreManager>
{
    public int CurrentScore { get; private set; } = 0;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;

    private void Start()
    {
        UpdateScoreUI();
    }

    public void AddScore(int level, float mass, float scale, bool isFeverTime)
    {
        float volume = Mathf.Pow(Mathf.Max(scale, 0.01f), 3);
        float expansion = volume / Mathf.Max(mass, 0.1f);
        float rawScore = Mathf.Pow(2, level) * expansion * GameManager.Instance.baseScoreMultiplier;

        int pointsToAdd = Mathf.Max(1, Mathf.RoundToInt(rawScore));

        if(isFeverTime) pointsToAdd *= 2;

        CurrentScore += pointsToAdd;
        UpdateScoreUI();
    }

    public void AddBonusScore(int bonus)
    {
        CurrentScore += bonus;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if(scoreText != null) scoreText.text = CurrentScore.ToString();
    }
}
