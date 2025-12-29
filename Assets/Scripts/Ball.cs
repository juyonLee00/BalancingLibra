using JusticeScale.Scripts.Scales;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [Header("Ball Info")]
    public int ballLevel; 
    public GameObject nextLevelPrefab; 

    private bool isMerging = false; 

    public Scale CurrentScale => _currentScale;
    private Scale _currentScale = null;

    //저울 직접 접촉
    public void RegisterScale(Scale detectedScale)
    {
        if(GameManager.Instance.isGameOver) return;

        if(_currentScale == null)
        {
            _currentScale = detectedScale;
            GameManager.Instance.AddScore(this.ballLevel);
            return;
        }
        
        if(_currentScale != detectedScale)
        {
            GameManager.Instance.GameOver("Ball moves another scale");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 1. 이미 합쳐지는 중이면 무시
        if (isMerging) return;

        // 2. 충돌한 물체에 Ball 스크립트가 있는지 확인
        Ball otherBall = collision.gameObject.GetComponent<Ball>();

        // 3. 상대방도 Ball이고, 나와 레벨이 같다면 합체 조건 성립
        if(otherBall != null)
        {
            CheckScaleConsistency(otherBall);

            if(otherBall.ballLevel == this.ballLevel)
            {
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
        else if(_currentScale != otherBall._currentScale)
        {
            GameManager.Instance.GameOver("Ball moved to another scale(Stack Collision)");
        }
    }

    void Merge(Ball otherBall)
    {
        // 중복 실행 방지
        isMerging = true; 
        otherBall.isMerging = true;

        // 충돌 지점 중간 계산
        Vector3 spawnPos = (transform.position + otherBall.transform.position) / 2f;

        // 다음 레벨의 공이 설정되어 있다면 생성 (마지막 레벨 수박이면 생성 안 함)
        if (nextLevelPrefab != null)
        {
            GameObject newBallObj = Instantiate(nextLevelPrefab, spawnPos, Quaternion.identity);
            
            //합쳐져서 나온 공에게 현재 저울 소속 전달
            Ball newBallScript = newBallObj.GetComponent<Ball>();
            if(newBallScript != null && _currentScale != null)
            {
                //RegisterScale을 통해 점수 얻고, 소속도 등록
                newBallScript.RegisterScale(_currentScale);
            }
            else
            {
                GameManager.Instance.AddScore(this.ballLevel + 1);
            }
            
        }

        // 현재 공과 충돌한 공 삭제
        Destroy(otherBall.gameObject);
        Destroy(this.gameObject);
    }
}
