#region Collector Events

using NUnit.Framework.Constraints;

public struct ProgressBarUpdateArgs
{
    public Collector Collector;
    public float Value;
    public double RemainingTime;
}

public struct CollectorEventArgs
{
    public Collector collector;
}

public struct CollectorFinishedEventArgs
{
    public Collector Collector;
}

public struct CollectorUpgradeAmountChangedEventArgs
{
    public int Value;
}

public struct CollectorUpgradeAmountChangedFinishedEventArgs
{
    public Collector Collector;
}

public struct CollectorUpgradeEventArgs
{
    public Collector Collector;
    public Upgrades Upgrade;
}
public struct CollectorUpgradeFinishedEventArgs
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

public struct SellResourceButtonUpdateEventArgs
{
    public Collector Collector;
}

public struct SellResourceHideEventArgs
{

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


public static class NumberExtensions
{
    public static string ToShortString(this double value)
    {
        if (value < 1000)
            return value.ToString("0.##"); // just strip extra decimals

        string[] suffixes = { "", "k", "M", "B", "T", "Qa", "Qi" };
        int i = 0;
        double shortened = value;

        while (shortened >= 1000 && i < suffixes.Length - 1)
        {
            shortened /= 1000;
            i++;
        }

        return shortened.ToString("0.##") + suffixes[i]; // abbreviated with max 2 decimals
    }

    public static string ToShortString(this float value)
    {
        if (value < 1000f)
            return value.ToString("0.##"); // just strip extra decimals

        string[] suffixes = { "", "k", "M", "B", "T", "Qa", "Qi" };
        int i = 0;
        float shortened = value;

        while (shortened >= 1000f && i < suffixes.Length - 1)
        {
            shortened /= 1000f;
            i++;
        }

        return shortened.ToString("0.##") + suffixes[i]; // abbreviated with max 2 decimals
    }
}




















