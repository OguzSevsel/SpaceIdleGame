using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;
using System.Linq;

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
    [SerializeField] private GameObject _sellButtonResource;
    [SerializeField] private Button _sellAllButton;
    [SerializeField] private TextMeshProUGUI _sellText;
    [SerializeField] private Button _sellButton;
    private List<Button> _sellResourceButtons = new List<Button>();
    private Resource _selectedResource;

    public event Action<Resource> OnSelectedResourceSell;


    private void Awake()
    {
        _sellButton.onClick.AddListener(SellButtonClickHandler);


        SubscribeToLevelAmountButtons();


        _resourceTextMap = new Dictionary<string, TextMeshProUGUI>();
        foreach (var text in ColonyResourceTexts)
        {
            _resourceTextMap[text.name.ToLower()] = text;
        }
    }

    public void InitializeSellButtons(List<Resource> resources)
    {
        foreach (Resource resource in resources)
        {
            if (resource.ResourceSO.resourceType != ResourceType.Money)
            {
                GameObject sellButtonObject = Instantiate(_sellButtonResource, _sellButtonPanel.transform);

                Button sellButton = sellButtonObject.GetComponent<Button>();

                sellButton.onClick.AddListener(() => SellResourceButtonClickHandler(resource));

                string resourceText = $"{resource.ResourceAmount} {resource.ResourceSO.ResourceUnit}";
                string moneyText = $"0 $";
                sellButton.GetComponent<SellButton>().SetResource(resource);
                sellButton.GetComponent<SellButton>().Initialize(resource.ResourceSO.ResourceIcon, resourceText, moneyText);
                _sellResourceButtons.Add(sellButton);
            }
        }
    }

    public void ChangeSellButtonUI()
    {
        foreach (Button button in _sellResourceButtons)
        {
            SellButton sellButton = button.gameObject.GetComponent<SellButton>();
            sellButton.SetButtonTexts();
        }
    }

    public void UpdateResourceText(ResourceSO resourceSO, double newAmount, int index)
    {
        string key = resourceSO.resourceType.ToString().ToLower();
        if (_resourceTextMap.TryGetValue($"text_resource_{index}", out var text))
        {
            text.text = $"{newAmount.ToShortString()} {resourceSO.ResourceUnit}";
        }
    }

    private void CollectorAmountButtonClickHandler(int value)
    {
        foreach (CollectorView collectorView in CollectorPanels)
        {
            collectorView.UpdateUpgradeAmountUI(value);
        }
    }

    private void SellResourceButtonClickHandler(Resource resource)
    {
        _selectedResource = resource;
        SetSellText();
    }

    public void SetSellText()
    {
        if (_selectedResource != null)
        {
            _sellText.text = $"You will sell {_selectedResource.ResourceAmount.ToShortString()} {_selectedResource.ResourceSO.ResourceUnit} of {_selectedResource.ResourceSO.resourceType.ToString()} for {(_selectedResource.SellRate * _selectedResource.ResourceAmount).ToShortString()} $?";
        }
    }

    private void SellButtonClickHandler()
    {
        if (_selectedResource != null)
        {
            OnSelectedResourceSell?.Invoke(_selectedResource);
        }
        else
        {
            _sellText.text = $"Please select a resource from left";
        }
    }

    private void SubscribeToLevelAmountButtons()
    {
        foreach (Button button in CollectorAmountButtons)
        {
            switch (button.name)
            {
                case "Button_X1":
                    button.onClick.AddListener(() => CollectorAmountButtonClickHandler(1));
                    break;
                case "Button_X5":
                    button.onClick.AddListener(() => CollectorAmountButtonClickHandler(5));
                    break;
                case "Button_X10":
                    button.onClick.AddListener(() => CollectorAmountButtonClickHandler(10));
                    break;
                case "Button_X100":
                    button.onClick.AddListener(() => CollectorAmountButtonClickHandler(100));
                    break;
                default:
                    break;
            }
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
