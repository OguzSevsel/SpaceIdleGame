using System.Collections.Generic;
using UnityEngine;

public interface ISaveable
{
    public object CaptureState();
    public void RestoreState(object state);
    public string GetUniqueId();
}

[System.Serializable]
public class GameData
{
    public Dictionary<string, object> savedObjects = new Dictionary<string, object>();
    public SaveMetadata metadata;
}

public struct SaveMetadata
{
    public string slotName;
    public System.DateTime lastSaved;
    public string sceneName;
    public int saveSlot;
}