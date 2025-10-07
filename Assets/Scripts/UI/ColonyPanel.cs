using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

public class ColonyPanel : MonoBehaviour
{
    //public ColonyTypeEnum colonyType;

    [Header("UI Elements")]
    public List<CollectorPanel> CollectorPanels;
    public List<TextMeshProUGUI> ColonyResourceTexts;
    public List<Button> CollectorAmountButtons;
    private TextMeshProUGUI _resourceText;
    public ColonyType ColonyType;

    void Start()
    {
        SubscribeToLevelAmountButtons();
    }

    private TextMeshProUGUI GetResourceText(CollectorType collectorType)
    {
        foreach (TextMeshProUGUI text in ColonyResourceTexts)
        {
            switch (text.name)
            {
                case "Text_Energy_Resource":
                    if (collectorType == CollectorType.EnergyCollector)
                        return text;
                    break;
                case "Text_Iron_Resource":
                    if (collectorType == CollectorType.IronCollector)
                        return text;
                    break;
                case "Text_Copper_Resource":
                    if (collectorType == CollectorType.CopperCollector)
                        return text;
                    break;
                case "Text_Silicon_Resource":
                    if (collectorType == CollectorType.SiliconCollector)
                        return text;
                    break;
                case "Text_LimeStone_Resource":
                    if (collectorType == CollectorType.LimeStoneCollector)
                        return text;
                    break;
                case "Text_Gold_Resource":
                    if (collectorType == CollectorType.GoldCollector)
                        return text;
                    break;
                case "Text_Aluminum_Resource":
                    if (collectorType == CollectorType.AluminumCollector)
                        return text;
                    break;
                case "Text_Carbon_Resource":
                    if (collectorType == CollectorType.CarbonCollector)
                        return text;
                    break;
                case "Text_Diamond_Resource":
                    if (collectorType == CollectorType.DiamondCollector)
                        return text;
                    break;
                default:
                    break;
            }
        }
        return null;
    }

    private void SubscribeToLevelAmountButtons()
    {
        foreach (Button button in CollectorAmountButtons)
        {
            switch (button.name)
            {
                case "Button_X1":
                    button.onClick.AddListener(() => OnCollectorAmountButtonClicked(1));
                    break;
                case "Button_X5":
                    button.onClick.AddListener(() => OnCollectorAmountButtonClicked(5));
                    break;
                case "Button_X10":
                    button.onClick.AddListener(() => OnCollectorAmountButtonClicked(10));
                    break;
                case "Button_X100":
                    button.onClick.AddListener(() => OnCollectorAmountButtonClicked(100));
                    break;
                default:
                    break;
            }
        }
    }

    private void OnCollectorAmountButtonClicked(int value)
    {
        foreach (CollectorPanel panel in CollectorPanels)
        {
            panel.UpdateUpgradeAmountUI(value);
        }
    }

    private void OnCollectorFinishedHandler(CollectorEventArgs @event)
    {
        _resourceText = GetResourceText(@event.Collector.Data.DataSO.CollectorType);

        if (_resourceText != null)
        {
            _resourceText.text = $"{@event.Collector.Data.GetResourceAmount().ToShortString()} {@event.Collector.Data.GetResourceUnit()}";
        }
    }

    private void OnCostResourcesSpendHandler(CostResourceEventArgs @event)
    {
        _resourceText = GetResourceText(@event.Collector.Data.DataSO.CollectorType);

        if (_resourceText != null)
        {
            _resourceText.text = $"{@event.Collector.Data.GetResourceAmount().ToShortString()} {@event.Collector.Data.GetResourceUnit()}";
        }
    }

    private void OnSoldHandler(CollectorEventArgs @event)
    {
        _resourceText = GetResourceText(@event.Collector.Data.DataSO.CollectorType);

        if (_resourceText != null)
        {
            _resourceText.text = $"{@event.Collector.Data.GetResourceAmount().ToShortString()} {@event.Collector.Data.GetResourceUnit()}";
        }
    }

    private void Subscribe()
    {
        Collector.OnCollectorFinished += OnCollectorFinishedHandler;
        Colony.OnCostResourcesSpend += OnCostResourcesSpendHandler;
        Collector.OnSold += OnSoldHandler;
    }

    private void UnSubscribe()
    {
        Collector.OnCollectorFinished -= OnCollectorFinishedHandler;
        Colony.OnCostResourcesSpend -= OnCostResourcesSpendHandler;
        Collector.OnSold -= OnSoldHandler;
    }

    private void OnEnable()
    {
        Subscribe();
    }

    private void OnDisable()
    {
        UnSubscribe();
    }
}
