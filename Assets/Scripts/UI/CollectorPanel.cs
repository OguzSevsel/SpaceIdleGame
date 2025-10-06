using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using Unity.Properties;
using System.Linq;

public class CollectorPanel : MonoBehaviour
{
    [Header("Related Type")]
    [SerializeField] private Collector _collector;

    [Header("Collector UI Elements")]
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _level;
    [SerializeField] private TextMeshProUGUI _upgradeEffect;

    [Header("Upgrade Elements")]
    [SerializeField] private Button _button_Upgrade;
    [SerializeField] private TextMeshProUGUI _textUpgradeButton;
    [SerializeField] private List<GameObject> _upgradeCosts;

    [Header("Progress Bar Elements")]
    [SerializeField] private ProgressBar _progressBar;
    [SerializeField] private TextMeshProUGUI _textProgressBarTime;

    [Header("Output Text")]
    [SerializeField] private TextMeshProUGUI _textOutput;

    //Private fields
    private Button _buttonCollectorPanel;
    private ColonyPanel ColonyPanel;

    //Events
    public static event Action<CollectorEventArgs> CollectorButtonClickEvent;
    public static event Action<CollectorUpgradeEventArgs> CollectorUpgradeButtonClickEvent;
    public static event Action<CollectorUpgradeAmountChangedEventArgs> CollectorUpgradeAmountChanged;



    #region Unity Functions

    private void Start()
    {
        Initialize();
    }

    #endregion

    #region Initialization Functions

    private void Initialize()
    {
        _buttonCollectorPanel = GetComponent<Button>();
        _buttonCollectorPanel.onClick.AddListener(OnCollectorPanelClicked);
        _button_Upgrade.onClick.AddListener(OnCollectorUpgradeButtonClick);
        ColonyPanel = GetComponentInParent<ColonyPanel>();

        if (_collector == null)
        {
            Debug.LogError($"{this.gameObject.name} needs a reference to a Collector");
        }
        else
        {
            InitializeUI();
        }
    }

    private void InitializeUI()
    {
        _iconImage.sprite = _collector.CollectorData.GeneratedResource.ResourceIcon;
        double upgradeEffect = _collector.GetCollectionRateMultiplier();
        _upgradeEffect.text = $"x{upgradeEffect.ToShortString()}";
        _textUpgradeButton.text = "LVL UP +1";
        _name.text = $"{_collector.CollectorData.CollectorName}";
        _level.text = $"LVL {_collector.GetLevel()}";
        _textOutput.text = $"{_collector.GetCollectionRate().ToShortString()} {_collector.CollectorData.GeneratedResource.ResourceUnit}";

        List<GameObject> costResources = GetCostResources(number: _collector.CostResources.Count);

        foreach (GameObject resource in costResources)
        {
            int index = costResources.IndexOf(resource);

            TextMeshProUGUI costResourceText = resource.GetComponentInChildren<TextMeshProUGUI>();
            Image icon = resource.GetComponentInChildren<Image>();

            costResourceText.text = $"{_collector.CostResources[index].GetCostAmount().ToShortString()} {_collector.CostResources[index].Resource.ResourceUnit}";
            icon.sprite = _collector.CostResources[index].Resource.ResourceIcon;
        }
    }

    #endregion

    #region Utility Functions

    private List<GameObject> GetCostResources(int? number = null)
    {
        List<GameObject> upgradeCosts = new List<GameObject>();

        if (number.HasValue)
        {
            if (number.Value < _upgradeCosts.Count())
            {
                foreach (GameObject resource in _upgradeCosts)
                {
                    if (_upgradeCosts.IndexOf(resource) < number.Value)
                    {
                        upgradeCosts.Add(resource);
                    }
                    else
                    {
                        resource.SetActive(false);
                    }
                }
            }
            else
            {
                upgradeCosts = _upgradeCosts.Take(number.Value).ToList();
            }
        }
        else
        {
            upgradeCosts = _upgradeCosts.Take(_collector.CostResources.Count).ToList();
        }

        return upgradeCosts;
    }

    #endregion

    #region Update UI Functions

    //Update Upgrade Button Text
    public void UpdateUpgradeAmountUI(int collectAmount)
    {
        _textUpgradeButton.text = $"LVL UP +{collectAmount}";
        CollectorUpgradeAmountChanged?.Invoke(new CollectorUpgradeAmountChangedEventArgs { Value = collectAmount });
    }

    //Update Upgrade Effect, Cost Resources Texts, Level Text
    private void UpdateUpgradeUIElements(List<CostResource> costResources, double upgradeEffect, int upgradeLevel)
    {
        for (int i = 0; i < costResources.Count; i++)
        {
            CostResource resource = costResources[i];
            TextMeshProUGUI text = _upgradeCosts[i].GetComponentInChildren<TextMeshProUGUI>();

            text.text = $"{resource.GetCostAmount().ToShortString()} {resource.Resource.ResourceUnit}";
        }

        _upgradeEffect.text = $"x{upgradeEffect.ToShortString()}";
        _level.text = $"LVL {upgradeLevel}";
    }

    //Update Progress Bar UI
    private void UpdateProgressBarUIElements(float value, float time)
    {
        if (value < 0f)
        {
            value = 0f;
        }

        if (time < 0f)
        {
            time = 0f;
        }

        _progressBar.progressValue = value;
        _textProgressBarTime.text = time.ToShortString() + " Sec";
    }

    //Update Collector Output Text
    private void UpdateOutputUIElements(double output, string unit)
    {
        _textOutput.text = $"{output.ToShortString()} {unit}";
    }

    #endregion

    #region Events

    //Progress Bar Update Event
    private void OnProgressBarUpdated(ProgressBarUpdateArgs @event)
    {
        if (@event.Collector == this._collector)
        {
            UpdateProgressBarUIElements(@event.Value, (float)@event.RemainingTime);
        }
    }

    //This one will be called when player starts collector.
    private void OnCollectorPanelClicked()
    {
        CollectorButtonClickEvent?.Invoke(new CollectorEventArgs { collector = _collector });
    }

    //This one will be called when player clicks upgrade button on the collector.
    private void OnCollectorUpgradeButtonClick()
    {
        CollectorUpgradeButtonClickEvent?.Invoke(new CollectorUpgradeEventArgs
        {
            Collector = _collector,
            Upgrade = Upgrades.CollectorLevel,
        });
    }

    //This one will be called after collector upgraded.
    private void OnCollectorUpgradeFinished(CollectorUpgradeFinishedEventArgs @event)
    {
        List<CostResource> costResources = @event.Collector.GetCostResources();

        if (@event.Collector == _collector)
        {
            if (costResources.Count <= 4 && costResources.Count >= 1)
            {
                UpdateUpgradeUIElements(@event.Collector.CostResources, @event.Collector.GetCollectionRateMultiplier(), @event.Collector.GetLevel());
                UpdateOutputUIElements(@event.Collector.GetCollectionRate(), @event.Collector.CollectorData.GeneratedResource.ResourceUnit);
            }
            else
            {
                Debug.LogWarning($"Please check Cost resources of this collector: {@event.Collector.gameObject.name}");
            }
        }
    }

    private void OnEnable()
    {
        Subscribe();
    }

    private void OnDisable()
    {
        UnSubscribe();
    }

    private void UnSubscribe()
    {
        Collector.ProgressBarUpdateEvent -= OnProgressBarUpdated;
        Collector.CollectorUpgradeFinishedEvent -= OnCollectorUpgradeFinished;
        Collector.CollectorUpgradeAmountChanged -= OnCollectorUpgradeAmountChanged;
    }

    private void Subscribe()
    {
        Collector.ProgressBarUpdateEvent += OnProgressBarUpdated;
        Collector.CollectorUpgradeFinishedEvent += OnCollectorUpgradeFinished;
        Collector.CollectorUpgradeAmountChanged += OnCollectorUpgradeAmountChanged;
    }

    private void OnCollectorUpgradeAmountChanged(CollectorUpgradeAmountChangedFinishedEventArgs @event)
    {
        UpdateUpgradeUIElements(@event.Collector.CostResources, @event.Collector.GetCollectionRateMultiplier(), @event.Collector.GetLevel());
    }

    #endregion
}
