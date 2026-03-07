using System;
using System.Collections.Generic;
using BalancingLibra.Data;
public class DataTableManager : SingletonBehaviour<DataTableManager>
{
    // 모든 테이블 타입 key-value로 설정해서 보관
    private Dictionary<Type, IDataTable> _tables = new Dictionary<Type, IDataTable>();

    protected override void Init()
    {
        base.Init();
        LoadAllTables();
    }

    public void LoadAllTables()
    {
        // 게임에 필요한 모든 데이터 테이블 등록 및 로드
        // LoadTable<ItemTable>("Data/ItemTable");

        Logger.Log("DataTableManager : All tables loaded");
    }

    // 제네릭 로드 함수: 테이블 객체 생성 후 Load 호출, 딕셔너리에 등록
    private void LoadTable<T>(string path) where T : IDataTable, new()
    {
        T table = new T();
        table.Load(path);

        if(_tables.ContainsKey(typeof(T)))
            _tables[typeof(T)] = table;
        else
            _tables.Add(typeof(T), table);
    }

    // 외부에서 특정 테이블 가져오는 함수
    public T Get<T>() where T : class, IDataTable
    {
        if(_tables.TryGetValue(typeof(T), out IDataTable table))
        {
            return table as T;
        }

        Logger.Log($"DataTableManager : Failed to get table {typeof(T).Name}");
        return null;
    }

}
