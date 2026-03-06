using JusticeScale.Scripts.Scales;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [Header("Ball Info")]
    public int ballLevel; 
    public GameObject nextLevelPrefab; 

    private bool isMerging = false; 

    private Rigidbody _rigidbody;

    public Scale CurrentScale => _currentScale;
    private Scale _currentScale = null;

    private void Awake()
    {
        _rigidbody  = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        isMerging = false;
        _currentScale = null;

        if(_rigidbody != null)
        {
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }
    }

    //저울 직접 접촉
    public void RegisterScale(Scale detectedScale)
    {
        if(GameManager.Instance.isGameOver) return;

        if(_currentScale == null)
        {
            _currentScale = detectedScale;

            float myMass = _rigidbody.mass;
            float myScale = transform.localScale.x;
            GameManager.Instance.AddScore(this.ballLevel, myMass, myScale);

            if (this.ballLevel == 5)
            {
                GameManager.Instance.CheckBlackHoleCondition();
            }
        }
        
        else if(_currentScale != detectedScale)
        {
            GameManager.Instance.GameOver("Ball moves another scale");
            GameManager.Instance.LoseLife("Ball moved to another scale");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isMerging) return;

        Ball otherBall = collision.collider.attachedRigidbody?.GetComponent<Ball>();

        if(otherBall != null)
        {
            CheckScaleConsistency(otherBall);

            if(GameManager.Instance.isGameOver) return;

            if(otherBall.ballLevel == this.ballLevel)
            {
                // 중복 병합 방지 (ID가 큰 쪽에서만 한 번 실행)
                if(this.gameObject.GetInstanceID() > otherBall.gameObject.GetInstanceID())
                {
                    Merge(otherBall);
                }
            }
        }
    }

    private void CheckScaleConsistency(Ball otherBall)
    {
        if (otherBall.CurrentScale == null)
            return;
        
        //상대는 이미 scale에 속해 있을 때
        if(_currentScale == null)
        {
            RegisterScale(otherBall.CurrentScale);
        }

        //상대와 나 둘 다 다른 scale에 속해 있을 때
        else if(_currentScale != otherBall.CurrentScale)
        {
            GameManager.Instance.GameOver("Ball moved to another scale(Stack Collision)");
            GameManager.Instance.LoseLife("Ball moved to another scale(Stack Collision)");
        }
    }

    void Merge(Ball otherBall)
    {
        isMerging = true; 
        otherBall.isMerging = true;

        // 충돌 지점 중간 계산
        Vector3 spawnPos = (transform.position + otherBall.transform.position) / 2f;

        // 다음 레벨의 공이 설정되어 있다면 생성 (마지막 레벨이면 생성 안 함)
        if (nextLevelPrefab != null)
        {
            int nextLevel = this.ballLevel + 1;
            PoolType nextPoolType = (PoolType)(nextLevel - 1);

            Ball newBallScript = PoolManager.Instance.Spawn<Ball>(nextPoolType, spawnPos, Quaternion.identity);
            
            //합쳐져서 나온 공에게 현재 저울 소속 전달
            if(newBallScript != null && _currentScale != null)
            {
                newBallScript.RegisterScale(_currentScale);
            }
            else
            {
                float newMass = newBallScript.GetComponent<Rigidbody>().mass;
                float newScale = newBallScript.transform.localScale.x;
                GameManager.Instance.AddScore(this.ballLevel + 1, newMass, newScale);
            }
            
        }

        PoolType myType = (PoolType)(this.ballLevel - 1);
        PoolType otherType = (PoolType)(otherBall.ballLevel - 1);
        
        PoolManager.Instance.ReturnObject(otherType, otherBall.gameObject);
        PoolManager.Instance.ReturnObject(myType, this.gameObject);
    }
}