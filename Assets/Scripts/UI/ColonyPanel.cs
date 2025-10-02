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

    private void OnCollectorFinished(CollectorFinishedEvent @event)
    {
        _resourceText = GetResourceText(@event.Collector.CollectorData.CollectorType);

        if (_resourceText != null)
        {
            _resourceText.text = $"{@event.Collector.GetResourceAmount()} {@event.Collector.GetResourceUnit()}";
        }
    }

    private void Subscribe()
    {
        EventBus.Subscribe<CollectorFinishedEvent>(OnCollectorFinished);
    }

    private void UnSubscribe()
    {
        EventBus.Unsubscribe<CollectorFinishedEvent>(OnCollectorFinished);
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
