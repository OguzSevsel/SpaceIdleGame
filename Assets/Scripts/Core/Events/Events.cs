public struct ProgressBarUpdateEvent
{
    public Collector Collector;
    public float Value;
    public double RemainingTime;
}

public struct CollectorEvent
{

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