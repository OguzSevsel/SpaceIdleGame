using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;

    public Dictionary<GameObject, List<GameObject>> Pools;

    private void Awake()
    {
        Instance = this;
        Pools = new Dictionary<GameObject, List<GameObject>>();
    }

    public void CreatePool(GameObject prefab, Transform parent, Quaternion rotation, int count = 1)
    {
        if (Pools.ContainsKey(parent.gameObject))
        {
            return;
        }

        List<GameObject> pool = new List<GameObject>();

        for (int i = 0; i < count; i++)
        {
            var gameObject = Instantiate(prefab, parent.position, rotation, parent);
            gameObject.SetActive(false);
            pool.Add(gameObject);
        }

        Pools.Add(parent.gameObject, pool);
    }

    public void CreatePool(GameObject prefab, Vector3 position, Quaternion rotation, int count = 1)
    {
        List<GameObject> pool = new List<GameObject>();

        for (int i = 0; i < count; i++)
        {
            var gameObject = Instantiate(prefab, position, rotation);
            gameObject.SetActive(false);
            pool.Add(gameObject);
        }

        Pools.Add(new GameObject("Pool"), pool);
    }

    public void CreatePool(GameObject prefab, Transform parent, int count = 1)
    {
        if (Pools.ContainsKey(parent.gameObject))
        {
            return;
        }

        List<GameObject> pool = new List<GameObject>();

        for (int i = 0; i < count; i++)
        {
            var gameObject = Instantiate(prefab, parent);
            gameObject.SetActive(false);
            pool.Add(gameObject);
        }

        Pools.Add(parent.gameObject, pool);
    }

    public void DestroyPool(GameObject key)
    {
        Pools.Remove(key);
    }

    public void ClearAllPools()
    {
        Pools.Clear();
    }

    public GameObject Get(GameObject key)
    {
        var pool = Pools[key];
        var gameObject = pool[0];

        foreach (GameObject item in pool)
        {
            if (!item.activeInHierarchy)
            {
                gameObject = item;
                return gameObject;
            }
        }

        Debug.LogWarning($"No available objects in pool, named {gameObject.name}!");
        return null;
    }

    public void AddToPool(GameObject key, GameObject poolItem)
    {
        var pool = Pools[key];
        bool isPoolFull = true;

        foreach (GameObject item in pool)
        {
            if (!item.activeInHierarchy)
            {
                isPoolFull = false;
                break;
            }
        }

        if (isPoolFull)
        {
            pool.Add(poolItem);
        }
    }

    public List<GameObject> GetPool(GameObject key)
    {
        if (!Pools.ContainsKey(key))
        {
            return null;
        }

        var pool = Pools[key];

        if (pool != null)
        {
            return pool;
        }

        Debug.LogWarning($"Pool with key {key.name} does not exist!");
        return null;
    }

    public void ReturnToPool(GameObject key, GameObject obj)
    {
        var pool = Pools[key];

        if (pool.Contains(obj))
            obj.SetActive(false);

        Debug.LogWarning($"{obj.name} does not belong to pool {key.name}!");
    }
}
