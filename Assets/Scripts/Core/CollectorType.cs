using System;
using System.Collections.Generic;
using UnityEngine;

public enum  CollectorTypeEnum
{
    EnergyCollector,
    IronCollector,
    CopperCollector,
    SiliconCollector,
    LimeStoneCollector,
    GoldCollector,
    AluminiumCollector,
    CarbonCollector,
    DiamondCollector
}

[Serializable]
public class CostResource
{
    public ResourceType resourceType;
    public double amount;
    public double BaseAmount;
}

[CreateAssetMenu(fileName = "New Collector Type", menuName = "Scriptable Objects/ New Collector Type")]
public class CollectorType : ScriptableObject
{
    public CollectorTypeEnum CollectorTypeName;
    public ResourceType generatedResource;
    public List<CostResource> costResourcesToUpgrade;
    public double CollectionRate;
    public double Speed;
    public int Level;
    public double BaseProduction;
    public double BaseSpeed;
}
