public enum UpgradeType
{
    CollectorSpeed,
    CollectorAmount,
    AutoCollect,
    CostReduction,
    ConvertRate,
    AutoConvert,
    BoostTransportSpeed,
    ReduceTransportCost,
    AutoTransport,
}

public enum UpgradeCategory
{
    Global,
    Local
}

public interface IUpgradeable
{
    public void Upgrade(UpgradeCategory upgradeCategory, UpgradeType upgradeType, ColonyTypeEnum? colonyType, CollectorTypeEnum? collectorType, double upgradeAmount);
}
