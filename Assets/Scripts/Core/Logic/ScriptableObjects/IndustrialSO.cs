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

    public override string ToString()
    {
        return CollectorName;
    }
}

[CreateAssetMenuAttribute(menuName = "ScriptableObjects/Transport Hub", fileName = "New Transport Hub")]
public class TransportHubSO : ScriptableObject
{
    
}

[CreateAssetMenuAttribute(menuName = "ScriptableObjects/Marketplace", fileName = "New Marketplace")]
public class MarketplaceSO : ScriptableObject
{
    
}


