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
    private Button _buttonCollectorPanel;
    private ColonyPanel ColonyPanel;

    public static event Action<CollectorEventArgs> CollectorButtonClickEvent;
    public static event Action<CollectorUpgradeEventArgs> CollectorUpgradeButtonClickEvent;
    public static event Action<CollectorUpgradeAmountChangedEventArgs> CollectorUpgradeAmountChanged;

    private void Start()
    {
        Initialize();
    }

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
            //Subscribe();
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

    public void UpdateUpgradeAmountUI(int collectAmount)
    {
        _textUpgradeButton.text = $"LVL UP +{collectAmount}";
        CollectorUpgradeAmountChanged?.Invoke(new CollectorUpgradeAmountChangedEventArgs { Value = collectAmount });
    }

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

    private void UpdateOutputUIElements(double output, string unit)
    {
        _textOutput.text = $"{output.ToShortString()} {unit}"; 
    }

    private void OnProgressBarUpdated(ProgressBarUpdateArgs @event)
    {
        if (@event.Collector == this._collector)
        {
            UpdateProgressBarUIElements(@event.Value, (float)@event.RemainingTime);
        }
    }

    private void OnCollectorPanelClicked()
    {
        CollectorButtonClickEvent?.Invoke(new CollectorEventArgs { collector = _collector});
    }

    private void OnCollectorUpgradeButtonClick()
    {
        CollectorUpgradeButtonClickEvent?.Invoke(new CollectorUpgradeEventArgs
        {
            Collector = _collector,
            Upgrade = Upgrades.CollectorLevel,
        });
    }

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
    }

    private void Subscribe()
    {
        Collector.ProgressBarUpdateEvent += OnProgressBarUpdated;
        Collector.CollectorUpgradeFinishedEvent += OnCollectorUpgradeFinished;
    }

    private CollectorType GetCollectorType(string name)
    {
        CollectorType collectorType = new CollectorType();

        switch (name)
        {
            case "1_Panel_Industry_Energy":
                collectorType = CollectorType.EnergyCollector;
                break;
            case "2_Panel_Industry_Iron":
                collectorType = CollectorType.IronCollector;
                break;
            case "3_Panel_Industry_Copper":
                collectorType = CollectorType.CopperCollector;
                break;
            case "4_Panel_Industry_Silicon":
                collectorType = CollectorType.SiliconCollector;
                break;
            case "5_Panel_Industry_LimeStone":
                collectorType = CollectorType.LimeStoneCollector;
                break;
            case "6_Panel_Industry_Gold":
                collectorType = CollectorType.GoldCollector;
                break;
            case "7_Panel_Industry_Aluminum":
                collectorType = CollectorType.AluminumCollector;
                break;
            case "8_Panel_Industry_Carbon":
                collectorType = CollectorType.CarbonCollector;
                break;
            case "9_Panel_Industry_Diamond":
                collectorType = CollectorType.DiamondCollector;
                break;
            default:
                break;
        }
        return collectorType;
    }

}
