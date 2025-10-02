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

    private void OnConvertButtonClicked()
    {
        _convertPanel.UpdateUI(_resourceUnit, _resourceName, _moneyAmount, _resourceAmount);
    }

    //private void OnSellUIChange(SellUIChangeEvent e)
    //{
    //    var collector = e.Collectors.Find(c => c == _collector);

    //    if (collector != null)
    //    {
    //        _resourceAmount = collector.CollectorType.ResourceAmount;
    //        _moneyAmount = collector.CollectorType.SellMoneyAmount;
    //        _resourceName = collector.CollectorType.GeneratedResource.ResourceName.ToString();
    //        _resourceUnit = collector.CollectorType.GeneratedResource.Unit;

    //        _textResource.text = $"{collector.CollectorType.ResourceAmount} {collector.CollectorType.GeneratedResource.Unit}";
    //        _textMoney.text = $"{collector.CollectorType.SellMoneyAmount} $";
    //    }
    //}

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
    }

    private void UnSubscribe()
    {
    }
}
