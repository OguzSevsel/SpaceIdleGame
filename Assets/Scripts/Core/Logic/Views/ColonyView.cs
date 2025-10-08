using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

public class ColonyView : MonoBehaviour
{
    [Header("UI Elements")]
    public List<CollectorView> CollectorPanels;
    public List<TextMeshProUGUI> ColonyResourceTexts;
    public List<Button> CollectorAmountButtons;
    public ColonyType ColonyType;
    private Dictionary<string, TextMeshProUGUI> _resourceTextMap;

    [Header("Convert Panel UI Elements")]
    [SerializeField] private GameObject _sellButtonPanel;
    [SerializeField] private Button _sellButtonResource;
    [SerializeField] private Button _sellAllButton;
    [SerializeField] private TextMeshProUGUI _sellText;
    [SerializeField] private Button _sellButton;


    private void AddSellButtonsToPanel(List<Resource> resources)
    {
        foreach (Resource resource in resources)
        {
            GameObject sellButtonObject = Instantiate(_sellButtonResource.gameObject, _sellButtonPanel.transform);

            Button sellButton = sellButtonObject.GetComponent<Button>();

            sellButton.onClick.AddListener(() => OnSellResourceButtonClicked(resource));
        }
    }

    private void ChangeSellText()
    {

    }

    private void OnSellResourceButtonClicked(Resource resource)
    {
        
    }

    private void OnSellButtonClicked()
    {

    }

    private void Awake()
    {
        _sellButton.onClick.AddListener(OnSellButtonClicked);


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
