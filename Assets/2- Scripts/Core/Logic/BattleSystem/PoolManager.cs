using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;

    public Dictionary<GameObject, List<GameObject>> Pools;

    private GameObject GameObjectInstance;

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
            GameObjectInstance = Instantiate(prefab, parent.position, rotation, parent);
            GameObjectInstance.SetActive(false);
            pool.Add(GameObjectInstance);
        }

        GameObjectInstance = null;
        Pools.Add(parent.gameObject, pool);
    }

    public void CreatePool(GameObject prefab, Vector3 position, Quaternion rotation, int count = 1)
    {
        List<GameObject> pool = new List<GameObject>();

        for (int i = 0; i < count; i++)
        {
            GameObjectInstance = Instantiate(prefab, position, rotation);
            GameObjectInstance.SetActive(false);
            pool.Add(GameObjectInstance);
            GameObjectInstance = null;
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
            GameObjectInstance = Instantiate(prefab, parent);
            GameObjectInstance.SetActive(false);
            pool.Add(GameObjectInstance);
            GameObjectInstance = null;
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

        foreach (GameObject item in pool)
        {
            if (!item.activeInHierarchy)
            {
                GameObjectInstance = item;
                break;
            }
        }

        if (GameObjectInstance != null)
        {
            GameObjectInstance.SetActive(true);
            return GameObjectInstance;
        }
        else
        {
            Debug.Log("Some of the bullets is not found");
        }

        Debug.LogWarning($"No available objects in pool, named {GameObjectInstance.name}!");
        return null;
    }

    public void MakeActive(GameObject key)
    {
        GameObjectInstance = Pools[key].Find(obj => !obj.activeInHierarchy);

        if (GameObjectInstance != null)
            GameObjectInstance.SetActive(true);

        Debug.LogWarning($"No available objects in pool, named {GameObjectInstance.name}!");
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
