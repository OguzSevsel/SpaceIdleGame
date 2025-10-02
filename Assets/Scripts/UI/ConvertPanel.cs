using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class ConvertPanel : MonoBehaviour
{
    [SerializeField] private List<ConvertButton> _convertButtons;
    [SerializeField] private Button _buttonSell;
    [SerializeField] private TextMeshProUGUI _textSell;

    private void Start()
    {
        _buttonSell.onClick.AddListener(OnSellButtonClicked);
    }

    private void OnSellButtonClicked()
    {
        
    }

    public void UpdateUI(string resourceUnit, string resourceName, double moneyAmount, double resourceAmount)
    {
        _textSell.text = $"You will sell {resourceAmount} {resourceUnit} of {resourceName} for {moneyAmount}?";
    }
}
