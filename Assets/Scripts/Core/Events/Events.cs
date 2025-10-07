#region Collector Events Args

public struct ProgressBarUpdateArgs
{
    public Collector Collector;
    public float Value;
    public double RemainingTime;
}

public struct CollectorEventArgs
{
    public Collector Collector;
}

public struct SellButtonEventArgs
{
    public CollectorType CollectorType;
    public ColonyType ColonyType;
    public double MoneyAmount;
    public double ResourceAmount;
}

public struct CollectorUpgradeAmountChangedEventArgs
{
    public int Value;
}

public struct CollectorUpgradeEventArgs
{
    public Collector Collector;
    public Upgrades Upgrade;
}

public struct CostResourceEventArgs
{
    public CostResource CostResource;
    public Collector Collector;
}

public struct ResourceEventArgs
{
    public ResourceSO CostResource;
}

#endregion
























