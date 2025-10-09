using System;
using System.Transactions;
using UnityEngine;

public enum CollectorType
{
    EnergyCollector,
    IronCollector,
    CopperCollector,
    SiliconCollector,
    LimeStoneCollector,
    GoldCollector,
    AluminumCollector,
    CarbonCollector,
    DiamondCollector,
    XeroniumCollector,
}

[CreateAssetMenuAttribute(menuName = "ScriptableObjects/Collector", fileName = "New Collector")]
public class CollectorSO : ScriptableObject
{
    public string CollectorName;
    public CollectorType CollectorType;
    public ResourceSO GeneratedResource;
    [Range(0f, 2f)] public double _baseCollectionRate;
}

[CreateAssetMenuAttribute(menuName = "ScriptableObjects/Transport Hub", fileName = "New Transport Hub")]
public class TransportHubSO : ScriptableObject
{
    [HideInInspector] public string _guid;

    private void OnEnable()
    {
        if (string.IsNullOrEmpty(_guid))
            _guid = System.Guid.NewGuid().ToString();
    }

    public string TransportHubGUID => _guid;
}

[CreateAssetMenuAttribute(menuName = "ScriptableObjects/Marketplace", fileName = "New Marketplace")]
public class MarketplaceSO : ScriptableObject
{
    [HideInInspector] public string _guid;

    private void OnEnable()
    {
        if (string.IsNullOrEmpty(_guid))
            _guid = System.Guid.NewGuid().ToString();
    }

    public string MarketplaceGUID => _guid;
}

[System.Serializable]
public class CostResource
{
    public ResourceSO Resource;
    [SerializeField] private double _costAmount;
    [SerializeField] private double _baseCostAmount;
    [SerializeField] private double _costMultiplier;

    public void AdjustAmountAndMultiplier(int level, int levelIncrement, double? overrideCostMultiplier = null)
    {
        if (overrideCostMultiplier != null)
        {
            _costMultiplier = (double)overrideCostMultiplier;
        }

        if (levelIncrement != 1)
        {
            _costAmount = GetTotalCost(level, levelIncrement);
        }
        else
        {
            _costAmount = GetNextCost(level);
        }
    }

    public double GetNextCost(int level)
    {
        return _baseCostAmount * Math.Pow(_costMultiplier, level);
    }

    public double GetTotalCost(int level, int levelIncrement)
    {
        return _baseCostAmount * (Math.Pow(_costMultiplier, level) * (Math.Pow(_costMultiplier, levelIncrement) - 1)) / (_costMultiplier - 1);
    }

    public void AdjustAmount(int levelIncrement, int currentLevel)
    {
        double totalCost = 0d;

        switch (levelIncrement)
        {
            case 1:
                totalCost = GetTotalCost(currentLevel, levelIncrement);
                break;
            case 5:
                totalCost = GetTotalCost(currentLevel, levelIncrement);
                break;
            case 10:
                totalCost = GetTotalCost(currentLevel, levelIncrement);
                break;
            case 100:
                totalCost = GetTotalCost(currentLevel, levelIncrement);
                break;
            default:
                break;
        }

        _costAmount = totalCost;
    }

    public double GetCostAmount() { return _costAmount; }
}
