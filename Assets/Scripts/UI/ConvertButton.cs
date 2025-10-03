using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ConvertButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textResource;
    [SerializeField] private TextMeshProUGUI _textMoney;
    private double _resourceAmount;
    private double _moneyAmount;
    private string _resourceName;
    private string _resourceUnit;
    private ConvertPanel _convertPanel;

    private void Start()
    {
        _convertPanel = this.GetComponentInParent<ConvertPanel>();
        this.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnConvertButtonClicked);
    }

    private void OnConvertButtonClicked()
    {
        _convertPanel.UpdateUI(_resourceUnit, _resourceName, _moneyAmount, _resourceAmount);
    }

    private void OnSellResourceButtonUpdate(SellResourceButtonUpdateEvent e)
    {
        var collector = e.Collector;

        if (collector != null && collector.CollectorData.CollectorType == GetCollectorType(this.gameObject.name) && e.Collector.GetColonyType() == _convertPanel.ColonyType)
        {
            _resourceAmount = collector.GetResourceAmount();
            _moneyAmount = collector.GetSellMoneyAmount();
            _resourceName = collector.CollectorData.GeneratedResource.resourceType.ToString();
            _resourceUnit = collector.CollectorData.GeneratedResource.ResourceUnit;

            _textResource.text = $"{_resourceName} {_resourceUnit}";
            _textMoney.text = $"{_moneyAmount} $";
        }
    }

    private CollectorType GetCollectorType(string name)
    {
        CollectorType collectorType = new CollectorType();

        switch (name)
        {
            case "Button_Energy":
                collectorType = CollectorType.EnergyCollector;
                break;
            case "Button_Iron":
                collectorType = CollectorType.IronCollector;
                break;
            case "Button_Copper":
                collectorType = CollectorType.CopperCollector;
                break;
            case "Button_Silicon":
                collectorType = CollectorType.SiliconCollector;
                break;
            case "Button_LimeStone":
                collectorType = CollectorType.LimeStoneCollector;
                break;
            case "Button_Gold":
                collectorType = CollectorType.GoldCollector;
                break;
            case "Button_Aluminum":
                collectorType = CollectorType.AluminumCollector;
                break;
            case "Button_Carbon":
                collectorType = CollectorType.CarbonCollector;
                break;
            case "Button_Diamond":
                collectorType = CollectorType.DiamondCollector;
                break;
            default:
                Debug.LogWarning("There is no corresponding Collector.");
                break;
        }

        return collectorType;
    }


    private void OnEnable()
    {
        Subscribe();
    }

    private void OnDisable()
    {
        UnSubscribe();
    }

    private void Subscribe()
    {
        EventBus.Subscribe<SellResourceButtonUpdateEvent>(OnSellResourceButtonUpdate);
    }

    private void UnSubscribe()
    {
        EventBus.Unsubscribe<SellResourceButtonUpdateEvent>(OnSellResourceButtonUpdate);
    }
}
