using System;
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
    DiamondCollector
}

[CreateAssetMenuAttribute(menuName = "ScriptableObjects/Collector", fileName = "New Collector")]
public class CollectorSO : ScriptableObject
{
    [HideInInspector] public string _guid;

    private void OnEnable()
    {
        if (string.IsNullOrEmpty(_guid))
            _guid = System.Guid.NewGuid().ToString();
    }

    public string CollectorGUID => _guid;
    public string CollectorName;
    public CollectorType CollectorType;
    public ResourceSO GeneratedResource;
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
    [SerializeField] private double _costMultiplier = 1.05;

    public void AdjustAmountAndMultiplier(int level, double? overrideCostMultiplier = null)
    {
        if (overrideCostMultiplier != null)
        {
            _costMultiplier = (double)overrideCostMultiplier;
        }
        
        _costAmount = _baseCostAmount * Math.Pow(_costMultiplier, (double)level);
    }

    public double GetCostAmount() { return _costAmount; }
}
