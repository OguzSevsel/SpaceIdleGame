#region Collector Events

using NUnit.Framework.Constraints;

public struct ProgressBarUpdateEvent
{
    public Collector Collector;
    public float Value;
    public double RemainingTime;
}

public struct CollectorEvent
{
    public CollectorType CollectorType;
    public ColonyType ColonyType;
}

public struct CollectorFinishedEvent
{
    public Collector Collector;
}

public struct CollectorUpgradeAmountChangedEvent
{
    public int Value;
}

public struct CollectorUpgradeEvent
{
    public CollectorType CollectorType;
    public Upgrades Upgrade;
    public ColonyType ColonyType;
}
public struct CollectorUpgradeFinishedEvent
{
    public Collector Collector;
}

#endregion



#region Sell Events

public struct SellButtonClicked
{

}

public struct SellResourceButtonClicked
{

}

public struct SellResourceButtonUpdateEvent
{
    public Collector Collector;
}

public struct SellTabButtonClicked
{

}

#endregion



#region Transport Events

public struct TransportTabButtonClicked
{

}

#endregion



#region Stats Events

public struct StatsTabButtonClicked
{

}

#endregion



#region Upgrades Events

public struct UpgradesTabButtonClicked
{

}

#endregion






















