using UnityEngine;

public enum UpgradeType
{
    CollectorSpeed,
    CollectorLevel,
    CollectorAutoCollect,
    TransportHub,
    Marketplace
}

public interface IUpgradeable
{
#nullable enable
    public void Upgrade(UpgradeType upgrade, CollectorModel collector);
}

public class Upgrade : MonoBehaviour
{
    public UpgradeType upgradeType;




}