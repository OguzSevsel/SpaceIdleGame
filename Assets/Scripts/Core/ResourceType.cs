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
    Diamond
}

[CreateAssetMenu(fileName = "New Resource Type", menuName = "Scriptable Objects/ New Resource Type")]
public class ResourceType : ScriptableObject
{
    public ResourceTypeEnum ResourceName;
    public double Amount = 0d;
    public string Unit;

    public void AddAmount(double amountToAdd)
    {
        Amount += amountToAdd;
    }
}
