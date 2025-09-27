using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

public class ColonyPanel : MonoBehaviour
{
    public ColonyTypeEnum colonyType;

    [Header("UI Elements")]
    public List<CollectorPanel> CollectorPanels;
    public List<TextMeshProUGUI> ColonyResourceTexts;
    public List<Button> CollectorAmountButtons;
    
    private TextMeshProUGUI _resourceText;

    void Start()
    {
        SubscribeToLevelAmountButtons();
    }

    private void OnEnable()
    {
        EventBus.Subscribe<CollectorFinishedEvent>(OnCollectorFinished);
    }

    private void OnCollectorFinished(CollectorFinishedEvent e)
    {
        if (e.colonyType == this.colonyType)
        {
            _resourceText = GetResourceText(e.collector.CollectorType.CollectorTypeName);

            if (_resourceText != null)
            {
                _resourceText.text = e.collector.ToString();
            }
        }
    }

    private TextMeshProUGUI GetResourceText(CollectorTypeEnum collectorType)
    {
        foreach (TextMeshProUGUI text in ColonyResourceTexts)
        {
            switch (text.name)
            {
                case "Text_Energy_Resource":
                    if (collectorType == CollectorTypeEnum.EnergyCollector)
                        return text;
                    break;
                case "Text_Iron_Resource":
                    if (collectorType == CollectorTypeEnum.IronCollector)
                        return text;
                    break;
                case "Text_Copper_Resource":
                    if (collectorType == CollectorTypeEnum.CopperCollector)
                        return text;
                    break;
                case "Text_Silicon_Resource":
                    if (collectorType == CollectorTypeEnum.SiliconCollector)
                        return text;
                    break;
                case "Text_LimeStone_Resource":
                    if (collectorType == CollectorTypeEnum.LimeStoneCollector)
                        return text;
                    break;
                case "Text_Gold_Resource":
                    if (collectorType == CollectorTypeEnum.GoldCollector)
                        return text;
                    break;
                case "Text_Aluminum_Resource":
                    if (collectorType == CollectorTypeEnum.AluminiumCollector)
                        return text;
                    break;
                case "Text_Carbon_Resource":
                    if (collectorType == CollectorTypeEnum.CarbonCollector)
                        return text;
                    break;
                case "Text_Diamond_Resource":
                    if (collectorType == CollectorTypeEnum.DiamondCollector)
                        return text;
                    break;
                default:
                    break;
            }
        }
        return null;
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<CollectorFinishedEvent>(OnCollectorFinished);
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
        EventBus.Publish(new CollectorLevelAmountRequestedEvent() { amount = value });
    }
}
