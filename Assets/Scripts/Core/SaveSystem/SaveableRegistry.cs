using System.Collections.Generic;
using UnityEngine;


public class SaveableRegistry : MonoBehaviour
{
    public static SaveableRegistry Instance { get; private set; }
    private List<ISaveable> saveables;

    private void Awake()
    {
        saveables = new List<ISaveable>();   
    }

    private void OnEnable()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Register(ISaveable saveable)
    {
        if (!saveables.Contains(saveable))
            saveables.Add(saveable);
    }

    public void Unregister(ISaveable saveable)
    {
        saveables.Remove(saveable);
    }

    public IEnumerable<ISaveable> GetAllSaveables() => saveables;
}
