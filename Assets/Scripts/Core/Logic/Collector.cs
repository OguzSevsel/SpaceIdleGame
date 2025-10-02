using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UIElements;

public class Collector : MonoBehaviour, IUpgradeable, ISellable
{
    //public fields
    public CollectorSO CollectorData;
    public List<CostResource> CostResources;

    //private types
    private Colony _colony;

    // private fields
    // Resource Amount Field
    private double _resourceAmount = 0.0;

    // Collecting Bools
    private bool _isCollecting = false;
    private bool _isAutoCollecting = false;

    //Collection Rate Fields
    private double _collectionRate = 1.0;
    private double _baseCollectionRate = 1.0;
    private double _collectionRateMultiplier = 1.05;

    //Collection Speed Fields
    private double _speed = 1.0;
    private double _speedMultiplier = 0.95;
    private double _time = 0.0;

    //Collector Level Fields
    private int _level = 1;
    private int _levelIncrement = 1;

    private void Start()
    {
        _colony = GetComponentInParent<Colony>();
    }

    private void Update()
    {
        if (_isCollecting)
        {
            _time += Time.deltaTime;

            EventBus.Publish(new ProgressBarUpdateEvent
            {
                Value = Mathf.Clamp01((float)(_time / _speed)),
                RemainingTime = _speed - _time,
                Collector = this
            });

            if (_time >= _speed)
            {
                _isCollecting = false;
                _time = 0d;
                AddResource();

                EventBus.Publish(new ProgressBarUpdateEvent
                {
                    Value = 0f,
                    RemainingTime = 0d,
                    Collector = this
                });

                EventBus.Publish(new CollectorFinishedEvent
                {
                    Collector = this
                });
            }
        }
        if (_isAutoCollecting)
        {
            Debug.Log("Collector: Auto-collecting resources...");
            // Simulate auto-collection logic here
            // Keep isAutoCollecting true for continuous auto-collection
        }
    }

    public void Collect()
    {
        if (!_isAutoCollecting)
        {
            _isCollecting = true;
        }
    }

    public void AutoCollect()
    {
        _isCollecting = false;
        _isAutoCollecting = true;
    }

    private void AddResource()
    {
        _resourceAmount += _collectionRate;
    }












    #region Interface Implementations

#nullable enable
    public void Upgrade(Upgrades upgrade, CollectorType collectorType, ColonyType colonyType)
    {
        bool isColonyHasEnoughResource = _colony.CheckIfColonyHasEnoughResources(CostResources);

        if (isColonyHasEnoughResource)
        {
            if (collectorType == this.CollectorData.CollectorType && colonyType == _colony.colonyData.ColonyType)
            {
                switch (upgrade)
                {
                    case Upgrades.CollectorSpeed:
                        _speed *= _speedMultiplier;
                        break;

                    case Upgrades.CollectorLevel:
                        _level += _levelIncrement;
                        _collectionRate = _baseCollectionRate * (_level + _collectionRateMultiplier);
                        _colony.SpendResources(CostResources);
                        IncreaseCost(_level, overrideCostMultiplier: null);
                        break;

                    case Upgrades.CollectorAutoCollect:
                        AutoCollect();
                        break;

                    default:
                        break;
                }
            }
        }
    }

    public void Sell()
    {
        throw new NotImplementedException();
    }

    #endregion

    #region Utility Functions

    private void IncreaseCost(int level, double? overrideCostMultiplier = null)
    {
        foreach (CostResource resource in CostResources)
        {
            resource.AdjustAmountAndMultiplier(level, overrideCostMultiplier: null);
        }
    }

    public void SetSpeed(double? speed = null, double? speedMultiplier = null)
    {
        if (speed.HasValue)
            _speed = speed.Value;

        if (speedMultiplier.HasValue)
            _speedMultiplier = speedMultiplier.Value;
    }

    public void SetCollection(double? baseCollectionRate = null, double? collectionRate = null, double? collectionRateMultiplier = null)
    {
        if (baseCollectionRate.HasValue)
        {
            _baseCollectionRate = baseCollectionRate.Value;
        }

        if (collectionRate.HasValue)
        {
            _collectionRate = collectionRate.Value;
        }

        if (collectionRateMultiplier.HasValue)
        {
            _collectionRateMultiplier = collectionRateMultiplier.Value;
        }
    }

    public void SetResourceAmount(double resourceAmount)
    {
        _resourceAmount = resourceAmount;
    }

    public double GetResourceAmount()
    {
        return _resourceAmount;
    }

    public string GetResourceUnit()
    {
        return CollectorData.GeneratedResource.ResourceUnit;
    }

    public double GetCollectionRateMultiplier()
    {
        return _collectionRateMultiplier;
    }

    public double GetCollectionRate()
    {
        return _collectionRate;
    }

    public List<CostResource> GetCostResources()
    {
        return CostResources;
    }

    public int GetLevel()
    {
        return _level;
    }

    public ColonyType GetColonyType()
    {
        return _colony.colonyData.ColonyType;
    }

    #endregion

    #region Events

    private void OnCollectorButtonClicked(CollectorEvent @event)
    {
        Collect();
    }

    private void OnCollectorUpgradeAmountChanged(CollectorUpgradeAmountChangedEvent @event)
    {
        this._levelIncrement = @event.Value;
    }

    private void OnCollectorUpgrade(CollectorUpgradeEvent @event)
    {
        Upgrade(@event.Upgrade, @event.CollectorType, @event.ColonyType);
        EventBus.Publish(new CollectorUpgradeFinishedEvent { Collector = this });
    }

    private void OnEnable()
    {
        Subscribe();
    }

    private void OnDisable()
    {
        UnSubscribe();
    }

    private void Subscribe()
    {
        EventBus.Subscribe<CollectorEvent>(OnCollectorButtonClicked);
        EventBus.Subscribe<CollectorUpgradeEvent>(OnCollectorUpgrade);
        EventBus.Subscribe<CollectorUpgradeAmountChangedEvent>(OnCollectorUpgradeAmountChanged);
    }

    private void UnSubscribe()
    {
        EventBus.Unsubscribe<CollectorEvent>(OnCollectorButtonClicked);
        EventBus.Unsubscribe<CollectorUpgradeEvent>(OnCollectorUpgrade);
        EventBus.Unsubscribe<CollectorUpgradeAmountChangedEvent>(OnCollectorUpgradeAmountChanged);
    }

    #endregion
}
