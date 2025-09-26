using UnityEngine;

public struct CollectorFinishedEvent
{
    public CollectorTypeEnum collectorType;
    public ColonyTypeEnum colonyType;
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

public struct CollectorUpgradeRequestedEvent
{
    public CollectorTypeEnum collectorType;
    public ColonyTypeEnum colonyType;
}

public struct CollectorUpgradeStartedEvent
{
    public CollectorTypeEnum collectorType;
    public ColonyTypeEnum colonyType;
}

public struct CollectorUpgradeFinishedEvent
{
    public CollectorTypeEnum collectorType;
    public ColonyTypeEnum colonyType;
}

public struct  CollectorLevelAmountRequestedEvent
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

public struct CollectorProgressEvent
{
    public CollectorTypeEnum collectorType;
    public ColonyTypeEnum colonyType;
    public float progress;
    public float timeRemaining;
}
