#region Collector Events Args

using System;

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

public struct UpgradeEventArgs
{
    public UpgradeModel UpgradeModel;
    public string TargetId;
}

public struct InfoUpgradeEventArgs
{
    public CostResource Resource;
    public bool IsEnoughResource;
    public string TargetId;
}

//public class Deneme
//{
//    delegate void MessageDelegate(string message);

//    static void SayHello(string message)
//    {
//        Console.WriteLine("Hello, " + message);
//    }

//    static void SayGoodbye(string message)
//    {
//        Console.WriteLine("Goodbye, " + message);
//    }

//    static void Main()
//    {
//        MessageDelegate greet = SayHello;
//        MessageDelegate farewell = SayGoodbye;

//        greet("O?uz");
//        farewell("O?uz");

//        MessageDelegate combo = greet + farewell;
//        combo("everyone");
//    }

//}

#endregion
























