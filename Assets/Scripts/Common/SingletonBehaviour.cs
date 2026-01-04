using UnityEngine;

public class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>
{
    private static object _lock = new object();
    private static bool _applicationIsQuitting = false;
    private static T _instance;

    public static T Instance
    {
        get
        {
            //게임 종료 시 null 반환
            if(_applicationIsQuitting)
            {
                Logger.LogError($"[SingletonBehaviour] Instance '{typeof(T)}' is already destroyed. Returning null.");
                return null;
            }

            //스레드 안전성 확보
            lock(_lock)
            {
                if(_instance == null)
                {
                    //씬에 이미 존재하는지 찾기
                    _instance = FindAnyObjectByType<T>();

                    if(FindObjectsByType<T>(FindObjectsSortMode.None).Length > 1)
                    {
                        Logger.LogError($"[SingletonBehaviour] Multiple instances of '{typeof(T)}' found. Reopening the scene might fix it.");
                        return _instance;
                    }
                    
                    //씬에 없으면 새로 생성
                    else if(_instance == null)
                    {
                        GameObject singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();
                        singleton.name = "[Singleton] {typeof(T)}";

                        DontDestroyOnLoad(singleton);

                        Logger.Log($"[SingletonBehaviour] An instance of '{typeof(T)}' is needed in the scene, so '{singleton}' was created with DontDestroyOnLoad.");
                    }

                    //씬에 없으면 새로 생성
                    else
                    {
                        DontDestroyOnLoad(_instance.gameObject);
                        Logger.Log($"[SingletonBehaviour] An instance of '{typeof(T)}' is needed in the scene, so '{_instance.gameObject}' was created with DontDestroyOnLoad.");
                    }
                }
                return _instance;
            }
        }
    }

    protected virtual void Awake()
    {
        Init();
    }

    //중복 생성 방지
    protected virtual void Init()
    {
        if(_instance == null)
        {
            _instance = (T)this;
            DontDestroyOnLoad(gameObject);
        }
        else if(_instance != this)
        {
            Destroy(gameObject);
        }
    }

    //삭제 시 실행
    protected virtual void OnApplicationQuit() 
    {
        _applicationIsQuitting = true;
    }

    protected virtual void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}
