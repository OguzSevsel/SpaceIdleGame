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
    [SerializeField] private GameObject _resourceTextParent;
    [SerializeField] private GameObject _resourceTextPrefab;

    private List<Button> _sellResourceButtons = new List<Button>();
    private Resource _selectedResource;

    public event Action<Resource> OnSelectedResourceSell;


    private void Awake()
    {
        _sellButton.onClick.AddListener(SellButtonClickHandler);
        SubscribeToLevelAmountButtons();
    }

    /// <summary>
    /// Creating the resource selection buttons in the sell screen.
    /// </summary>
    /// <param name="resources"></param>
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


    /// <summary>
    /// This will initialize resource texts according to the argument of resource list that it will get from the colony presenter and colony presenter get the list from the colony model. Also this method create a dictionary that we will use in another function in this file.
    /// </summary>
    /// <param name="colonyResources"></param>
    public void InitializeResourceTexts(List<Resource> colonyResources)
    {
        foreach (Resource resource in colonyResources)
        {
            GameObject resourceObject = Instantiate(_resourceTextPrefab);
            Transform iconTransform = resourceObject.transform.Find("Icon_Resource");
            Transform textTransform = resourceObject.transform.Find("Text_Resource");
            Image icon = iconTransform.GetComponent<Image>();
            TextMeshProUGUI text = textTransform.GetComponent<TextMeshProUGUI>();
            icon.sprite = resource.ResourceSO.ResourceIcon;
            resourceObject.transform.SetParent(_resourceTextParent.transform);

            RectTransform resourceRect = resourceObject.GetComponent<RectTransform>();
            resourceRect.anchorMin = Vector2.zero;
            resourceRect.anchorMax = Vector2.one;
            resourceRect.offsetMin = Vector2.zero;
            resourceRect.localScale = Vector3.one;
            resourceRect.position = new Vector3(resourceRect.position.x, resourceRect.position.y, 1);

            text.name = $"text_resource_{colonyResources.IndexOf(resource)}";
            ColonyResourceTexts.Add(text);
        }

        _resourceTextMap = new Dictionary<string, TextMeshProUGUI>();
        foreach (var textVar in ColonyResourceTexts)
        {
            _resourceTextMap[textVar.name.ToLower()] = textVar;
        }
    }

    /// <summary>
    ///Changing the sell selection button UI. 
    /// </summary>
    public void ChangeSellButtonUI()
    {
        foreach (Button button in _sellResourceButtons)
        {
            SellButton sellButton = button.gameObject.GetComponent<SellButton>();
            sellButton.SetButtonTexts();
        }
    }

    /// <summary>
    /// Update resource texts that colony panel has, which they are created according to the resources that colony has. 
    /// </summary>
    /// <param name="resourceSO"></param>
    /// <param name="newAmount"></param>
    /// <param name="index"></param>
    public void UpdateResourceText(ResourceSO resourceSO, double newAmount, int index)
    {
        if (_resourceTextMap != null)
        {
            if (_resourceTextMap.TryGetValue($"text_resource_{index}", out var text))
            {
                text.text = $"{newAmount.ToShortString()} {resourceSO.ResourceUnit}";
            }
        }
    }

    /// <summary>
    /// This will just update all the collector upgrade buttons, according to the colony upgrade amount buttons. 
    /// </summary>
    /// <param name="value"></param>
    private void CollectorAmountButtonClickHandler(int value)
    {
        foreach (CollectorView collectorView in CollectorPanels)
        {
            collectorView.UpdateUpgradeAmountUI(value);
        }
    }

    /// <summary>
    /// This is the event handler of the resource sell selection buttons. 
    /// </summary>
    /// <param name="resource"></param>
    private void SellResourceButtonClickHandler(Resource resource)
    {
        _selectedResource = resource;
        SetSellText();
    }


    /// <summary>
    /// This is updating the sell string that player sees right side of the sell selection buttons. 
    /// </summary>
    public void SetSellText()
    {
        if (_selectedResource != null && _sellText != null)
        {
            _sellText.text = $"You will sell {_selectedResource.ResourceAmount.ToShortString()} {_selectedResource.ResourceSO.ResourceUnit} of {_selectedResource.ResourceSO.resourceType.ToString()} for {(_selectedResource.SellRate * _selectedResource.ResourceAmount).ToShortString()} $?";
        }
    }

    /// <summary>
    /// This is event handler for actual big sell button on the sell panel. 
    /// </summary>
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


    /// <summary>
    /// This is the subscription of the collector upgrade amount buttons. 
    /// </summary>
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
    
    //Generic event subscriptions and unsubscriptions.
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
