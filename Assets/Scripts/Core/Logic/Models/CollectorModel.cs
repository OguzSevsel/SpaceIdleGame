using System;
using UnityEngine;

public class CollectorModel : MonoBehaviour, IUpgradeable, ISellable
{
    [Header("Required Types")]
    public CollectorData Data;
    private double _elapsedTime = 0d;

    //private types
    private ColonyModel _colony;

    // Collecting Bools
    private bool _isCollecting = false;
    private bool _isAutoCollecting = false;

    //Events
    public event Action<ProgressBarUpdateArgs> OnProgressBarUpdate;
    public event Action<CollectorEventArgs> OnCollectorFinished;
    public event Action<CollectorEventArgs> OnCollectorUpgrade;
    public event Action<CollectorEventArgs> OnCollectorUpgradeAmount;

    //GUID Fields
    private string _guid;
    public string CollectorGUID => _guid;



    #region Unity Functions

    private void Start()
    {
        _colony = GetComponentInParent<ColonyModel>();
        //AutoCollect();
    }

    private void Update()
    {
        if (_isCollecting)
        {
            _elapsedTime += Time.deltaTime;

            OnProgressBarUpdate?.Invoke(new ProgressBarUpdateArgs
            {
                CollectorModel = this,
                Value = Mathf.Clamp01((float)(_elapsedTime / Data.GetSpeed())),
                RemainingTime = Data.GetSpeed() - _elapsedTime
            });


            if (_elapsedTime >= Data.GetSpeed())
            {
                _isCollecting = false;
                _elapsedTime = 0d;
                AddResource(Data.GetCollectionRate());

                OnProgressBarUpdate?.Invoke(new ProgressBarUpdateArgs
                {
                    CollectorModel = this,
                    Value = 0f,
                    RemainingTime = 0d
                });

                OnCollectorFinished?.Invoke(new CollectorEventArgs
                {
                    Collector = this
                });
            }
        }
        if (_isAutoCollecting)
        {
            _elapsedTime += Time.deltaTime;

            OnProgressBarUpdate?.Invoke(new ProgressBarUpdateArgs
            {
                CollectorModel = this,
                Value = Mathf.Clamp01((float)(_elapsedTime / Data.GetSpeed())),
                RemainingTime = Data.GetSpeed() - _elapsedTime
            });

            if (_elapsedTime >= Data.GetSpeed())
            {
                _elapsedTime = 0d;
                AddResource(Data.GetCollectionRate());

                OnProgressBarUpdate?.Invoke(new ProgressBarUpdateArgs
                {
                    CollectorModel = this,
                    Value = 0f,
                    RemainingTime = 0d
                });

                OnCollectorFinished?.Invoke(new CollectorEventArgs
                {
                    Collector = this
                });
            }
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

    private void AddResource(double amount)
    {
        _colony.AddResource(Data.DataSO.GeneratedResource, amount);
    }

    #endregion

    #region Interface Implementations

#nullable enable
    public void Upgrade(UpgradeType upgrade, CollectorModel collector)
    {
        bool isColonyHasEnoughResource = _colony.CheckResources(Data.CostResources);

        if (isColonyHasEnoughResource)
        {
            if (this == collector)
            {
                switch (upgrade)
                {
                    case UpgradeType.CollectorSpeed:
                        Data.SetSpeed(Data.GetSpeed() * Data.GetSpeedMultiplier());
                        break;

                    case UpgradeType.CollectorLevel:
                        Data.SetLevel(Data.GetLevel() + Data.GetLevelIncrement());
                        Data.SetCollectionRate(Data.DataSO._baseCollectionRate * Math.Pow(Data.GetCollectionRateMultiplier(), Data.GetLevel()));

                        _colony.SpendResources(Data.CostResources);
                        IncreaseCost(Data.GetLevel(), overrideCostMultiplier: null);
                        break;

                    case UpgradeType.CollectorAutoCollect:
                        AutoCollect();
                        break;

                    default:
                        break;
                }
                OnCollectorUpgrade?.Invoke(new CollectorEventArgs { Collector = this });
            }
        }
    }

    public void Sell(ColonyType colonyType, CollectorType collectorType, double resourceAmount, double moneyAmount)
    {
        if (colonyType == _colony.colonyData.ColonyType && collectorType == Data.DataSO.CollectorType)
        {
            //if (Data.GetResourceAmount() >= resourceAmount)
            //{
            //    Data.SetResourceAmount(Data.GetResourceAmount() - resourceAmount);
            //    GlobalResourceManager.Instance.AddMoney(moneyAmount);
            //    OnSold?.Invoke(new CollectorEventArgs { Collector = this });
            //}
        }
    }

    #endregion

    #region Utility Functions

    private void IncreaseCost(int level, double? overrideCostMultiplier = null, int? increaseLevelIncrement = null)
    {
        if (increaseLevelIncrement.HasValue)
        {
            foreach (CostResource resource in Data.CostResources)
            {
                resource.AdjustAmount(increaseLevelIncrement.Value, level);
            }
        }

        if (overrideCostMultiplier.HasValue)
        {
            foreach (CostResource resource in Data.CostResources)
            {
                resource.AdjustAmountAndMultiplier(level, Data.GetLevelIncrement(), overrideCostMultiplier.Value);
            }
        }

        if (!increaseLevelIncrement.HasValue && !overrideCostMultiplier.HasValue)
        {
            foreach (CostResource resource in Data.CostResources)
            {
                resource.AdjustAmountAndMultiplier(level, Data.GetLevelIncrement(), overrideCostMultiplier: null);
            }
        }

        OnCollectorUpgradeAmount?.Invoke(new CollectorEventArgs { Collector = this });
    }

    public void ChangeUpgradeAmount(int levelIncrement)
    {
        Data.SetLevelIncrement(levelIncrement);
        IncreaseCost(Data.GetLevel(), overrideCostMultiplier: null, increaseLevelIncrement: levelIncrement);
    }

    #endregion

    #region Getters

    public ColonyType GetColonyType()
    {
        return _colony.colonyData.ColonyType;
    }

    #endregion

    #region Events

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
    }

    private void UnSubscribe()
    {
    }

    #endregion
}
