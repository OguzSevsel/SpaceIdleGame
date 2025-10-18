using System;
using UnityEngine;

[System.Serializable]
public class CostResource
{
    public ResourceSO Resource;
    [SerializeField] private double _costAmount;
    [SerializeField] private double _baseCostAmount;
    [SerializeField] private double _costMultiplier;

    public void AdjustAmountAndMultiplier(int level, int levelIncrement, double? overrideCostMultiplier = null)
    {
        if (overrideCostMultiplier != null)
        {
            _costMultiplier = (double)overrideCostMultiplier;
        }

        if (levelIncrement != 1)
        {
            _costAmount = GetTotalCost(level, levelIncrement);
        }
        else
        {
            _costAmount = GetNextCost(level);
        }
    }

    public double GetNextCost(int level)
    {
        return _baseCostAmount * Math.Pow(_costMultiplier, level);
    }

    public double GetTotalCost(int level, int levelIncrement)
    {
        return _baseCostAmount * (Math.Pow(_costMultiplier, level) * (Math.Pow(_costMultiplier, levelIncrement) - 1)) / (_costMultiplier - 1);
    }

    public void AdjustAmount(int levelIncrement, int currentLevel)
    {
        double totalCost = 0d;

        switch (levelIncrement)
        {
            case 1:
                totalCost = GetTotalCost(currentLevel, levelIncrement);
                break;
            case 5:
                totalCost = GetTotalCost(currentLevel, levelIncrement);
                break;
            case 10:
                totalCost = GetTotalCost(currentLevel, levelIncrement);
                break;
            case 100:
                totalCost = GetTotalCost(currentLevel, levelIncrement);
                break;
            default:
                break;
        }

        _costAmount = totalCost;
    }

    public double GetCostAmount() { return _costAmount; }
    public double GetBaseCostAmount() { return _baseCostAmount; }
    public double GetCostMultiplier() { return _costMultiplier; }

    public void SetCostAmount(double amount) { _costAmount = amount; }
    public void SetBaseCostAmount(double amount) { _baseCostAmount = amount; }
    public void SetCostMultiplier(double multiplier) { _costMultiplier = multiplier; }
}