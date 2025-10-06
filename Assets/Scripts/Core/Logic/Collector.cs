using System;
using System.Collections.Generic;
using UnityEngine;

public class Collector : MonoBehaviour, IUpgradeable, ISellable
{
    [Header("Required Types")]
    public CollectorSO CollectorData;
    public List<CostResource> CostResources;

    [Header("Resource Amount Rates")]
    [SerializeField] [Range(0f, 2f)] private double _resourceAmount;
    [SerializeField][Range(0f, 2f)] private double _sellRate;
    [SerializeField][Range(0f, 2f)] private double _sellMultiplier;

    [Header("Collection Rates")]
    [SerializeField][Range(0f, 2f)] private double _collectionRate;
    [SerializeField][Range(0f, 2f)] private double _baseCollectionRate;
    [SerializeField][Range(0f, 2f)] private double _collectionRateMultiplier;

    [Header("Speed Rates")]
    [SerializeField][Range(0f, 2f)] private double _speed;
    [SerializeField][Range(0f, 1f)] private double _speedMultiplier;
    private double _time = 0d;

    [Header("Collector Level Rates")]
    [SerializeField] private int _level = 1;
    [SerializeField] private int _levelIncrement = 1;

    //private types
    private Colony _colony;

    // Collecting Bools
    private bool _isCollecting = false;
    private bool _isAutoCollecting = false;
    private bool _isShowingInfo = false;

    //Events
    public static event Action<ProgressBarUpdateArgs> ProgressBarUpdateEvent;
    public static event Action<CollectorFinishedEventArgs> CollectorFinishedEvent;
    public static event Action<SellResourceButtonUpdateEventArgs> SellResourceButtonUpdateEvent;
    public static event Action<CollectorUpgradeFinishedEventArgs> CollectorUpgradeFinishedEvent;
    public static event Action<CollectorUpgradeAmountChangedFinishedEventArgs> CollectorUpgradeAmountChanged;

    //GUID Fields
    private string _guid;
    public string CollectorGUID => _guid;



    #region Unity Functions

    private void Start()
    {
        _colony = GetComponentInParent<Colony>();
        AutoCollect();
    }

    private void Update()
    {
        if (_isCollecting)
        {
            _time += Time.deltaTime;

            ProgressBarUpdateEvent?.Invoke(new ProgressBarUpdateArgs
            {
                Collector = this,
                Value = Mathf.Clamp01((float)(_time / _speed)),
                RemainingTime = _speed - _time
            });


            if (_time >= _speed)
            {
                _isCollecting = false;
                _time = 0d;
                AddResource();

                ProgressBarUpdateEvent?.Invoke(new ProgressBarUpdateArgs
                {
                    Collector = this,
                    Value = 0f,
                    RemainingTime = 0d
                });

                CollectorFinishedEvent?.Invoke(new CollectorFinishedEventArgs
                {
                    Collector = this
                });

            }
        }
        if (_isAutoCollecting)
        {
            _time += Time.deltaTime;

            ProgressBarUpdateEvent?.Invoke(new ProgressBarUpdateArgs
            {
                Collector = this,
                Value = Mathf.Clamp01((float)(_time / _speed)),
                RemainingTime = _speed - _time
            });

            if (_time >= _speed)
            {
                _time = 0d;
                AddResource();

                ProgressBarUpdateEvent?.Invoke(new ProgressBarUpdateArgs
                {
                    Collector = this,
                    Value = 0f,
                    RemainingTime = 0d
                });

                CollectorFinishedEvent?.Invoke(new CollectorFinishedEventArgs
                {
                    Collector = this
                });
            }
        }

        if (_isShowingInfo)
        {
            SellResourceButtonUpdateEvent?.Invoke(new SellResourceButtonUpdateEventArgs { Collector = this });
        }
    }

    #endregion

    #region Logic Functions

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

    #endregion

    #region Interface Implementations

#nullable enable
    public void Upgrade(Upgrades upgrade, Collector collector)
    {
        bool isColonyHasEnoughResource = _colony.CheckIfColonyHasEnoughResources(CostResources);

        if (isColonyHasEnoughResource)
        {
            if (this == collector)
            {
                switch (upgrade)
                {
                    case Upgrades.CollectorSpeed:
                        _speed *= _speedMultiplier;
                        break;

                    case Upgrades.CollectorLevel:
                        _level += _levelIncrement;
                        _collectionRate = _baseCollectionRate * (_level * _collectionRateMultiplier);
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

    private void IncreaseCost(int level, double? overrideCostMultiplier = null, int? increaseLevelIncrement = null)
    {
        if (increaseLevelIncrement.HasValue)
        {
            foreach (CostResource resource in CostResources)
            {
                resource.AdjustAmount(increaseLevelIncrement.Value, level);
            }
        }

        if (overrideCostMultiplier.HasValue)
        {
            foreach (CostResource resource in CostResources)
            {
                resource.AdjustAmountAndMultiplier(level, overrideCostMultiplier.Value);
            }
        }

        if (!increaseLevelIncrement.HasValue && !overrideCostMultiplier.HasValue)
        {
            foreach (CostResource resource in CostResources)
            {
                resource.AdjustAmountAndMultiplier(level, overrideCostMultiplier: null);
            }
        }
    }

    #endregion

    #region Setters

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

    #endregion

    #region Getters

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

    //This one will be called after player starts Collector
    private void OnCollectorButtonClicked(CollectorEventArgs @event)
    {
        if (@event.collector == this)
        {
            Collect();
        }
    }

    //This one will be called after player clicks upgrade amount buttons (x1, x5, x10, x100)
    private void OnCollectorUpgradeAmountChanged(CollectorUpgradeAmountChangedEventArgs @event)
    {
        this._levelIncrement = @event.Value;
        IncreaseCost(_level, overrideCostMultiplier: null, increaseLevelIncrement: @event.Value);
        CollectorUpgradeAmountChanged?.Invoke(new CollectorUpgradeAmountChangedFinishedEventArgs { Collector = this});
    }

    //This one will be called after player clicks collector upgrade button
    private void OnCollectorUpgrade(CollectorUpgradeEventArgs @event)
    {
        Upgrade(@event.Upgrade, @event.Collector);
        CollectorUpgradeFinishedEvent?.Invoke(new CollectorUpgradeFinishedEventArgs { Collector = this});
    }

    //This one will be called after player clicks Sell Tab Button
    private void OnSellResourceShow()
    {
        ShowInfo();
    }

    //This one will be called after player clicks any tab button except Sell Tab Button
    private void OnSellResourceHide()
    {
        HideInfo();
    }

    private void OnEnable()
    {
        if (string.IsNullOrEmpty(_guid))
            _guid = System.Guid.NewGuid().ToString();

        Subscribe();
    }

    private void OnDisable()
    {
        UnSubscribe();
    }

    private void Subscribe()
    {
        CollectorPanel.CollectorButtonClickEvent += OnCollectorButtonClicked;
        TabGroup.SellResourceHideEvent += OnSellResourceHide;
        TabGroup.SellResourceShowEvent += OnSellResourceShow;
        CollectorPanel.CollectorUpgradeButtonClickEvent += OnCollectorUpgrade;
        CollectorPanel.CollectorUpgradeAmountChanged += OnCollectorUpgradeAmountChanged;
    }

    private void UnSubscribe()
    {
        CollectorPanel.CollectorButtonClickEvent -= OnCollectorButtonClicked;
        TabGroup.SellResourceHideEvent -= OnSellResourceHide;
        TabGroup.SellResourceShowEvent -= OnSellResourceShow;
        CollectorPanel.CollectorUpgradeButtonClickEvent -= OnCollectorUpgrade;
        CollectorPanel.CollectorUpgradeAmountChanged -= OnCollectorUpgradeAmountChanged;
    }

    #endregion
}
