using UnityEngine;
using TMPro; 

public class TitleController : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Click to Start 텍스트 객체를 연결하세요.")]
    [SerializeField] private TextMeshProUGUI clickToStartText;
    
    [Tooltip("Balancing In Space 타이틀 텍스트(또는 상위 부모 객체)를 연결하세요.")]
    [SerializeField] private Transform titleTextTransform; 

    [Header("Blink Settings")]
    [SerializeField] private float blinkSpeed = 3f;
    [SerializeField] private float minAlpha = 0.2f; 

    [Header("Title Floating Settings")]
    [SerializeField] private float floatSpeed = 2f;
    [SerializeField] private float floatAmount = 15f; // 위아래로 움직일 픽셀(거리) 범위
    [SerializeField] private float tiltAngle = 5f; // 좌우로 흔들릴 최대 각도

    private Vector3 _originalTitlePos;
    private bool _isTransitioning = false;

    private void Start()
    {
        if (titleTextTransform != null)
        {
            _originalTitlePos = titleTextTransform.localPosition;
        }
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBGM(BGMType.Title);
        }
    }

    private void Update()
    {
        if (_isTransitioning) return;

        AnimateClickToStartText();
        AnimateTitleText();
        CheckInput();
    }

    private void AnimateClickToStartText()
    {
        if (clickToStartText == null) return;

        float sinValue = (Mathf.Sin(Time.time * blinkSpeed) + 1f) / 2f; 
        
        float alpha = Mathf.Lerp(minAlpha, 1f, sinValue);
        
        clickToStartText.color = new Color(
            clickToStartText.color.r, 
            clickToStartText.color.g, 
            clickToStartText.color.b, 
            alpha
        );
    }

    private void AnimateTitleText()
    {
        if (titleTextTransform == null) return;

        // 무중력 부유 효과
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatAmount;
        titleTextTransform.localPosition = _originalTitlePos + new Vector3(0f, yOffset, 0f);

        // 저울 기울임 효과 
        float zRot = Mathf.Sin(Time.time * (floatSpeed * 0.7f)) * tiltAngle;
        titleTextTransform.localRotation = Quaternion.Euler(0f, 0f, zRot);
    }

    private void CheckInput()
    {
        if (Input.GetMouseButtonDown(0) || Input.anyKeyDown)
        {
            StartGame();
        }
    }

    private void StartGame()
    {
        _isTransitioning = true;
        
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(SFXType.Click);
        }

        if (SceneLoader.Instance != null)
        {
            AudioManager.Instance.StopBGM();
            SceneLoader.Instance.LoadScene(SceneType.Loading);
        }
        else
        {
            Debug.LogError("[TitleController] Can't find SceneLoader Instance.");
        }
    }
}