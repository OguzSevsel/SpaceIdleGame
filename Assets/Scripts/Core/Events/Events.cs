using System.Collections.Generic;

#region Collector Events

public struct CollectorFinishedEvent
{
    public Collector Collector;
    public ColonyTypeEnum ColonyType;
    public double AmountCollected;
}

public struct CollectorStartedEvent
{
    public CollectorTypeEnum CollectorType;
    public ColonyTypeEnum ColonyType;
}

#endregion

#region Collector Upgrade Event

public struct CollectorUpgradeStartedEvent
{
    public CollectorTypeEnum? CollectorType;
    public ColonyTypeEnum? ColonyType;
    public UpgradeCategory UpgradeCategory;
    public UpgradeType UpgradeType;
    public double UpgradeMultiplier;
}

public struct CollectorUpgradeFinishedEvent
{
    public CollectorTypeEnum CollectorType;
    public ColonyTypeEnum ColonyType;
}
#endregion

#region Collector Level Amount Events

public struct CollectorLevelAmount
{
    public int Amount;
}

#endregion

#region Collector ProgressBar Events

public struct CollectorProgressEvent
{
    public CollectorTypeEnum CollectorType;
    public ColonyTypeEnum ColonyType;
    public float Progress;
    public float TimeRemaining;
}

#endregion

#region Converter Events

public struct SellUIStartedEvent
{
    public ColonyTypeEnum ColonyType;
    public TabButton TabButton;
}

public struct SellUIChangeEvent
{
    public ColonyTypeEnum ColonyType;
    public List<Collector> Collectors;
}

public struct SellButtonClickedEvent
{
    public ConvertButton Button;
}

#endregion


