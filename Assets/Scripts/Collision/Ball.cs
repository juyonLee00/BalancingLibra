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
        // 1. ballLevel 변수를 기반으로 우리가 찾고 싶은 Enum 이름을 문자로 조립한다.
        // 예: ballLevel이 1이면 "Ball_1", 0이면 "Ball_0"
        string targetEnumName = "Ball_" + this.ballLevel;

        // 2. 조립한 문자와 똑같이 생긴 PoolType이 있는지 검색해서, 있다면 자동으로 꽂아 넣는다.
        if (System.Enum.TryParse(targetEnumName, out PoolType parsedType))
        {
            this.myPoolType = parsedType;
        }
        else
        {
            // 만약 PoolType.cs 에 Ball_0 이 없는데 ballLevel을 0으로 적었다면 경고를 띄워준다.
            Debug.LogWarning($"[Ball 자동화] '{targetEnumName}' 이라는 PoolType이 존재하지 않습니다. PoolType.cs를 확인하세요!", this.gameObject);
        }
    }
#endif

}