using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class CollectorPanel : MonoBehaviour
{
    [Header("Upgrade Elements")]
    [SerializeField] private Button _button_Upgrade;
    [SerializeField] private TextMeshProUGUI _textUpgradeButton;
    [SerializeField] private List<TextMeshProUGUI> _textUpgradeCosts;
    [SerializeField] private TextMeshProUGUI _textUpgradeEffect;
    [SerializeField] private TextMeshProUGUI _textUpgradeLevel;

    [Header("Progress Bar Elements")]
    [SerializeField] private ProgressBar _progressBar;
    [SerializeField] private TextMeshProUGUI _textProgressBarTime;

    [Header("Output Text")]
    [SerializeField] private TextMeshProUGUI _textOutput;
    private Button _buttonCollectorPanel;
    private ColonyPanel ColonyPanel;


    private void Start()
    {
        _buttonCollectorPanel = GetComponent<Button>();
        _buttonCollectorPanel.onClick.AddListener(OnCollectorPanelClicked);
        _button_Upgrade.onClick.AddListener(OnCollectorUpgradeButtonClick);
        ColonyPanel = GetComponentInParent<ColonyPanel>();
    }

    public void UpdateUpgradeAmountUI(int collectAmount)
    {
        _textUpgradeButton.text = $"LVL UP +{collectAmount}";
        EventBus.Publish(new CollectorUpgradeAmountChangedEvent { Value = collectAmount });
    }

    private void UpdateUpgradeUIElements(List<CostResource> costResources, double upgradeEffect, int upgradeLevel)
    {
        for (int i = 0; i < costResources.Count; i++)
        {
            CostResource resource = costResources[i];
            TextMeshProUGUI text = _textUpgradeCosts[i];

            text.text = $"{resource.Resource.resourceType} {resource.Resource.ResourceUnit}";
        }

        _textUpgradeEffect.text = $"{upgradeEffect}";
        _textUpgradeLevel.text = $"LVL {upgradeLevel}";
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
        _textProgressBarTime.text = time.ToString() + " Sec";
    }

    private void UpdateOutputUIElements(double output, string unit)
    {
        _textOutput.text = $"{output} {unit}"; 
    }








    private bool isTypesMatching(CollectorType collectorType, ColonyType colonyType)
    {
        if (colonyType == ColonyPanel.ColonyType && collectorType == GetCollectorType(this.gameObject.name))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnProgressBarUpdated(ProgressBarUpdateEvent @event)
    {
        bool isMatch = isTypesMatching(@event.Collector.CollectorData.CollectorType, @event.Collector.GetColonyType());
        if (isMatch)
        {
            UpdateProgressBarUIElements(@event.Value, (float)@event.RemainingTime);
        }
    }

    private void OnCollectorPanelClicked()
    {
        EventBus.Publish(new CollectorEvent());
    }

    private void OnCollectorUpgradeButtonClick()
    {
        EventBus.Publish(new CollectorUpgradeEvent { CollectorType = GetCollectorType(this.gameObject.name), Upgrade = Upgrades.CollectorLevel, ColonyType = ColonyPanel.ColonyType });
    }

    private void OnCollectorUpgradeFinished(CollectorUpgradeFinishedEvent @event)
    {
        List<CostResource> costResources = @event.Collector.GetCostResources();
        bool isMatch = isTypesMatching(@event.Collector.CollectorData.CollectorType, @event.Collector.GetColonyType());

        if (isMatch)
        {
            if (costResources.Count <= 4 && costResources.Count >= 1)
            {
                UpdateUpgradeUIElements(@event.Collector.CostResources, @event.Collector.GetCollectionRateMultiplier(), @event.Collector.GetLevel());
                UpdateOutputUIElements(@event.Collector.GetCollectionRate(), @event.Collector.CollectorData.GeneratedResource.ResourceUnit);
            }
            else
            {
                Debug.LogWarning($"Please check Cost resources of this collector: {@event.Collector.name}");
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
        EventBus.Unsubscribe<ProgressBarUpdateEvent>(OnProgressBarUpdated);
        EventBus.Unsubscribe<CollectorUpgradeFinishedEvent>(OnCollectorUpgradeFinished);
    }

    private void Subscribe()
    {
        EventBus.Subscribe<ProgressBarUpdateEvent>(OnProgressBarUpdated);
        EventBus.Subscribe<CollectorUpgradeFinishedEvent>(OnCollectorUpgradeFinished);
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
