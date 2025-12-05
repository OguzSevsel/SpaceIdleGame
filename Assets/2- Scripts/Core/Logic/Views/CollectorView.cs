using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using System.Linq;

public class CollectorView : MonoBehaviour
{
    [Header("Collector UI Elements")]
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _level;
    [SerializeField] private TextMeshProUGUI _upgradeEffect;

    [Header("Upgrade Elements")]
    [SerializeField] private Button _button_Upgrade;
    [SerializeField] private TextMeshProUGUI _textUpgradeButton;
    [SerializeField] private List<GameObject> _upgradeCosts;

    [Header("Progress Bar Elements")]
    [SerializeField] private ProgressBar _progressBar;
    [SerializeField] private TextMeshProUGUI _textProgressBarTime;

    [Header("Output Text")]
    [SerializeField] private TextMeshProUGUI _textOutput;


    //Private fields
    private Button _buttonCollectorPanel;

    public event Action<int> OnUpgradeAmountChanged;
    public event Action OnCollectorCollect;
    public event Action OnCollectorUpgrade;

    #region Initialization Functions

    /// <summary>
    /// This is the entry point for collector card UI.
    /// </summary>
    /// <param name="collectorModel"></param>
    public void Initialize(CollectorModel collectorModel)
    {
        _buttonCollectorPanel = GetComponent<Button>();
        _buttonCollectorPanel.onClick.AddListener(OnCollectorPanelClickHandler);
        _button_Upgrade.onClick.AddListener(OnCollectorPanelUpgradeClickHandler);
        InitializeUI(collectorModel);
    }

    /// <summary>
    /// This is the function that sets the UI for Collector Cards.
    /// </summary>
    /// <param name="collectorModel"></param>
    private void InitializeUI(CollectorModel collectorModel)
    {
        _iconImage.sprite = collectorModel.Data.DataSO.GeneratedResource.ResourceIcon;
        double upgradeEffect = collectorModel.Data.GetCollectionRateMultiplier();
        _upgradeEffect.text = $"x{upgradeEffect.ToShortString()}";
        _textUpgradeButton.text = "LVL UP +1";
        _name.text = $"{collectorModel.Data.DataSO.CollectorName}";
        _level.text = $"LVL {collectorModel.Data.GetLevel()}";
        _textOutput.text = $"{collectorModel.Data.GetCollectionRate().ToShortString()} {collectorModel.Data.DataSO.GeneratedResource.ResourceUnit}";

        List<GameObject> costResources = GetCostResources(collectorModel, number: collectorModel.LevelUpgrade.upgradeCosts.Count);

        foreach (GameObject resource in costResources)
        {
            int index = costResources.IndexOf(resource);

            TextMeshProUGUI costResourceText = resource.GetComponentInChildren<TextMeshProUGUI>();
            Image icon = resource.GetComponentInChildren<Image>();

            costResourceText.text = $"{collectorModel.LevelUpgrade.upgradeCosts[index].GetCostAmount().ToShortString()} {collectorModel.LevelUpgrade.upgradeCosts[index].Resource.ResourceUnit}";
            icon.sprite = collectorModel.LevelUpgrade.upgradeCosts[index].Resource.ResourceIcon;
        }
    }

    #endregion

    #region Utility Functions

    /// <summary>
    /// Utility function for getting cost resources from the collector model, and arranging them for UI.
    /// </summary>
    /// <param name="collectorModel"></param>
    /// <param name="number"></param>
    /// <returns></returns>
    private List<GameObject> GetCostResources(CollectorModel collectorModel, int? number = null)
    {
        List<GameObject> upgradeCosts = new List<GameObject>();

        if (number.HasValue)
        {
            if (number.Value < _upgradeCosts.Count())
            {
                foreach (GameObject resource in _upgradeCosts)
                {
                    if (_upgradeCosts.IndexOf(resource) < number.Value)
                    {
                        upgradeCosts.Add(resource);
                    }
                    else
                    {
                        resource.SetActive(false);
                    }
                }
            }
            else
            {
                upgradeCosts = _upgradeCosts.Take(number.Value).ToList();
            }
        }
        else
        {
            upgradeCosts = _upgradeCosts.Take(collectorModel.LevelUpgrade.upgradeCosts.Count).ToList();
        }

        return upgradeCosts;
    }

    #endregion

    #region Update UI Functions

    /// <summary>
    /// Update Upgrade Button Text. 
    /// </summary>
    /// <param name="collectAmount"></param>
    public void UpdateUpgradeAmountUI(int collectAmount)
    {
        _textUpgradeButton.text = $"LVL UP +{collectAmount}";
        OnUpgradeAmountChanged?.Invoke(collectAmount);
    }

    /// <summary>
    /// Update Upgrade Effect, Cost Resources Texts, Level Text 
    /// </summary>
    /// <param name="costResources"></param>
    /// <param name="upgradeEffect"></param>
    /// <param name="upgradeLevel"></param>
    private void UpdateUpgradeUIElements(List<CostResource> costResources, double upgradeEffect, int upgradeLevel)
    {
        for (int i = 0; i < costResources.Count; i++)
        {
            CostResource resource = costResources[i];
            TextMeshProUGUI text = _upgradeCosts[i].GetComponentInChildren<TextMeshProUGUI>();

            text.text = $"{resource.GetCostAmount().ToShortString()} {resource.Resource.ResourceUnit}";
        }

        _upgradeEffect.text = $"x{upgradeEffect.ToShortString()}";
        _level.text = $"LVL {upgradeLevel}";
    }

    /// <summary>
    /// Update Progress Bar UI. 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="time"></param>
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
        _textProgressBarTime.text = time.ToShortString() + " Sec";
    }

    /// <summary>
    /// Update Collector Output Text 
    /// </summary>
    /// <param name="output"></param>
    /// <param name="unit"></param>
    private void UpdateOutputUIElements(double output, string unit)
    {
        _textOutput.text = $"{output.ToShortString()} {unit}";
    }

    #endregion

    #region Events

    /// <summary>
    /// Progress Bar Update Event 
    /// </summary>
    /// <param name="event"></param>
    public void ProgressBarUpdate(ProgressBarUpdateArgs @event)
    {
        UpdateProgressBarUIElements(@event.Value, (float)@event.RemainingTime);
    }

    /// <summary>
    /// Collector Button Event Handler. 
    /// </summary>
    private void OnCollectorPanelClickHandler()
    {
        OnCollectorCollect?.Invoke();
    }


    /// <summary>
    /// Collector Upgrade Button. 
    /// </summary>
    private void OnCollectorPanelUpgradeClickHandler()
    {
        OnCollectorUpgrade?.Invoke();
    }


    /// <summary>
    /// This one will be called after collector upgraded. 
    /// </summary>
    /// <param name="event"></param>
    public void CollectorUpgradedHandler(CollectorEventArgs @event)
    {
        List<CostResource> costResources = @event.Collector.LevelUpgrade.upgradeCosts;

        if (costResources.Count <= 4 && costResources.Count >= 1)
        {
            UpdateUpgradeUIElements(@event.Collector.LevelUpgrade.upgradeCosts, @event.Collector.Data.GetCollectionRateMultiplier(), @event.Collector.Data.GetLevel());
            UpdateOutputUIElements(@event.Collector.Data.GetCollectionRate(), @event.Collector.Data.DataSO.GeneratedResource.ResourceUnit);
        }
        else
        {
            Debug.LogWarning($"Please check Cost resources of this collector: {@event.Collector.gameObject.name}");
        }
    }


    /// <summary>
    /// This will be called after collector upgrade amount changed via colony panel 
    /// </summary>
    /// <param name="event"></param>
    public void CollectorUpgradeAmountChanged(CollectorEventArgs @event)
    {
        UpdateUpgradeUIElements(@event.Collector.LevelUpgrade.upgradeCosts, @event.Collector.Data.GetCollectionRateMultiplier(), @event.Collector.Data.GetLevel());
    }

    #endregion
}
