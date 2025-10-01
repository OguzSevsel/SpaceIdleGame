using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeMultipliers", menuName = "Scriptable Objects/ Multipliers / UpgradeMultipliers")]
public class UpgradeMultipliers : ScriptableObject
{
    [Range(1f, 2f)] public double CollectorUpgradeAmountMultiplier = 1.05d;
    [Range(1f, 2f)] public double CollectorUpgradeCostMultiplier = 1.05d;
    [Range(0f, 0.95f)] public double CollectorUpgradeSpeedMultiplier = 0.95d;
    [Range(1f, 2f)] public double SellRate = 1.05d;
}
