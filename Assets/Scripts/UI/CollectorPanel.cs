using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CollectorPanel : MonoBehaviour
{
    [Header("Upgrade Elements")]
    [SerializeField] private Button button_Upgrade;
    [SerializeField] private TextMeshProUGUI _textUpgradeButton;
    [SerializeField] private TextMeshProUGUI _textUpgradeCost1;
    [SerializeField] private TextMeshProUGUI _textUpgradeCost2;
    [SerializeField] private TextMeshProUGUI _textUpgradeCost3;
    [SerializeField] private TextMeshProUGUI _textUpgradeCost4;
    [SerializeField] private TextMeshProUGUI _textUpgradeEffect;
    [SerializeField] private TextMeshProUGUI _textUpgradeLevel;

    [Header("Progress Bar Elements")]
    [SerializeField] private ProgressBar _progressBar;
    [SerializeField] private TextMeshProUGUI _textProgressBarTime;

    [Header("Output Text")]
    [SerializeField] private TextMeshProUGUI _textOutput;

    private Button _buttonCollectorPanel;
    private CollectorTypeEnum _collectorType;
    [SerializeField] private ColonyTypeEnum _colonyType;

    

    private void OnCollectorLevelAmount(CollectorLevelAmount e)
    {
        UpdateUpgradeAmountUI(e.Amount);
    }

    private void OnProgressChanged(CollectorProgressEvent e)
    {
        if (e.CollectorType == _collectorType && e.ColonyType == this._colonyType)
        {
            UpdateProgressBarUIElements(e.Progress, e.TimeRemaining);
        }
    }

    private void Start()
    {
        selectCollectorType();
        _buttonCollectorPanel = GetComponent<Button>();
        _buttonCollectorPanel.onClick.AddListener(OnCollectorPanelButtonClicked);
        button_Upgrade.onClick.AddListener(OnUpgradeButtonClicked);
    }

    private void OnUpgradeButtonClicked()
    {
        EventBus.Publish(new CollectorUpgradeStartedEvent() 
        { 
            CollectorType = this._collectorType,
            ColonyType = this._colonyType,
            UpgradeCategory = UpgradeCategory.Local,
            UpgradeType = UpgradeType.CollectorAmount,
            UpgradeMultiplier = 1.1d
        });
    }

    private void OnCollectorPanelButtonClicked()
    {
        EventBus.Publish(new CollectorStartedEvent() { CollectorType = this._collectorType, ColonyType = this._colonyType});
    }

    private void UpdateUpgradeAmountUI(int collectAmount)
    {
        _textUpgradeButton.text = $"Upgrade +{collectAmount}";
    }

    private void UpdateUpgradeUIElements(double upgradeCost1, double upgradeCost2, double upgradeCost3, double upgradeCost4, string upgradeEffect, string upgradeLevel)
    {
        _textUpgradeCost1.text = upgradeCost1.ToString();
        _textUpgradeCost2.text = upgradeCost2.ToString();
        _textUpgradeCost3.text = upgradeCost3.ToString();
        _textUpgradeCost4.text = upgradeCost4.ToString();
        _textUpgradeEffect.text = upgradeEffect;
        _textUpgradeLevel.text = upgradeLevel;
    }

    private void UpdateProgressBarUIElements(float value, float time)
    {
        if (value < 0f)
        {
            value = 0f;
        }

        if (time < 0f)
        {
            time = 0f;
        }

        _progressBar.progressValue = value;
        _textProgressBarTime.text = time.ToString() + " Sec";
    }

    private void UpdateOutputUIElements(string output)
    {
        _textOutput.text = output; 
    }

    private void selectCollectorType()
    {
        switch (gameObject.name)
        {
            case "Energy_Industry":
                _collectorType = CollectorTypeEnum.EnergyCollector;
                break;
            case "Iron_Industry":
                _collectorType = CollectorTypeEnum.IronCollector;
                break;
            case "Copper_Industry":
                _collectorType = CollectorTypeEnum.CopperCollector;
                break;
            case "Silicon_Industry":
                _collectorType = CollectorTypeEnum.SiliconCollector;
                break;
            case "LimeStone_Industry":
                _collectorType = CollectorTypeEnum.LimeStoneCollector;
                break;
            case "Gold_Industry":
                _collectorType = CollectorTypeEnum.GoldCollector;
                break;
            case "Aluminum_Industry":
                _collectorType = CollectorTypeEnum.AluminiumCollector;
                break;
            case "Carbon_Industry":
                _collectorType = CollectorTypeEnum.CarbonCollector;
                break;
            case "Diamond_Industry":
                _collectorType = CollectorTypeEnum.DiamondCollector;
                break;
            default:
                break;
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

    private void UnSubscribe()
    {
        EventBus.Unsubscribe<CollectorProgressEvent>(OnProgressChanged);
        EventBus.Unsubscribe<CollectorLevelAmount>(OnCollectorLevelAmount);
    }

    private void Subscribe()
    {
        EventBus.Subscribe<CollectorProgressEvent>(OnProgressChanged);
        EventBus.Subscribe<CollectorLevelAmount>(OnCollectorLevelAmount);
    }
}
