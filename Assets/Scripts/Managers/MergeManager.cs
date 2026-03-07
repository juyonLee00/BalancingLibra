using UnityEngine;

public class MergeManager : SingletonBehaviour<MergeManager>
{
    public void TryMerge(Ball firstBall, Ball secondBall)
    {
        // 어느 한 쪽이 이미 병합중일 경우
        if(firstBall.IsMerging || secondBall.IsMerging) return;

        firstBall.SetMerging(true);
        secondBall.SetMerging(true);

        // 충돌 지점 계산
        Vector3 spawnPos = (firstBall.transform.position + secondBall.transform.position) / 2f;

        if (firstBall.nextLevelPrefab != null)
        {
            int nextLevel = firstBall.ballLevel + 1;
            PoolType nextPoolType = firstBall.nextLevelPrefab.GetComponent<Ball>().myPoolType;

            Ball newBall = PoolManager.Instance.Spawn<Ball>(nextPoolType, spawnPos, Quaternion.identity);

            if (newBall != null)
            {
                BallCollisionSensor sensor = newBall.GetComponent<BallCollisionSensor>();

                if(sensor != null && firstBall.CurrentScale != null)
                {
                    sensor.RegisterScale(firstBall.CurrentScale);
                }
                else
                {
                    float newMass = newBall.Rigidbody.mass;
                    float newScale = newBall.transform.localScale.x;
                
                    ScoreManager.Instance.AddScore(nextLevel, newMass, newScale, GameManager.Instance.isFeverTime);
                }
            }
        }
        PoolManager.Instance.ReturnObject(firstBall.myPoolType, firstBall.gameObject);
        PoolManager.Instance.ReturnObject(secondBall.myPoolType, secondBall.gameObject);
    }
}
