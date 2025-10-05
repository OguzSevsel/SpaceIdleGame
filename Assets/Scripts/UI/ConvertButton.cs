using System;
using TMPro;
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

    private CollectorType GetCollectorType(string tag)
    {
        CollectorType collectorType = new CollectorType();

        switch (tag)
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

    private void OnConvertButtonClicked()
    {
        _convertPanel.UpdateUI(_resourceUnit, _resourceName, _moneyAmount, _resourceAmount);
    }

    private void OnSellResourceButtonUpdate(SellResourceButtonUpdateEventArgs @event)
    {
        CollectorType thisCollectorType = GetCollectorType(this.gameObject.name);
        ColonyType eventColonyType = @event.Collector.GetColonyType();

        if (_convertPanel != null)
        {
            if (@event.Collector.CollectorData.CollectorType == thisCollectorType
            && eventColonyType == _convertPanel.ColonyType)
            {
                _resourceAmount = @event.Collector.GetResourceAmount();
                _moneyAmount = @event.Collector.GetSellMoneyAmount();
                _resourceName = @event.Collector.CollectorData.GeneratedResource.resourceType.ToString();
                _resourceUnit = @event.Collector.CollectorData.GeneratedResource.ResourceUnit;

                _textResource.text = $"{_resourceAmount} {_resourceUnit}";
                _textMoney.text = $"{_moneyAmount} $";
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

    private void Subscribe()
    {
        Collector.SellResourceButtonUpdateEvent += OnSellResourceButtonUpdate;
    }

    private void UnSubscribe()
    {
        Collector.SellResourceButtonUpdateEvent -= OnSellResourceButtonUpdate;
    }
}
