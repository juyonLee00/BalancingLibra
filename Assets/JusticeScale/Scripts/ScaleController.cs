using JusticeScale.Scripts.Scales;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace JusticeScale.Scripts
{
    public class ScaleController : MonoBehaviour
    {
        [Header("Scales")]
        [SerializeField]
        [Tooltip("Reference to the left scale. Should be assigned to the left side of the balance.")]
        private Scale leftScale;

        [SerializeField] 
        [Tooltip("Reference to the right scale. Should be assigned to the right side of the balance.")]
        private Scale rightScale;

        [Tooltip("Normalized value representing the current balance. 0 means fully tilted to the left, 1 means fully tilted to the right, and 0.5 is perfectly balanced.")]
        public float BalanceNormalized { get; private set; } = 0.5f;

        [Tooltip("The difference in weight between the left and right scales. A positive value means the left scale is heavier, while a negative value means the right scale is heavier.")]
        public float WeightDifference { get; private set; }

        [Header("Configuration")]
        [Min(0.01f)]
        [Tooltip("The maximum weight difference between the two sides for the scale to reach its limit. A higher value makes the balance less sensitive.")]
        public float maxWeightDifference = 10f;

        [SerializeField]
        [Tooltip("The smoothing time for updating the balance. This helps to avoid abrupt changes that could cause erratic behavior or glitches in the balance's operation.")]
        private float balanceSmoothTime = 0.05f;

        [SerializeField] [Tooltip("Internal smoothed result for the balance. Used for gradual balance adjustments.")]
        private float weightResultSmoothed;

        [Header("Game Over Rules")]
        [Tooltip("The threshold for the weight difference to trigger a game over.")]
        [Range(0f, 0.5f)] public float tiltThreshold = 0.3f;
        public float maxTiltDuration = 10f;

        [Header("Warning UI Effect")]
        [SerializeField] private GameObject warningUI;
        [SerializeField] private float warningStartTime = 5f;
        [SerializeField] private float minBlinkSpeed = 5f;
        [SerializeField] private float maxBlinkSpeed = 20f;

        private float _currentTiltTimer = 0f;
        private Image _warningImage;

        private void Start()
        {
            if(warningUI != null)
            {
                _warningImage = warningUI.GetComponentInChildren<Image>();

                warningUI.SetActive(false);
            }
        }

        private void Update()
        {
            if (GameManager.Instance.isGameOver) return;

            if (!leftScale || !rightScale)
            {
                Debug.LogWarning("The scale references are not assigned.");
                return;
            }
            
            UpdateBalance();
            CheckTiltCondition();
        }

        private void UpdateBalance()
        {
            var targetWeightDifference = leftScale.TotalWeight - rightScale.TotalWeight;
            WeightDifference = targetWeightDifference;
            weightResultSmoothed = Mathf.Lerp(weightResultSmoothed, targetWeightDifference, balanceSmoothTime);

            BalanceNormalized = Mathf.InverseLerp(-maxWeightDifference, maxWeightDifference, weightResultSmoothed);
        }

        private void CheckTiltCondition()
        {
            float deviation = Mathf.Abs(BalanceNormalized - 0.5f);
            if (deviation > tiltThreshold)
            {
                _currentTiltTimer += Time.deltaTime;

                if(_currentTiltTimer >= warningStartTime && warningUI != null)
                {
                    if(!warningUI.activeSelf)
                    {
                        warningUI.SetActive(true);
                    }

                    if(_warningImage != null)
                    {
                        float progress = (_currentTiltTimer - warningStartTime) / (maxTiltDuration - warningStartTime);
                        progress = Mathf.Clamp01(progress);

                        float currentBlinkSpeed = Mathf.Lerp(minBlinkSpeed, maxBlinkSpeed, progress);
                        float alpha = (Mathf.Sin(Time.time * currentBlinkSpeed) + 1f) / 2f * 0.5f;

                        _warningImage.color = new Color(0.85f, 0f, 0f, alpha);
                    }
                }
                if (_currentTiltTimer >= maxTiltDuration)
                {
                    warningUI.SetActive(false);
                    GameManager.Instance.GameOver("Scale tilted too far");
                }
            }
            else
            {
                _currentTiltTimer = 0f;

                if(warningUI != null && warningUI.activeSelf)
                {
                    warningUI.SetActive(false);
                }
            }
        }
    }
}