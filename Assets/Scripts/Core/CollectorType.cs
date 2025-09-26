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


[CreateAssetMenu(fileName = "New Collector Type", menuName = "Scriptable Objects/ New Collector Type")]
public class CollectorType : ScriptableObject
{
    public CollectorTypeEnum CollectorTypeName;
    public ResourceType Resource;
    public double CollectionRate;
    public double Speed;
}
