using UnityEngine;

public class Test : MonoBehaviour
{
   [Header("테스트용 공 프리팹 (Ball 스크립트 필수)")]
    public GameObject testBallPrefab;

    [Header("공을 떨어뜨릴 좌표 (저울의 양쪽 위)")]
    public Vector3 leftDropPosition = new Vector3(-4.4f,5f,0f); // 좌측 저울 위쪽
    public Vector3 rightDropPosition = new Vector3(4.1f, 4.5f, 0f); // 우측 저울 위쪽

    void Update()
    {
        // A 키를 누르면 즉시 실행
        if (Input.GetKeyDown(KeyCode.A))
        {
            SpawnTestBalls();
        }
    }

    private void SpawnTestBalls()
    {
        if (testBallPrefab == null)
        {
            Logger.LogWarning("[FeverTest] 인스펙터에 테스트용 공 프리팹을 등록하세요!", this);
            return;
        }

        // 1. 공 프리팹에서 PoolType을 빼온다.
        Ball ballScript = testBallPrefab.GetComponent<Ball>();
        
        if (ballScript != null && PoolManager.Instance != null)
        {
            // 2. 우리가 완벽하게 고친 PoolManager를 통해 양쪽에 동시 소환!
            PoolManager.Instance.Spawn<Ball>(ballScript.myPoolType, leftDropPosition, Quaternion.identity);
            PoolManager.Instance.Spawn<Ball>(ballScript.myPoolType, rightDropPosition, Quaternion.identity);
            
            Logger.Log("<color=#FF00FF><b>[FeverTest]</b></color> A키 입력: 양쪽 저울에 테스트 공 투하 완료!", this);
        }
        else
        {
            // 만약 풀 매니저 세팅 전이라면 강제 Instantiate (예비용 방어 코드)
            Instantiate(testBallPrefab, leftDropPosition, Quaternion.identity);
            Instantiate(testBallPrefab, rightDropPosition, Quaternion.identity);
            Logger.Log("<color=#FF00FF><b>[FeverTest]</b></color> A키 입력: (Instantiate) 테스트 공 투하 완료!", this);
        }
    }
}
