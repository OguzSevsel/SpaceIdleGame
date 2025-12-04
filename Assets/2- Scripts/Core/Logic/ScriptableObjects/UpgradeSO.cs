using System.Collections.Generic;
using UnityEngine;

public enum UpgradeType
{
    Speed,
    Level,
    AutoCollect
}

public abstract class UpgradeSO : ScriptableObject
{
    public UpgradeType upgradeType;
}



