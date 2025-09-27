using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CollectorPanel : MonoBehaviour
{
    [Header("Upgrade Elements")]
    [SerializeField] private Button button_Upgrade;
    [SerializeField] private TextMeshProUGUI text_UpgradeButton;
    [SerializeField] private TextMeshProUGUI text_UpgradeCost_1;
    [SerializeField] private TextMeshProUGUI text_UpgradeCost_2;
    [SerializeField] private TextMeshProUGUI text_UpgradeCost_3;
    [SerializeField] private TextMeshProUGUI text_UpgradeCost_4;
    [SerializeField] private TextMeshProUGUI text_UpgradeEffect;
    [SerializeField] private TextMeshProUGUI text_UpgradeLevel;

    [Header("Progress Bar Elements")]
    [SerializeField] private ProgressBar progressBar;
    [SerializeField] private TextMeshProUGUI text_ProgressBarTime;

    [Header("Output Text")]
    [SerializeField] private TextMeshProUGUI text_Output;

    private Button _button_CollectorPanel;
    private CollectorTypeEnum _collectorType;
    [SerializeField] private ColonyTypeEnum colonyType;

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
    }

    private void Subscribe()
    {
        EventBus.Subscribe<CollectorProgressEvent>(OnProgressChanged);
    }

    private void OnProgressChanged(CollectorProgressEvent e)
    {
        if (e.collectorType == _collectorType && e.colonyType == this.colonyType)
        {
            UpdateProgressBarUIElements(e.progress, e.timeRemaining);
        }
    }

    private void Start()
    {
        selectCollectorType();
        _button_CollectorPanel = GetComponent<Button>();
        _button_CollectorPanel.onClick.AddListener(OnCollectorPanelButtonClicked);
        button_Upgrade.onClick.AddListener(OnUpgradeButtonClicked);
    }

    private void OnUpgradeButtonClicked()
    {
        EventBus.Publish(new CollectorUpgradeRequestedEvent() { collectorType = this._collectorType , colonyType = this.colonyType});
    }

    private void OnCollectorPanelButtonClicked()
    {
        EventBus.Publish(new CollectorRequestedEvent() { collectorType = this._collectorType, colonyType = this.colonyType});
    }

    private void UpdateUpgradeUIElements(int collectAmount, double upgradeCost1, double upgradeCost2, double upgradeCost3, double upgradeCost4, string upgradeEffect, string upgradeLevel)
    {
        text_UpgradeButton.text = $"Upgrade +{collectAmount}";
        text_UpgradeCost_1.text = upgradeCost1.ToString();
        text_UpgradeCost_2.text = upgradeCost2.ToString();
        text_UpgradeCost_3.text = upgradeCost3.ToString();
        text_UpgradeCost_4.text = upgradeCost4.ToString();
        text_UpgradeEffect.text = upgradeEffect;
        text_UpgradeLevel.text = upgradeLevel;
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

        progressBar.progressValue = value;
        text_ProgressBarTime.text = time.ToString() + " Sec";
    }

    private void UpdateOutputUIElements(string output)
    {
        text_Output.text = output; 
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
}
