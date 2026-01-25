using UnityEngine;
using System.Collections.Generic;
using BalancingLibra.Data;


// CSVReader와 DataTableManager 기능 테스트
public class CSVReaderAndDataTableTest : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("=== CSVReader 및 DataTableManager 테스트 시작 ===");
        
        TestCSVReader();
        TestDataTableManager();
        
        Debug.Log("=== 모든 테스트 완료 ===");
    }

    // CSVReader 기능 테스트
    private void TestCSVReader()
    {
        Debug.Log("\n--- CSVReader 테스트 시작 ---");
        
        // 테스트 1: ParseLine - 기본 CSV 라인 파싱
        string testLine1 = "id,name,value";
        string[] result1 = CSVReader.ParseLine(testLine1);
        Debug.Log($"[Test 1] ParseLine 테스트");
        Debug.Log($"  입력: {testLine1}");
        Debug.Log($"  결과: [{string.Join(", ", result1)}]");
        Debug.Log($"  성공: {result1.Length == 3 && result1[0] == "id" && result1[1] == "name" && result1[2] == "value"}");
        
        // 테스트 2: ParseLine - 빈 값 포함
        string testLine2 = "1,Item A,100";
        string[] result2 = CSVReader.ParseLine(testLine2);
        Debug.Log($"[Test 2] ParseLine - 숫자 포함 테스트");
        Debug.Log($"  입력: {testLine2}");
        Debug.Log($"  결과: [{string.Join(", ", result2)}]");
        Debug.Log($"  성공: {result2.Length == 3 && result2[0] == "1" && result2[1] == "Item A" && result2[2] == "100"}");
        
        // 테스트 3: ParseCSV - 헤더 제외 파싱
        string testCSV1 = "id,name,value\n1,Item A,100\n2,Item B,200\n3,Item C,300";
        List<string[]> result3 = CSVReader.ParseCSV(testCSV1);
        Debug.Log($"[Test 3] ParseCSV - 기본 테스트");
        Debug.Log($"  입력 CSV:\n{testCSV1}");
        Debug.Log($"  파싱된 행 수: {result3.Count} (헤더 제외)");
        Debug.Log($"  첫 번째 행: [{string.Join(", ", result3[0])}]");
        Debug.Log($"  성공: {result3.Count == 3 && result3[0][0] == "1" && result3[1][1] == "Item B"}");
        
        // 테스트 4: ParseCSV - 빈 줄 처리
        string testCSV2 = "id,name,value\n1,Item A,100\n\n2,Item B,200\n";
        List<string[]> result4 = CSVReader.ParseCSV(testCSV2);
        Debug.Log($"[Test 4] ParseCSV - 빈 줄 처리 테스트");
        Debug.Log($"  입력 CSV:\n{testCSV2}");
        Debug.Log($"  파싱된 행 수: {result4.Count} (빈 줄 제외)");
        Debug.Log($"  성공: {result4.Count == 2}");
        
        // 테스트 5: ParseCSV - 헤더만 있는 경우
        string testCSV3 = "id,name,value";
        List<string[]> result5 = CSVReader.ParseCSV(testCSV3);
        Debug.Log($"[Test 5] ParseCSV - 헤더만 있는 경우");
        Debug.Log($"  입력 CSV: {testCSV3}");
        Debug.Log($"  파싱된 행 수: {result5.Count}");
        Debug.Log($"  성공: {result5.Count == 0}");
        
        // 테스트 6: ParseCSV - 빈 CSV
        string testCSV4 = "";
        List<string[]> result6 = CSVReader.ParseCSV(testCSV4);
        Debug.Log($"[Test 6] ParseCSV - 빈 CSV 테스트");
        Debug.Log($"  입력 CSV: (빈 문자열)");
        Debug.Log($"  파싱된 행 수: {result6.Count}");
        Debug.Log($"  성공: {result6.Count == 0}");
        
        Debug.Log("--- CSVReader 테스트 완료 ---\n");
    }

    // DataTableManager 기능 테스트
    private void TestDataTableManager()
    {
        Debug.Log("--- DataTableManager 테스트 시작 ---");
        
        // 테스트용 Mock 테이블 클래스 생성
        TestDataTable testTable = new TestDataTable();
        
        // 테스트 1: IDataTable.Load 메서드 테스트
        Debug.Log("[Test 1] IDataTable.Load 메서드 테스트");
        testTable.Load("Test/Path");
        Debug.Log($"  Load 호출 후 데이터: {testTable.Data}");
        bool test1Success = testTable.Data == "Test/Path";
        Debug.Log($"  성공: {test1Success}");
        
        // 테스트 2: DataTableManager 인스턴스 확인
        Debug.Log("[Test 2] DataTableManager 인스턴스 확인");
        DataTableManager manager = DataTableManager.Instance;
        bool test2Success = manager != null;
        Debug.Log($"  인스턴스 존재: {manager != null}");
        Debug.Log($"  성공: {test2Success}");
        
        if (!test2Success)
        {
            Debug.LogError("  DataTableManager 인스턴스를 가져올 수 없습니다. 테스트를 중단합니다.");
            return;
        }
        
        // 테스트 3: LoadTable 메서드 - 테이블 로드 및 등록
        Debug.Log("[Test 3] LoadTable 메서드 - 테이블 로드 및 등록");
        // manager.LoadTable<TestDataTable>("Test/Data/Path");  // LoadTable이 private이므로 주석처리
        TestDataTable loadedTable = manager.Get<TestDataTable>();
        bool test3Success = loadedTable != null && loadedTable.Data == "Test/Data/Path";
        Debug.Log($"  로드된 테이블 존재: {loadedTable != null}");
        if (loadedTable != null)
        {
            Debug.Log($"  로드된 테이블 데이터: {loadedTable.Data}");
        }
        Debug.Log($"  성공: {test3Success}");
        
        // 테스트 4: LoadTable 메서드 - 중복 로드 시 업데이트 확인
        Debug.Log("[Test 4] LoadTable 메서드 - 중복 로드 시 업데이트 확인");
        // manager.LoadTable<TestDataTable>("Test/Updated/Path");  // LoadTable이 private이므로 주석처리
        TestDataTable updatedTable = manager.Get<TestDataTable>();
        bool test4Success = updatedTable != null && updatedTable.Data == "Test/Updated/Path";
        Debug.Log($"  업데이트된 테이블 데이터: {(updatedTable != null ? updatedTable.Data : "null")}");
        Debug.Log($"  이전 테이블 데이터: {loadedTable.Data}");
        Debug.Log($"  성공: {test4Success}");
        
        // 테스트 5: Get 메서드 - 존재하지 않는 테이블
        Debug.Log("[Test 5] Get 메서드 - 존재하지 않는 테이블");
        TestDataTable2 nonExistentTable = manager.Get<TestDataTable2>();
        bool test5Success = nonExistentTable == null;
        Debug.Log($"  결과: {nonExistentTable}");
        Debug.Log($"  성공: {test5Success}");
        
        Debug.Log("--- DataTableManager 테스트 완료 ---\n");
    }
    
    // 테스트용 Mock IDataTable 구현 2 (존재하지 않는 테이블 테스트용)
    private class TestDataTable2 : IDataTable
    {
        public string Data { get; private set; } = "";
        
        public void Load(string path)
        {
            Data = path;
            Debug.Log($"[TestDataTable2] Load 호출됨: {path}");
        }
    }

    // 테스트용 Mock IDataTable 구현
    private class TestDataTable : IDataTable
    {
        public string Data { get; private set; } = "";
        
        public void Load(string path)
        {
            Data = path;
            Debug.Log($"[TestDataTable] Load 호출됨: {path}");
        }
    }
}
