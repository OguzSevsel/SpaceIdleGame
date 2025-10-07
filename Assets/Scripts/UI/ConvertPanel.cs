using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class ConvertPanel : MonoBehaviour
{
    [SerializeField] private Button _buttonSell;
    [SerializeField] private TextMeshProUGUI _textSell;
    public ColonyType ColonyType;
    public static event Action<SellButtonEventArgs> OnSellButtonClick;
    public CollectorType LastSelectedCollectorType;
    public double LastSelectedResourceAmount;
    public double LastSelectedMoneyAmount;

    private void Awake()
    {
        _buttonSell.onClick.AddListener(OnSellButtonClicked);
        ColonyType = GetComponentInParent<ColonyView>().ColonyType;
    }

    private void OnSellButtonClicked()
    {
        OnSellButtonClick?.Invoke(new SellButtonEventArgs 
        { 
            CollectorType = LastSelectedCollectorType,
            ColonyType = ColonyType,
            ResourceAmount = LastSelectedResourceAmount,
            MoneyAmount = LastSelectedMoneyAmount
        });
    }

    public void UpdateUI(string resourceUnit, string resourceName, double moneyAmount, double resourceAmount)
    {
        _textSell.text = $"You will sell {resourceAmount.ToShortString()} {resourceUnit} of {resourceName} for {moneyAmount.ToShortString()}?";
    }
}
