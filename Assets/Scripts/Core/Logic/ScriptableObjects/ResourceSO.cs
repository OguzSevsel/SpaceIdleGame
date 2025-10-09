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
    Xeronium
}

[CreateAssetMenuAttribute(menuName = "ScriptableObjects/Resource", fileName = "New Resource")]
public class ResourceSO : ScriptableObject
{
    public Sprite ResourceIcon;
    public ResourceType resourceType;
    public string ResourceUnit;
}

[System.Serializable]
public class Resource
{
    public ResourceSO ResourceSO;

    [field: SerializeField]
    public double ResourceAmount {  get; private set; }
    [field: SerializeField]
    public double SellRate { get; private set; }
    [field: SerializeField]
    public double SellRateMultiplier { get; private set; }

    public void AddResource(double amount) { ResourceAmount += amount; }
    public void SpendResource (double amount) 
    { 
        bool isEnough = CheckEnoughResource(amount);
        if (isEnough)
        {
            ResourceAmount -= amount;
        }
        else
        {
            Debug.LogWarning($"Not Enough {ResourceSO.resourceType}");
        }
    }
    public bool CheckEnoughResource(double amount)
    {
        if (ResourceAmount < amount) return false;
        return true;
    }

    public void ChangeSellRate (double value) {  SellRate = value; }
    public void ChangeSellRateMultiplier (double value) {  SellRateMultiplier = value; }
}