using UnityEngine;
using BalancingLibra.Data; // 네임스페이스 필수

public class DataTestManager : MonoBehaviour
{
    private void Update()
    {
        // 1. 데이터 변경 (돈 벌기)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            UserDataManager.Instance.Goods.Gold += 1000;
            Debug.Log($"[Test] Gold 증가: {UserDataManager.Instance.Goods.Gold}");
        }

        // 2. 특정 데이터만 저장 (Goods만 저장)
        if (Input.GetKeyDown(KeyCode.S))
        {
            UserDataManager.Instance.SaveSpecificData(UserDataManager.Instance.Goods);
            Debug.Log("[Test] Goods 데이터 개별 저장 요청");
        }

        // 3. 특정 데이터만 삭제 (Setting만 삭제)
        if (Input.GetKeyDown(KeyCode.D))
        {
            UserDataManager.Instance.DeleteSpecificData(UserDataManager.Instance.Setting);
            Debug.Log("[Test] Setting 데이터 개별 삭제 요청 (Sound 초기화됨)");
        }

        // 4. 전체 초기화
        if (Input.GetKeyDown(KeyCode.R))
        {
            UserDataManager.Instance.ResetAllData();
            Debug.Log("[Test] 모든 데이터 초기화 완료");
        }

        // 5. 데이터 조회 (현재 상태 확인)
        if (Input.GetKeyDown(KeyCode.P))
        {
            long gold = UserDataManager.Instance.Goods.Gold;
            bool sound = UserDataManager.Instance.Setting.Sound;
            Debug.Log($"[Test] 현재 상태 -> Gold: {gold} | Sound: {sound}");
        }

        // 6. PlayerPrefs 생성 (Sound 값 변경 및 저장)
        if (Input.GetKeyDown(KeyCode.A))
        {
            UserDataManager.Instance.Setting.Sound = !UserDataManager.Instance.Setting.Sound;
            
            UserDataManager.Instance.SaveSpecificData(UserDataManager.Instance.Setting);
            
            Debug.Log($"[Test] Sound 변경 및 저장됨: {UserDataManager.Instance.Setting.Sound}");
        }
    }
}