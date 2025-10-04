using UnityEngine;

public enum ResourceType
{
    Money,
    Energy,
    Iron,
    Copper,
    Silicon,
    LimeStone,
    Gold,
    Aluminum,
    Carbon,
    Diamond,
}

[CreateAssetMenuAttribute(menuName = "ScriptableObjects/Resource", fileName = "New Resource")]
public class ResourceSO : ScriptableObject
{
    [HideInInspector] public string guid;

    private void OnEnable()
    {
        if (string.IsNullOrEmpty(guid))
            guid = System.Guid.NewGuid().ToString();
    }

    public string ResourceGUID => guid;
    public Sprite ResourceIcon;
    public ResourceType resourceType;
    public string ResourceUnit;
}
