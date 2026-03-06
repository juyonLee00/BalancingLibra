using UnityEngine;
using System.Collections.Generic;

public class PoolManager : SingletonBehaviour<PoolManager>
{
    //이름으로 구분해서 데이터 보관
    private Dictionary<PoolType, Queue<GameObject>> _poolDictionary = new Dictionary<PoolType, Queue<GameObject>>();
    
    //원본 프리팹 보관소
    private Dictionary<PoolType, GameObject> _prefabDictionary = new Dictionary<PoolType, GameObject>();
    
    private int maxPoolSizePerTier = 50; 

    //오브젝트 생성
    public void CreatePool(PoolType poolType, GameObject prefab, int initialSize)
    {
        if (!_poolDictionary.ContainsKey(poolType))
        {
            _poolDictionary.Add(poolType, new Queue<GameObject>());
            _prefabDictionary.Add(poolType, prefab);
        }

        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = Instantiate(prefab, this.transform);
            obj.SetActive(false);
            _poolDictionary[poolType].Enqueue(obj);
        }
        Logger.Log($"PoolManager: Pooling {poolType} {initialSize}.");
    }

    //Pool에서 오브젝트 꺼내기
    public T Spawn<T>(PoolType poolType, Vector3 position, Quaternion rotation) where T : Component
    {
        if (!_poolDictionary.ContainsKey(poolType))
        {
            Logger.LogError($"PoolManager: {poolType} pool is not here.");
            return null;
        }

        GameObject objToSpawn = null;
        Queue<GameObject> pool = _poolDictionary[poolType];

        if (pool.Count > 0)
        {
            objToSpawn = pool.Dequeue();
        }
        else
        {
            objToSpawn = Instantiate(_prefabDictionary[poolType], this.transform);
        }

        objToSpawn.transform.position = position;
        objToSpawn.transform.rotation = rotation;
        objToSpawn.SetActive(true);

        //이전 상태 초기화
        return objToSpawn.GetComponent<T>();
    }

    //사용한 오브젝트 다시 반납
    public void ReturnObject(PoolType poolType, GameObject obj)
    {
        // 반납하려는 오브젝트 검사
        if (!_poolDictionary.ContainsKey(poolType))
        {
            Logger.LogError($"[PoolManager] Return error: Pool {poolType} does not exist.");
            Destroy(obj);
            return;
        }

        Queue<GameObject> pool = _poolDictionary[poolType];

        if(pool.Count >= maxPoolSizePerTier)
        {
            //더 이상 보관할 자리가 없으면 파괴
            Destroy(obj);
            Logger.Log($"[PoolManager] {poolType} pool is MAX. Excess is being forcibly destroyed.");
        }
        else
        {
            // 자리가 넉넉하면 꺼서 큐에 넣는다
            obj.gameObject.SetActive(false); 
            pool.Enqueue(obj); 
        }
    }
}
