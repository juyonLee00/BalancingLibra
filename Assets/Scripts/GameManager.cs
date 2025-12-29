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

public class GameManager : MonoBehaviour
{
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

    public static GameManager Instance {get; private set;}
    public bool isGameOver = false;
    public int currentScore = 0;
    
    private int _nextBallIndex;

    private Camera mainCamera;
    private float timeCount = 0f;
    private float spawnInterval = 0.5f;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found!");
        }

        if(gameOverPanel != null) gameOverPanel.SetActive(false);

        SetNextBall();
        UpdateScoreUI();
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            HandleGameExit();
        }

        if (isGameOver) return;

        if(_currentPreviewObject != null)
        {
            _currentPreviewObject.transform.Rotate(Vector3.up * 50f * Time.deltaTime);
        }

        //클릭시 오브젝트 생성
        timeCount += Time.deltaTime;
        if (Input.GetMouseButtonDown(0) && timeCount >= spawnInterval)
        {
            SpawnBall();
            //SpawnRandomBallAtMousePosition();
        }
    }

    void SpawnBall()
    {
        if(ballList == null || ballList.Count == 0)
            return;
        
        timeCount = 0f;
        if(mainCamera == null) return;

        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = mainCamera.transform.position.z; 
        
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
        
        worldPosition.z = 0f;

        GameObject selectedBallPrefab = ballList[_nextBallIndex].prefab;

        if(selectedBallPrefab != null)
        {
            Instantiate(selectedBallPrefab, worldPosition, Quaternion.identity);
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
            Destroy(_currentPreviewObject);
        }

        GameObject prefab = ballList[_nextBallIndex].prefab;
        if(prefab != null && previewPos != null)
        {
            _currentPreviewObject = Instantiate(prefab, previewPos.position, Quaternion.identity);

            _currentPreviewObject.transform.localScale = Vector3.one;

            int previewLayerIndex = LayerMask.NameToLayer("PreviewLayer");
            SetLayerRecursively(_currentPreviewObject, previewLayerIndex);

            Rigidbody rb = _currentPreviewObject.GetComponent<Rigidbody>();

            if(rb != null)
            {
                rb.isKinematic = true;
            }

            Ball ballScript = _currentPreviewObject.GetComponent<Ball>();
            if(ballScript != null)
            {
                ballScript.enabled = false;
            }

            Collider col = _currentPreviewObject.GetComponent<Collider>();
            if(col != null) col.enabled = false;
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
        Debug.Log(reason);

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
        Time.timeScale = 1f;

        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    public void AddScore(int level)
    {
        if(isGameOver)
            return;
        
        int pointsToAdd = (int)Mathf.Pow(2, level);
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

    private void HandleGameExit()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Game Exit Triggered"); // 로그 확인용

            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
}
