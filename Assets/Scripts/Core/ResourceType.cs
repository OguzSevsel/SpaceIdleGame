using UnityEngine;

public enum ResourceTypeEnum
{
    Energy,
    Iron,
    Copper,
    Silicon,
    LimeStone,
    Gold,
    Aluminum,
    Carbon,
    Diamond,
    Money
}

[CreateAssetMenu(fileName = "New Resource Type", menuName = "Scriptable Objects/ New Resource Type")]
public class ResourceType : ScriptableObject
{
    public ResourceTypeEnum ResourceName;
    public string Unit;
}

