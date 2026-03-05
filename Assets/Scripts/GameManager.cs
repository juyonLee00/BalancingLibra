using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class BallData
{
    public string name;
    public GameObject prefab;
}

public class GameManager : SingletonBehaviour<GameManager>
{

    [Header("End Game Settings")]
    // 블랙홀 소멸 보너스 점수
    public int blackHoleBonusScore = 5000; 
    public GameObject blackHoleEffectPrefab; /

    // 큰 별을 만들 경우에 대한 점수 배율
    [Header("Score Balance Settings")]
    public float baseScoreMultiplier = 8000f; 

    [Header("Fever System")]
    [SerializeField] private Image feverFillImage; 
    public float currentFever = 0f;
    public bool isFeverTime = false;

    // 균형 유지하는 시간 체크
    private float balanceTimer = 0f;
    
    // 수평 유지로 피버 게이지 10% 상승했는지 체크
    private bool hasRewardedForThisBalance = false;

    [Header("Game Settings")]
    [SerializeField] private List<BallData> ballList;

    [Header("Preview Settings")]
    [SerializeField] private Transform previewPos;
    private GameObject _currentPreviewObject;

    [Header("UI Settings")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private GameObject gameOverPanel;

    [SerializeField] private GameObject scorePanel;
    [SerializeField] private GameObject nextPanel;

    [SerializeField] private GameObject helpPanel;

    private bool isPaused = false;

    private static bool hasSeenTutorial = false;

    public bool isGameOver = false;
    public int currentScore = 0;
    
    private int _nextBallIndex;

    private Camera mainCamera;
    private float timeCount = 0f;
    private float spawnInterval = 0.5f;

    [Header("Life System")]
    public int maxLife = 3;        
    public int currentLife;       
    public Image[] heartUIArray;   
    public Sprite fullHeartSprite;   
    public Sprite emptyHeartSprite;  
    private Coroutine fadeCoroutine;

    public GameObject cheatLevel6Prefab;
    
    protected override void Awake()
    {
        base.Awake();
        Time.timeScale = 1f;
    }
    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Logger.LogError("Main Camera not found!", this);
        }

        currentLife = maxLife;
        UpdateHeartUI();

        if(gameOverPanel != null) gameOverPanel.SetActive(false);

        SetNextBall();
        UpdateScoreUI();

        if(helpPanel != null)
        {
            if (!hasSeenTutorial)
            {
                fadeCoroutine = StartCoroutine(InitialTutorialSequence());
            }
            else
            {
                helpPanel.SetActive(false);
            }
        }

        foreach (BallData data in ballList)
        {
            if(data.prefab != null)
            {
                Ball ballScript = data.prefab.GetComponent<Ball>();
                if(ballScript != null)
                {
                    int poolSize = (ballScript.ballLevel <= 2) ? 30 : 10;
                    PoolType type = (PoolType)(ballScript.ballLevel - 1);
                    PoolManager.Instance.CreatePool(type, data.prefab, poolSize);
                }
            }
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            HandleGameExit();
        }

        if(Input.GetKeyDown(KeyCode.F1) && !isGameOver)
        {
            ToggleHelpMenu();
        }

        if (isGameOver || isPaused) return;

        if(_currentPreviewObject != null)
        {
            _currentPreviewObject.transform.Rotate(Vector3.up * 50f * Time.deltaTime);
        }

        //클릭시 오브젝트 생성
        timeCount += Time.deltaTime;
        if (Input.GetMouseButtonDown(0) && timeCount >= spawnInterval)
        {
            SpawnBall();
        }
    }

    void SpawnBall()
    {
        if(ballList == null || ballList.Count == 0)
            return;
        
        timeCount = 0f;
        if(mainCamera == null) return;

        Vector3 mousePosition = Input.mousePosition;
        
        // 카메라의 위치값이 아닌, 카메라에서 Z=0 평면까지의 거리(절댓값)를 입력해야 함
        mousePosition.z = Mathf.Abs(mainCamera.transform.position.z); 
        
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
        
        worldPosition.z = 0f;

        GameObject selectedBallPrefab = ballList[_nextBallIndex].prefab;

        if(selectedBallPrefab != null)
        {
            Ball ballPrefabScript = selectedBallPrefab.GetComponent<Ball>();
            PoolType type = (PoolType)(ballPrefabScript.ballLevel - 1); 
            PoolManager.Instance.Spawn<Ball>(type, worldPosition, Quaternion.identity);
        }
        SetNextBall();
    }

    void SetNextBall()
    {
        int maxSpawnIndex = Mathf.Min(ballList.Count, 3);
        _nextBallIndex = Random.Range(0, maxSpawnIndex);

        UpdatePreviewObject();
    }

    void UpdatePreviewObject()
    {
        if(_currentPreviewObject != null)
        {
            _currentPreviewObject.SetActive(false);
            Destroy(_currentPreviewObject);
        }

        Logger.Log("UpdatePreviewObject value: " + _nextBallIndex, this);

        GameObject prefab = ballList[_nextBallIndex].prefab;
        if(prefab != null && previewPos != null)
        {
            _currentPreviewObject = Instantiate(prefab, previewPos.position, Quaternion.identity);

            _currentPreviewObject.transform.localScale = Vector3.one;
            float previewScaleMultiplier = 0.08f; 
            _currentPreviewObject.transform.localScale = _currentPreviewObject.transform.localScale * previewScaleMultiplier;

            int previewLayerIndex = LayerMask.NameToLayer("PreviewLayer");
            SetLayerRecursively(_currentPreviewObject, previewLayerIndex);

            Rigidbody[] rbs = _currentPreviewObject.GetComponentsInChildren<Rigidbody>(true);
            foreach(Rigidbody rb in rbs)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
                Destroy(rb);
            }

            Collider[] cols = _currentPreviewObject.GetComponentsInChildren<Collider>(true);
            foreach(Collider col in cols)
            {
                col.enabled = false;
                Destroy(col);
            }

            Ball[] ballScripts = _currentPreviewObject.GetComponentsInChildren<Ball>(true);
            foreach(Ball script in ballScripts)
            {
                script.enabled = false;
                Destroy(script);
            }
        }
    }

    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if(newLayer == -1) return;

        obj.layer = newLayer;
        foreach(Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    public void GameOver(string reason)
    {

        if (isGameOver)
        {
            return;
        }

        isGameOver = true;
        Logger.Log(reason, this);

        if(gameOverPanel != null)
        {
            nextPanel.SetActive(false);
            scorePanel.SetActive(false);
            gameOverPanel.SetActive(true);
        }

        if(finalScoreText != null)
        {
            finalScoreText.text = $"Score: {currentScore}";
        }
        Time.timeScale = 0f;
    }
    
    public void RestartGame()
    {
        if(Instance == this)
        {
            Destroy(this.gameObject);
        }

        SceneLoader.Instance.ReloadScene();
        
    }

    public void AddScore(int level, float mass, float scale)
    {
        if(isGameOver)
            return;

        // 부피 계산
        float volume = Mathf.Pow(Mathf.Max(scale, 0.01f), 3);

        // 팽창도 산출: 부피 / 질량
        // 최소값 0.1f 보정
        float expansion = volume / Mathf.Max(mass, 0.1f);
        
        float rawScore = Mathf.Pow(2, level) * expansion * baseScoreMultiplier;

        // 최종 점수: 레벨 가중치 * 팽창도 * 기본배율
        int pointsToAdd = Mathf.Max(1, Mathf.RoundToInt(rawScore));

        // 피버타임 중이라면 획득 점수 2배 
        if (isFeverTime)
        {
            pointsToAdd *= 2;
            Logger.Log($"FEVER BONUS! +{pointsToAdd} Points (Expansion: {expansion:F1})", this);
        }

        currentScore += pointsToAdd;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if(scoreText != null)
        {
            scoreText.text = currentScore.ToString();
        }
    }

    // 저울의 수평 상태를 매 프레임 전달받아 타이머 시작
    public void UpdateBalanceState(bool isBalanced, bool isEmpty)
    {
        // 게임 오버, 일시정지, 혹은 이미 피버타임 중이라면 충전 중지
        if (isGameOver || isPaused || isFeverTime) return;

        if (isEmpty) 
        {
            balanceTimer = 0f;
            hasRewardedForThisBalance = false;
            return;
        }

        if (isBalanced)
        {
            // 수평 상태이면서 아직 보상을 안 받았을 때만 타이머 증가
            if (!hasRewardedForThisBalance) 
            {
                balanceTimer += Time.deltaTime;
                
                if (balanceTimer >= 5.0f)
                {
                    // 게이지 증가
                    AddFever(10f); 
                    
                    // 보상 지급 완료. 
                    hasRewardedForThisBalance = true; 
                    Logger.Log("Fever +10% 획득. 다음 충전을 위해선 수평을 한 번 깨야 함.", this);
                }
            }
        }
        else
        {
            // 공을 새로 떨어뜨려 수평이 깨질 경우 
            balanceTimer = 0f; 
            hasRewardedForThisBalance = false; 
        }
    }

    private void AddFever(float amount)
    {
        currentFever += amount;
        
        if (currentFever >= 100f)
        {
            currentFever = 100f;
            StartCoroutine(FeverTimeRoutine());
        }
        
        UpdateFeverUI();
    }

    private void UpdateFeverUI()
    {
        if (feverFillImage != null)
        {
            feverFillImage.fillAmount = currentFever / 100f; 
        }
    }

    // 최고 단계 별이 등록될 때마다 양쪽 저울 상태 체크
    public void CheckBlackHoleCondition()
    {
        // 씬에 존재하는 모든 공 가져옴
        Ball[] allBalls = FindObjectsOfType<Ball>();
        
        Ball leftSideStar = null;
        Ball rightSideStar = null;

        foreach (Ball b in allBalls)
        {
            // 해당 공이 최종 티어이고, 저울에 무사히 안착한 경우
            if (b.ballLevel == 5 && b.CurrentScale != null)
            {
                if (leftSideStar == null)
                {
                    leftSideStar = b; // 일단 하나 발견하면 임시 보관
                }
                // 두 번째 최종 티어 별이 첫 번째 별과 소속된 저울이 다를 경우
                else if (leftSideStar.CurrentScale != b.CurrentScale)
                {
                    rightSideStar = b;
                    break; 
                }
            }
        }

        // 양쪽 저울에서 최종 티어 별을 모두 찾았다면 블랙홀 생성
        if (leftSideStar != null && rightSideStar != null)
        {
            TriggerBlackHole(leftSideStar, rightSideStar);
        }
    }

    private void TriggerBlackHole(Ball star1, Ball star2)
    {
        Logger.Log("DUAL BLACK HOLE TRIGGERED!", this);

        currentScore += blackHoleBonusScore;
        UpdateScoreUI();

        StartCoroutine(AnimateBlackHoleAbsorption(star1, star2));

        // 블랙홀 발동과 동시에 100% 피버타임
        if (!isFeverTime)
        {
            currentFever = 100f; 
            UpdateFeverUI();
            StartCoroutine(FeverTimeRoutine()); 
            Logger.Log("FEVER TIME FORCED BY BLACK HOLE!", this);
        }

        if (blackHoleEffectPrefab != null)
        {
            GameObject effect = Instantiate(blackHoleEffectPrefab, new Vector3(0f, 6.21f, 0f), Quaternion.Euler(330f, 0f, 0f));
            effect.SetActive(true);
            
            Destroy(effect, 7f); 
        }
        if (star1 != null) 
            PoolManager.Instance.ReturnObject((PoolType)(star1.ballLevel), star1.gameObject);
        
        if (star2 != null) 
            PoolManager.Instance.ReturnObject((PoolType)(star2.ballLevel), star2.gameObject);
    }

    private System.Collections.IEnumerator AnimateBlackHoleAbsorption(Ball star1, Ball star2)
    {
        float duration = 7f; 
        float elapsed = 0f;

        if (CameraShake.Instance != null)
        {
            StartCoroutine(CameraShake.Instance.ShakeCoroutine(duration, 0.2f));
        }

        // 애니메이션 시작 전 두 공의 Rigidbody를 kinematic으로 바꿔서 물리 연산을 끈다.
        // 흡수되는 동안 저울이 더 흔들리거나 다른 공과 충돌하는 현상 방지.
        if (star1 != null && star1.GetComponent<Rigidbody>() != null) star1.GetComponent<Rigidbody>().isKinematic = true;
        if (star2 != null && star2.GetComponent<Rigidbody>() != null) star2.GetComponent<Rigidbody>().isKinematic = true;

        Vector3 originalScale1 = star1.transform.localScale;
        Vector3 originalScale2 = star2.transform.localScale;


        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            if (star1 != null) star1.transform.localScale = Vector3.Lerp(originalScale1, Vector3.zero, t);
            if (star2 != null) star2.transform.localScale = Vector3.Lerp(originalScale2, Vector3.zero, t);
            
            yield return null;
        }

        if (star1 != null) 
            PoolManager.Instance.ReturnObject((PoolType)(star1.ballLevel), star1.gameObject);
        
        if (star2 != null) 
            PoolManager.Instance.ReturnObject((PoolType)(star2.ballLevel), star2.gameObject);
    }

    // 피버타임 코루틴
    private System.Collections.IEnumerator FeverTimeRoutine()
    {
        isFeverTime = true;
        float duration = 7f;
        float elapsed = 0f;

        Logger.Log("FEVER TIME START! (SCORE x2)", this);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            currentFever = Mathf.Lerp(100f, 0f, elapsed / duration);
            UpdateFeverUI();
            
            yield return null;
        }

        currentFever = 0f;
        isFeverTime = false;
        balanceTimer = 0f; 
        UpdateFeverUI();
        Logger.Log("FEVER TIME END", this);
    }

    private void HandleGameExit()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Logger.Log("Game Exit Triggered", this);

            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }

    //목숨 차감 및 게임 오버 판별
    public void LoseLife(string reason)
    {
        if (isGameOver) return;

        currentLife--;
        Logger.Log($"Life Lost: {reason} | Remaining: {currentLife}", this);
        
        UpdateHeartUI();

        if (currentLife <= 0)
        {
            GameOver("하트 소진: " + reason);
        }
    }

    //Life UI 스프라이트 교체 
    private void UpdateHeartUI()
    {
        for (int i = 0; i < heartUIArray.Length; i++)
        {
            if (i < currentLife)
            {
                heartUIArray[i].enabled = true;
            }
            else
            {
                heartUIArray[i].enabled = false;
            }
        }
    }

    // 도움말 창 켜기/끄기 및 시간 제어
    public void ToggleHelpMenu()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f; 
            
            if (helpPanel != null)
            {
                helpPanel.SetActive(true); 
                
                CanvasGroup cg = helpPanel.GetComponent<CanvasGroup>();
                if(cg != null) cg.alpha = 1f; 
                
                if(fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            }
            Logger.Log("Game Paused: Help Menu Opened", this);
        }
        else
        {
            Time.timeScale = 1f; 
            
            if (helpPanel != null && helpPanel.activeSelf)
            {
                fadeCoroutine = StartCoroutine(FadeOutHelpPanel());
            }
            Logger.Log("Game Resumed: Help Menu Closed", this);
        }
    }

    private System.Collections.IEnumerator FadeOutHelpPanel()
    {
        CanvasGroup cg = helpPanel.GetComponent<CanvasGroup>();
        
        if (cg == null) 
        {
            cg = helpPanel.AddComponent<CanvasGroup>();
        }

        float fadeDuration = 0.5f; 
        float currentTime = 0f;

        while (currentTime < fadeDuration)
        {
            // 일시정지를 풀었을 때의 미세한 오차 방지
            currentTime += Time.unscaledDeltaTime; 
            
            cg.alpha = Mathf.Lerp(1f, 0f, currentTime / fadeDuration);
            
            yield return null; 
        }

        helpPanel.SetActive(false);
        cg.alpha = 1f; 
    }

    // 처음 시작할 때 조작법 보여주는 코루틴
    private System.Collections.IEnumerator InitialTutorialSequence()
    {
        hasSeenTutorial = true;

        helpPanel.SetActive(true);
        CanvasGroup cg = helpPanel.GetComponent<CanvasGroup>();
        if(cg == null) cg = helpPanel.AddComponent<CanvasGroup>();
        cg.alpha = 1f;

        yield return new WaitForSecondsRealtime(7f);

        float fadeDuration = 2f; 
        float currentTime = 0f;

        while (currentTime < fadeDuration)
        {
            currentTime += Time.unscaledDeltaTime; 
            cg.alpha = Mathf.Lerp(1f, 0f, currentTime / fadeDuration);
            yield return null; 
        }

        helpPanel.SetActive(false);
        cg.alpha = 1f; 
    }
}
