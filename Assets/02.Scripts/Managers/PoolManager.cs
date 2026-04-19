using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    [SerializeField] private int _defaultPoolSize = 10;

    private readonly Dictionary<string, Queue<GameObject>> _pools = new();

    public void CreatePool(string poolName, GameObject prefab, Transform spawnPoint = null, int? customSize = null)
    {
        if (_pools.ContainsKey(poolName))
        {
            Debug.Log($"[PoolManager] '{poolName}' 풀은 이미 존재합니다.");
            return;
        }

        int poolSize = customSize ?? _defaultPoolSize;
        Queue<GameObject> objectQueue = new();

        Transform parent = spawnPoint != null ? spawnPoint : transform;

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab, parent);
            obj.SetActive(false);
            objectQueue.Enqueue(obj);
        }

        _pools.Add(poolName, objectQueue);
    }

    public GameObject GetObject(string poolName, Vector3? position = null, Quaternion? rotation = null, Transform parent = null)
    {
        if (!_pools.TryGetValue(poolName, out var pool))
        {
            Debug.Log($"'{poolName}' 풀을 찾을 수 없습니다!");
            return null;
        }

        if (pool.Count == 0)
        {
            Debug.Log($"'{poolName}' 풀에 남은 오브젝트가 없습니다. 자동 확장합니다.");
            var prefab = pool.Peek(); 
            GameObject newObj = Instantiate(prefab, transform);
            newObj.SetActive(true);
            return newObj;
        }

        GameObject obj = pool.Dequeue();
        obj.SetActive(true);

        if (position.HasValue) obj.transform.position = position.Value;
        if (rotation.HasValue) obj.transform.rotation = rotation.Value;
        if (parent != null) obj.transform.SetParent(parent);

        return obj;
    }

    public void ReturnObject(string poolName, GameObject obj)
    {
        if (obj == null)
            return;

        if (!_pools.TryGetValue(poolName, out var pool))
        {
            Debug.LogWarning($"'{poolName}' 풀을 찾을 수 없습니다.");
            return;
        }

        obj.SetActive(false);
        obj.transform.SetParent(transform);
        pool.Enqueue(obj);
    }

    public void DestroyPool(string poolName)
    {
        if (!_pools.TryGetValue(poolName, out var pool))
            return;

        while (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            if (obj != null)
                Destroy(obj);
        }

        _pools.Remove(poolName);
    }

    public void ClearAllPools()
    {
        foreach (var kvp in _pools)
        {
            while (kvp.Value.Count > 0)
            {
                GameObject obj = kvp.Value.Dequeue();
                if (obj != null)
                    Destroy(obj);
            }
        }
        _pools.Clear();
    }
}
