using UnityEngine;
using JusticeScale.Scripts.Scales;

//자동으로 Ball 스크립트도 붙여줌
[RequireComponent(typeof(Ball))]
public class BallCollisionSensor : MonoBehaviour
{
    private Ball _ball;

    private void Awake()
    {
        _ball = GetComponent<Ball>();
    }

    public void RegisterScale(Scale detectedScale)
    {
        if (GameManager.Instance.isGameOver) return;

        if (_ball.CurrentScale == null)
        {
            //ball의 데이터 갱신
            _ball.SetScale(detectedScale);

            float myMass = _ball.Rigidbody.mass;
            float myScale = transform.localScale.x;
            ScoreManager.Instance.AddScore(_ball.ballLevel, myMass, myScale, false);

            if (_ball.ballLevel == 5)
            {
                GameManager.Instance.CheckBlackHoleCondition();
            }
        }
        
        else if (_ball.CurrentScale != detectedScale)
        {
            GameManager.Instance.GameOver("Ball moves another scale");
            GameManager.Instance.LoseLife("Ball moved to another scale");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_ball.IsMerging) return;

        Ball otherBall = collision.collider.attachedRigidbody?.GetComponent<Ball>();

        if (otherBall != null)
        {
            CheckScaleConsistency(otherBall);

            if (GameManager.Instance.isGameOver) return;

            if (otherBall.ballLevel == _ball.ballLevel)
            {
                // 중복 충돌 방지
                if (this.gameObject.GetInstanceID() > otherBall.gameObject.GetInstanceID())
                {
                    MergeManager.Instance.TryMerge(_ball, otherBall);
                }
            }
        }
    }

    private void CheckScaleConsistency(Ball otherBall)
    {
        if (otherBall.CurrentScale == null) return;

        if (_ball.CurrentScale == null)
        {
            RegisterScale(otherBall.CurrentScale);
        }
        else if (_ball.CurrentScale != otherBall.CurrentScale)
        {
            GameManager.Instance.GameOver("Ball moved to another scale(Stack Collision)");
            GameManager.Instance.LoseLife("Ball moved to another scale(Stack Collision)");
        }
    }
}
