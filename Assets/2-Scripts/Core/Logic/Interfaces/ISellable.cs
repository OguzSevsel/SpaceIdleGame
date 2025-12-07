using UnityEngine;

public interface ISellable
{
    public void Sell(ColonyType colonyType, CollectorType collectorType, double resourceAmount, double moneyAmount);
}
