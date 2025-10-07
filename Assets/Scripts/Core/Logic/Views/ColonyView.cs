using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

public class ColonyView : MonoBehaviour
{
    //public ColonyTypeEnum colonyType;

    [Header("UI Elements")]
    public List<CollectorView> CollectorPanels;
    public List<TextMeshProUGUI> ColonyResourceTexts;
    public List<Button> CollectorAmountButtons;
    private TextMeshProUGUI _resourceText;
    public ColonyType ColonyType;


    private Dictionary<string, TextMeshProUGUI> _resourceTextMap;
    private void Awake()
    {
        SubscribeToLevelAmountButtons();
        _resourceTextMap = new Dictionary<string, TextMeshProUGUI>();
        foreach (var text in ColonyResourceTexts)
        {
            _resourceTextMap[text.name.ToLower()] = text;
        }
    }

    public void UpdateResourceText(ResourceSO resourceSO, double newAmount)
    {
        string key = resourceSO.resourceType.ToString().ToLower();
        if (_resourceTextMap.TryGetValue($"text_{key}_resource", out var text))
        {
            text.text = $"{newAmount.ToShortString()} {resourceSO.ResourceUnit}";
        }
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
        foreach (CollectorView collectorView in CollectorPanels)
        {
            collectorView.UpdateUpgradeAmountUI(value);
        }
    }

    private void Subscribe()
    {
    }

    private void UnSubscribe()
    {
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
