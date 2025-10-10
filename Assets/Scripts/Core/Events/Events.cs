#region Collector Events Args

public struct ProgressBarUpdateArgs
{
    public CollectorModel CollectorModel;
    public float Value;
    public double RemainingTime;
}

public struct CollectorEventArgs
{
    public CollectorModel Collector;
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
    public CollectorModel Collector;
    public UpgradeType Upgrade;
}

public struct CostResourceEventArgs
{
    public CostResource CostResource;
    public CollectorModel Collector;
}

public struct ResourceEventArgs
{
    public ResourceSO CostResource;
}

#endregion
























