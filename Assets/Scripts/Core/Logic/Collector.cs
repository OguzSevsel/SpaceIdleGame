using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
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
    [SerializeField] [Range(0f, 2f)] private double _resourceAmount;
    [SerializeField] [Range(0f, 2f)] private double _sellRate;
    [SerializeField] [Range(0f, 2f)] private double _sellMultiplier;

    // Collecting Bools
    private bool _isCollecting = false;
    private bool _isAutoCollecting = false;
    private bool _isShowingInfo = false;

    //Collection Rate Fields
    [SerializeField] [Range(0f, 2f)] private double _collectionRate;
    [SerializeField] [Range(0f, 2f)] private double _baseCollectionRate;
    [SerializeField] [Range(0f, 2f)] private double _collectionRateMultiplier;

    //Collection Speed Fields
    [SerializeField] [Range(0f, 2f)] private double _speed;
    [SerializeField] [Range(0f, 2f)] private double _speedMultiplier;
    private double _time;

    //Collector Level Fields
    private int _level;
    private int _levelIncrement;


    private void Start()
    {
        _colony = GetComponentInParent<Colony>();
        _level = 1;
        _levelIncrement = 1;
        _time = 0d;
        _resourceAmount = 0d;
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

    private void ShowInfo()
    {
        _isShowingInfo = true;
    }

    private void HideInfo()
    {
        _isShowingInfo = false;
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

    public double GetSellMoneyAmount()
    {
        return _resourceAmount *= _sellRate;
    }

    #endregion

    #region Events

    private void OnCollectorButtonClicked(CollectorEvent @event)
    {
        if (@event.CollectorType == this.CollectorData.CollectorType && @event.ColonyType == _colony.colonyData.ColonyType)
        {
            Collect();
        }
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

    private void OnSellTabButtonClicked(SellTabButtonClicked @event)
    {
        ShowInfo();
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
        EventBus.Subscribe<SellTabButtonClicked>(OnSellTabButtonClicked);
    }

    private void UnSubscribe()
    {
        EventBus.Unsubscribe<CollectorEvent>(OnCollectorButtonClicked);
        EventBus.Unsubscribe<CollectorUpgradeEvent>(OnCollectorUpgrade);
        EventBus.Unsubscribe<CollectorUpgradeAmountChangedEvent>(OnCollectorUpgradeAmountChanged);
        EventBus.Unsubscribe<SellTabButtonClicked>(OnSellTabButtonClicked);
    }

    #endregion
}
