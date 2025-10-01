using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Collector : MonoBehaviour, IUpgradeable
{
    //Types
    public CollectorType CollectorType;
    private Colony _colony;

    //Bools
    private bool _isCollecting;
    private bool _isAutoCollecting;

    //Trackers
    private float _collectionTimer;
    public double ResourceAmount;
    public double SellMoneyAmount;


    private void Start()
    {
        _colony = GetComponentInParent<Colony>();
    }

    void Update()
    {
        if (_isCollecting)
        {
            _collectionTimer += Time.deltaTime;

            EventBus.Publish(new CollectorProgressEvent() 
            { 
                CollectorType = CollectorType.CollectorTypeName, 
                Progress = Mathf.Clamp01(_collectionTimer /(float)CollectorType.Speed), 
                TimeRemaining = (float)CollectorType.Speed - _collectionTimer 
            });

            if (_collectionTimer >= CollectorType.Speed)
            {
                _collectionTimer = 0f;
                AddAmount(CollectorType.CollectionRate);
                _isCollecting = false;
                EventBus.Publish(new CollectorFinishedEvent()
                {
                    Collector = this,
                    ColonyType = _colony.ColonyType.ColonyTypeName
                });

                EventBus.Publish(new CollectorProgressEvent() 
                { 
                    CollectorType = CollectorType.CollectorTypeName,
                    ColonyType = _colony.ColonyType.ColonyTypeName,
                    Progress = Mathf.Clamp01(0f), 
                    TimeRemaining = 0f
                });
            }
        }

        if (_isAutoCollecting)
        {
            _collectionTimer += Time.deltaTime;

            EventBus.Publish(new CollectorProgressEvent()
            {
                CollectorType = CollectorType.CollectorTypeName,
                Progress = Mathf.Clamp01(_collectionTimer / (float)CollectorType.Speed),
                TimeRemaining = (float)CollectorType.Speed - _collectionTimer
            });

            if (_collectionTimer >= CollectorType.Speed)
            {
                _collectionTimer = 0f;
                AddAmount(CollectorType.CollectionRate);
                EventBus.Publish(new CollectorFinishedEvent()
                {
                    Collector = this,
                    ColonyType = _colony.ColonyType.ColonyTypeName
                });

                EventBus.Publish(new CollectorProgressEvent()
                {
                    CollectorType = CollectorType.CollectorTypeName,
                    ColonyType = _colony.ColonyType.ColonyTypeName,
                    Progress = Mathf.Clamp01(0f),
                    TimeRemaining = 0f
                });
            }
        }
    }


    

    

    public override string ToString()
    {
        return $"{ResourceAmount} {CollectorType.GeneratedResource.Unit}";
    }

    public void AddAmount(double amountToAdd)
    {
        ResourceAmount += amountToAdd;
        SellMoneyAmount = ResourceAmount * _colony.Multiplier.SellRate;
    }


    public void Collect()
    {
        _isCollecting = true;
    }
    public void AutoCollect()
    {
        _isAutoCollecting = true;
    }


    public void Upgrade(UpgradeCategory upgradeCategory, UpgradeType upgradeType, ColonyTypeEnum? colonyType, CollectorTypeEnum? collectorType, double upgradeMultiplier)
    {
        foreach (CostResource resource in this.CollectorType.CostResourcesToUpgrade)
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
                            this.CollectorType.Level = this.CollectorType.Level + this.CollectorType.LevelAmount;
                            this.CollectorType.CollectionRate = CollectorType.BaseProduction * CollectorType.Level;
                            IncreaseUpgradeCost(resource, _colony.Multiplier.CollectorUpgradeCostMultiplier);
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
        Upgrade(e.UpgradeCategory, e.UpgradeType, e.ColonyType, e.CollectorType, e.UpgradeMultiplier);
    }


    private void OnEnable()
    {
        EventBus.Subscribe<CollectorUpgradeStartedEvent>(OnCollectorUpgradeStarted);
        EventBus.Subscribe<CollectorLevelAmount>(OnCollectorLevelAmount);
    }

    private void OnCollectorLevelAmount(CollectorLevelAmount e)
    {
        this.CollectorType.LevelAmount = e.Amount;
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<CollectorUpgradeStartedEvent>(OnCollectorUpgradeStarted);
    }
}
