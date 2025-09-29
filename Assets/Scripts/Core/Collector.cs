using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Collector : MonoBehaviour, IUpgradeable
{
    public CollectorType CollectorType;
    private float collectionTimer;
    private bool isCollecting;
    private bool isAutoCollecting;
    public double resourceAmount;
    private Colony _colony;
    private double upgradeCostMultiplier = 1.1d;

    private void Start()
    {
        _colony = GetComponentInParent<Colony>();
    }
    void Update()
    {
        if (isCollecting)
        {
            collectionTimer += Time.deltaTime;

            EventBus.Publish(new CollectorProgressEvent() 
            { 
                collectorType = CollectorType.CollectorTypeName, 
                progress = Mathf.Clamp01(collectionTimer /(float)CollectorType.Speed), 
                timeRemaining = (float)CollectorType.Speed - collectionTimer 
            });

            if (collectionTimer >= CollectorType.Speed)
            {
                collectionTimer = 0f;
                AddAmount(CollectorType.CollectionRate);
                isCollecting = false;
                EventBus.Publish(new CollectorFinishedEvent()
                {
                    collector = this,
                    colonyType = _colony.ColonyType.ColonyTypeName
                });

                EventBus.Publish(new CollectorProgressEvent() 
                { 
                    collectorType = CollectorType.CollectorTypeName,
                    colonyType = _colony.ColonyType.ColonyTypeName,
                    progress = Mathf.Clamp01(0f), 
                    timeRemaining = 0f
                });
            }
        }

        if (isAutoCollecting)
        {
            collectionTimer += Time.deltaTime;

            EventBus.Publish(new CollectorProgressEvent()
            {
                collectorType = CollectorType.CollectorTypeName,
                progress = Mathf.Clamp01(collectionTimer / (float)CollectorType.Speed),
                timeRemaining = (float)CollectorType.Speed - collectionTimer
            });

            if (collectionTimer >= CollectorType.Speed)
            {
                collectionTimer = 0f;
                AddAmount(CollectorType.CollectionRate);
                EventBus.Publish(new CollectorFinishedEvent()
                {
                    collector = this,
                    colonyType = _colony.ColonyType.ColonyTypeName
                });

                EventBus.Publish(new CollectorProgressEvent()
                {
                    collectorType = CollectorType.CollectorTypeName,
                    colonyType = _colony.ColonyType.ColonyTypeName,
                    progress = Mathf.Clamp01(0f),
                    timeRemaining = 0f
                });
            }
        }
    }


    

    

    public override string ToString()
    {
        return $"{resourceAmount} {CollectorType.generatedResource.Unit}";
    }

    public void AddAmount(double amountToAdd)
    {
        resourceAmount += amountToAdd;
    }


    public void Collect()
    {
        isCollecting = true;
    }
    public void AutoCollect()
    {
        isAutoCollecting = true;
    }


    public void Upgrade(UpgradeCategory upgradeCategory, UpgradeType upgradeType, ColonyTypeEnum? colonyType, CollectorTypeEnum? collectorType, double upgradeMultiplier)
    {
        foreach (CostResource resource in this.CollectorType.costResourcesToUpgrade)
        {
            bool isUpgradeable = _colony.CheckIfColonyHasEnoughResourcesForUpgrade(resource, resource.amount);

            if (isUpgradeable)
            {
                if (upgradeCategory == UpgradeCategory.Local && colonyType == _colony.ColonyType.ColonyTypeName && collectorType == CollectorType.CollectorTypeName)
                {
                    switch (upgradeType)
                    {
                        case UpgradeType.CollectorSpeed:
                            this.CollectorType.Speed /= upgradeMultiplier;
                            break;
                        case UpgradeType.CollectorAmount:
                            this.CollectorType.Level++;
                            this.CollectorType.CollectionRate = CollectorType.BaseProduction * CollectorType.Level;
                            IncreaseUpgradeCost(resource, upgradeCostMultiplier);
                            break;
                        case UpgradeType.AutoCollect:
                            AutoCollect();
                            break;
                        case UpgradeType.CostReduction:
                            break;
                    }
                }
            }
        }
    }

    private void IncreaseUpgradeCost(CostResource resource, double costMultiplier)
    {
        resource.amount *= resource.BaseAmount * Math.Pow(costMultiplier, this.CollectorType.Level);
        Debug.Log($"New upgrade cost for {CollectorType.CollectorTypeName} is {resource.resourceType} {resource.amount}");
    }


    private void OnCollectorUpgradeStarted(CollectorUpgradeStartedEvent e)
    {
        Upgrade(e.upgradeCategory, e.upgradeType, e.colonyType, e.collectorType, e.upgradeMultiplier);
    }


    private void OnEnable()
    {
        EventBus.Subscribe<CollectorUpgradeStartedEvent>(OnCollectorUpgradeStarted);
    }
    private void OnDisable()
    {
        EventBus.Unsubscribe<CollectorUpgradeStartedEvent>(OnCollectorUpgradeStarted);
    }
}
