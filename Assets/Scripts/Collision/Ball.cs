using JusticeScale.Scripts.Scales;
using UnityEngine;


public class Ball : MonoBehaviour
{
    [Header("Ball Info")]
    public int ballLevel; 
    public PoolType myPoolType;
    public GameObject nextLevelPrefab; 

    public Rigidbody Rigidbody { get; private set; }
    public Scale CurrentScale { get; private set; }
    public bool IsMerging { get; private set;}

    private void Awake()
    {
        Rigidbody  = GetComponent<Rigidbody>();
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
    }

    public void SetMerging(bool state)
    {
        IsMerging = state;
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