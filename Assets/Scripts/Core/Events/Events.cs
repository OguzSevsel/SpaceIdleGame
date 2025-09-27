using UnityEngine;



#region Collector Events

public struct CollectorFinishedEvent
{
    public Collector collector;
    public ColonyTypeEnum colonyType;
    public double amountCollected;
}

public struct CollectorRequestedEvent
{
    public CollectorTypeEnum collectorType;
    public ColonyTypeEnum colonyType;
}

public struct CollectorStartedEvent
{
    public CollectorTypeEnum collectorType;
    public ColonyTypeEnum colonyType;
}

#endregion

#region Collector Upgrade Events
public struct CollectorUpgradeRequestedEvent
{
    public CollectorTypeEnum collectorType;
    public ColonyTypeEnum colonyType;
}

public struct CollectorUpgradeStartedEvent
{
    public CollectorTypeEnum? collectorType;
    public ColonyTypeEnum? colonyType;
    public UpgradeCategory upgradeCategory;
    public UpgradeType upgradeType;
    public double upgradeMultiplier;
}

public struct CollectorUpgradeFinishedEvent
{
    public CollectorTypeEnum collectorType;
    public ColonyTypeEnum colonyType;
}
#endregion

#region Collector Level Amount Events
public struct CollectorLevelAmountRequestedEvent
{
    public int amount;
}

public struct CollectorLevelAmountStartedEvent
{
    public int amount;
}

public struct CollectorLevelAmountFinishedEvent
{
    public int amount;
}
#endregion

#region Collector ProgressBar Events

public struct CollectorProgressEvent
{
    public CollectorTypeEnum collectorType;
    public ColonyTypeEnum colonyType;
    public float progress;
    public float timeRemaining;
}

#endregion


