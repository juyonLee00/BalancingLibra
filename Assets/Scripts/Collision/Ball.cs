using JusticeScale.Scripts.Scales;
using UnityEngine;
using System.Collections;


public class Ball : MonoBehaviour
{
    [Header("Ball Info")]
    public int ballLevel; 
    public PoolType myPoolType;
    public GameObject nextLevelPrefab; 

    public Rigidbody Rigidbody { get; private set; }
    public Scale CurrentScale { get; private set; }
    public bool IsMerging { get; private set;}

    private Vector3 _originalScale;

    private void Awake()
    {
        Rigidbody  = GetComponent<Rigidbody>();
        _originalScale = transform.localScale;

        if(Rigidbody != null)
        {
            Rigidbody.maxDepenetrationVelocity = 2f;
        }
    }

    private void OnEnable()
    {
        IsMerging = false;
        CurrentScale = null;

        if(Rigidbody != null)
        {
            Rigidbody.linearVelocity = Vector3.zero;
            Rigidbody.angularVelocity = Vector3.zero;
        }
        
        // collider 재활성화
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
        }
    }



    public void SetMerging(bool state)
    {
        IsMerging = state;
    }

    public void StartMergeGrouth()
    {
        StartCoroutine(GrowUpRoutine());
    }

    private IEnumerator GrowUpRoutine()
    {
        float duration = 0.15f;
        float elapsed = 0f;

        // 시작 크기를 0.5배 정도로 아주 작게 시작
        Vector3 startScale = _originalScale * 0.5f; 
        transform.localScale = startScale;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            // 부드러운 팽창 (Lerp)
            transform.localScale = Vector3.Lerp(startScale, _originalScale, elapsed / duration);
            yield return null;
        }

        transform.localScale = _originalScale;
    }

    public void SetScale(Scale scale)
    {
        CurrentScale = scale;
    }

    #if UNITY_EDITOR
    private void OnValidate()
    {
        string targetEnumName = "Ball_" + this.ballLevel;

        if (System.Enum.TryParse(targetEnumName, out PoolType parsedType))
        {
            this.myPoolType = parsedType;
        }
        else
        {
            Debug.LogWarning($"[Ball 자동화] '{targetEnumName}' 이라는 PoolType이 존재하지 않습니다. PoolType.cs를 확인하세요!", this.gameObject);
        }
    }
#endif

}