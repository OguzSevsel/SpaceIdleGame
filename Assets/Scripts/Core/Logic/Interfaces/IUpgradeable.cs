using UnityEngine;

public enum Upgrades
{
    CollectorSpeed,
    CollectorLevel,
    CollectorAutoCollect,
    TransportHub,
    Marketplace
}

public enum UpgradeType
{
    Speed,
    Rate,
    AutoCollect,
    TransportHub,
    Marketplace
}

public interface IUpgradeable
{
#nullable enable
    public void Upgrade(Upgrades upgrade, CollectorType collectorType, ColonyType colonyType);
}
