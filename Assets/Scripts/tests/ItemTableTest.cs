using UnityEngine;
using BalancingLibra.Data;

// ItemTable 기능 테스트
public class ItemTableTest : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("=== ItemTable 테스트 시작 ===");
        
        TestItemTableLoad();
        TestItemDataMapping();
        
        Debug.Log("=== ItemTable 테스트 완료 ===");
    }

    // ItemTable 로드 테스트
    private void TestItemTableLoad()
    {
        Debug.Log("\n--- ItemTable 로드 테스트 ---");
        
        // DataTableManager에서 ItemTable 가져오기
        ItemTable itemTable = DataTableManager.Instance.Get<ItemTable>();
        
        if (itemTable == null)
        {
            Debug.LogError("[Test] ItemTable을 가져올 수 없습니다.");
            return;
        }
        
        Debug.Log($"[Test] ItemTable 로드 성공");
        Debug.Log($"[Test] 로드된 아이템 개수: {itemTable.Count}");
        
        // 요구사항: "[ItemTable] 아이템 n개가 로드되었습니다." 로그 확인
        // 이 로그는 ItemTable.Load() 메서드에서 이미 출력됩니다.
    }

    // CSV 데이터 매핑 테스트
    private void TestItemDataMapping()
    {
        Debug.Log("\n--- CSV 데이터 매핑 테스트 ---");
        
        ItemTable itemTable = DataTableManager.Instance.Get<ItemTable>();
        
        if (itemTable == null)
        {
            Debug.LogError("[Test] ItemTable을 가져올 수 없습니다.");
            return;
        }
        
        // CSV 파일에 있는 데이터 확인
        // id=1: 검, 강력한 검입니다
        // id=2: 방패, 튼튼한 방패입니다
        // id=3: 물약, 체력을 회복시켜줍니다
        // id=4: 활, 정확한 활입니다
        // id=5: 갑옷, 강력한 방어력을 제공합니다
        
        // 테스트 케이스 1: ID 1 - 검
        ItemData item1 = itemTable.GetItem(1);
        bool test1 = item1 != null && item1.id == 1 && item1.name == "검" && item1.description == "강력한 검입니다";
        Debug.Log($"[Test 1] ID 1 검 테스트");
        Debug.Log($"  결과: id={item1?.id}, name={item1?.name}, description={item1?.description}");
        Debug.Log($"  성공: {test1}");
        
        // 테스트 케이스 2: ID 2 - 방패
        ItemData item2 = itemTable.GetItem(2);
        bool test2 = item2 != null && item2.id == 2 && item2.name == "방패" && item2.description == "튼튼한 방패입니다";
        Debug.Log($"[Test 2] ID 2 방패 테스트");
        Debug.Log($"  결과: id={item2?.id}, name={item2?.name}, description={item2?.description}");
        Debug.Log($"  성공: {test2}");
        
        // 테스트 케이스 3: ID 3 - 물약
        ItemData item3 = itemTable.GetItem(3);
        bool test3 = item3 != null && item3.id == 3 && item3.name == "물약" && item3.description == "체력을 회복시켜줍니다";
        Debug.Log($"[Test 3] ID 3 물약 테스트");
        Debug.Log($"  결과: id={item3?.id}, name={item3?.name}, description={item3?.description}");
        Debug.Log($"  성공: {test3}");
        
        // 테스트 케이스 4: ID 4 - 활
        ItemData item4 = itemTable.GetItem(4);
        bool test4 = item4 != null && item4.id == 4 && item4.name == "활" && item4.description == "정확한 활입니다";
        Debug.Log($"[Test 4] ID 4 활 테스트");
        Debug.Log($"  결과: id={item4?.id}, name={item4?.name}, description={item4?.description}");
        Debug.Log($"  성공: {test4}");
        
        // 테스트 케이스 5: ID 5 - 갑옷
        ItemData item5 = itemTable.GetItem(5);
        bool test5 = item5 != null && item5.id == 5 && item5.name == "갑옷" && item5.description == "강력한 방어력을 제공합니다";
        Debug.Log($"[Test 5] ID 5 갑옷 테스트");
        Debug.Log($"  결과: id={item5?.id}, name={item5?.name}, description={item5?.description}");
        Debug.Log($"  성공: {test5}");
        
        // 테스트 케이스 6: 존재하지 않는 ID
        ItemData item6 = itemTable.GetItem(999);
        bool test6 = item6 == null;
        Debug.Log($"[Test 6] 존재하지 않는 ID 테스트");
        Debug.Log($"  결과: {item6}");
        Debug.Log($"  성공: {test6}");
        
        // 전체 테스트 결과
        bool allTestsPassed = test1 && test2 && test3 && test4 && test5 && test6;
        Debug.Log($"\n[Test 결과] 모든 매핑 테스트: {(allTestsPassed ? "성공" : "실패")}");
        
        // 모든 아이템 출력
        Debug.Log("\n[전체 아이템 목록]");
        foreach (var kvp in itemTable.Items)
        {
            ItemData item = kvp.Value;
            Debug.Log($"  ID: {item.id}, Name: {item.name}, Description: {item.description}");
        }
    }
}
