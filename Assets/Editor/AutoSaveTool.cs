using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class AutoSaveTool : EditorWindow
{
    private const string PREF_KEY_ENABLED = "AutoSave_Enabled";
    private const string PREF_KEY_INTERVAL = "AutoSave_Interval";

    private bool _isAutoSaveEnabled = true;
    private float _saveIntervalMinutes = 5f;
    private double _lastSaveTime;

    // 최소 저장 시간 간격 (1분)
    private float _minSaveTime = 1f;

    private int _lastDisplayedSeconds = -1;

    // 상단 메뉴 등록
    [MenuItem("Tools/Auto Save Settings")]
    public static void ShowWindow()
    {
        // 최소 창 크기 고정
        AutoSaveTool window = GetWindow<AutoSaveTool>("씬 자동 저장 설정");
        window.minSize = new Vector2(300, 200);
    }

    private void OnEnable()
    {
        // 유니티가 꺼져도 해당 조건은 유지
        _isAutoSaveEnabled = EditorPrefs.GetBool("AutoSave_Enabled", true);
        _saveIntervalMinutes = EditorPrefs.GetFloat("AutoSave_Interval", 5f);
        _lastSaveTime = EditorApplication.timeSinceStartup;
        
        EditorApplication.update += OnEditorUpdate;
    }

    private void OnDisable()
    {
        EditorApplication.update -= OnEditorUpdate;
    }

    private void LoadSettings()
    {
        _isAutoSaveEnabled = EditorPrefs.GetBool(PREF_KEY_ENABLED, true);
        _saveIntervalMinutes = EditorPrefs.GetFloat(PREF_KEY_INTERVAL, 5f);
    }

    private void SaveSettings()
    {
        EditorPrefs.SetBool(PREF_KEY_ENABLED, _isAutoSaveEnabled);
        EditorPrefs.SetFloat(PREF_KEY_INTERVAL, _saveIntervalMinutes);
    }

    private void OnGUI()
    {
        DrawHeader();
        DrawSettings();
        DrawStatus();
    }
    
    private void DrawHeader()
    {
        GUILayout.Space(10);
        GUILayout.Label("씬 자동 저장 컨트롤러", EditorStyles.boldLabel);
        GUILayout.Space(10);
    }

    private void DrawSettings()
    {
        // 실제로 조작 가했을 때만 체크
        EditorGUI.BeginChangeCheck();
        
        _isAutoSaveEnabled = EditorGUILayout.Toggle("자동 저장 켜기", _isAutoSaveEnabled);
        _saveIntervalMinutes = EditorGUILayout.FloatField("저장 주기 (분)", _saveIntervalMinutes);
        _saveIntervalMinutes = Mathf.Max(_minSaveTime, _saveIntervalMinutes);

        if (EditorGUI.EndChangeCheck())
        {
            SaveSettings();
            //타이머 리셋
            _lastSaveTime = EditorApplication.timeSinceStartup;
        }
    }

    private void DrawStatus()
    {
        GUILayout.Space(20);

        if (_isAutoSaveEnabled)
        {
            double timeRemaining = GetTimeRemaining();
            GUILayout.Label($"다음 자동 저장까지: {Mathf.Max(0, (float)timeRemaining):F0}초", EditorStyles.helpBox);
        }
        else
        {
            GUILayout.Label("자동 저장 기능이 정지되었습니다.", EditorStyles.helpBox);
        }

        GUILayout.Space(10);

        if (GUILayout.Button("지금 즉시 저장 (수동)", GUILayout.Height(35)))
        {
            ExecuteSave();
        }
    }

    // 남은 시간(초) 계산 함수
    private double GetTimeRemaining()
    {
        double timeSinceLastSave = EditorApplication.timeSinceStartup - _lastSaveTime;
        return (_saveIntervalMinutes * 60f) - timeSinceLastSave;
    }

    private void OnEditorUpdate()
    {
        if (!_isAutoSaveEnabled || EditorApplication.isPlaying || EditorApplication.isCompiling)
            return;

        double timeRemaining = GetTimeRemaining();

        if (timeRemaining <= 0)
        {
            ExecuteSave();
        }

        // 초 단위 바뀔 때만 UI 갱신
        int currentSeconds = Mathf.CeilToInt((float)timeRemaining);
        if (currentSeconds != _lastDisplayedSeconds)
        {
            _lastDisplayedSeconds = currentSeconds;
            Repaint(); 
        }
    }

    private void ExecuteSave()
    {
        if (EditorSceneManager.SaveOpenScenes())
        {
            Logger.Log($"<color=#00FF00><b>[AutoSave]</b></color> Scene auto-save complete! (period: {_saveIntervalMinutes}minutes)");
        }
        _lastSaveTime = EditorApplication.timeSinceStartup;
    }
}