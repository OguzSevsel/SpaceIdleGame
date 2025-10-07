using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using Unity.Properties;
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
    private ColonyView ColonyPanel;


    public event Action<int> OnUpgradeAmountChanged;
    public event Action OnCollectorCollect;
    public event Action OnCollectorUpgrade;

    #region Initialization Functions

    public void Initialize(CollectorModel collectorModel)
    {
        _buttonCollectorPanel = GetComponent<Button>();
        _buttonCollectorPanel.onClick.AddListener(OnCollectorPanelClickHandler);
        _button_Upgrade.onClick.AddListener(OnCollectorPanelUpgradeClickHandler);
        ColonyPanel = GetComponentInParent<ColonyView>();
        InitializeUI(collectorModel);
    }

    private void InitializeUI(CollectorModel collectorModel)
    {
        _iconImage.sprite = collectorModel.Data.DataSO.GeneratedResource.ResourceIcon;
        double upgradeEffect = collectorModel.Data.GetCollectionRateMultiplier();
        _upgradeEffect.text = $"x{upgradeEffect.ToShortString()}";
        _textUpgradeButton.text = "LVL UP +1";
        _name.text = $"{collectorModel.Data.DataSO.CollectorName}";
        _level.text = $"LVL {collectorModel.Data.GetLevel()}";
        _textOutput.text = $"{collectorModel.Data.GetCollectionRate().ToShortString()} {collectorModel.Data.DataSO.GeneratedResource.ResourceUnit}";

        List<GameObject> costResources = GetCostResources(collectorModel, number: collectorModel.Data.CostResources.Count);

        foreach (GameObject resource in costResources)
        {
            int index = costResources.IndexOf(resource);

            TextMeshProUGUI costResourceText = resource.GetComponentInChildren<TextMeshProUGUI>();
            Image icon = resource.GetComponentInChildren<Image>();

            costResourceText.text = $"{collectorModel.Data.CostResources[index].GetCostAmount().ToShortString()} {collectorModel.Data.CostResources[index].Resource.ResourceUnit}";
            icon.sprite = collectorModel.Data.CostResources[index].Resource.ResourceIcon;
        }
    }

    #endregion

    #region Utility Functions

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
            upgradeCosts = _upgradeCosts.Take(collectorModel.Data.CostResources.Count).ToList();
        }

        return upgradeCosts;
    }

    #endregion

    #region Update UI Functions

    //Update Upgrade Button Text
    public void UpdateUpgradeAmountUI(int collectAmount)
    {
        _textUpgradeButton.text = $"LVL UP +{collectAmount}";
        OnUpgradeAmountChanged?.Invoke(collectAmount);
    }

    //Update Upgrade Effect, Cost Resources Texts, Level Text
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

    //Update Progress Bar UI
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

    //Update Collector Output Text
    private void UpdateOutputUIElements(double output, string unit)
    {
        _textOutput.text = $"{output.ToShortString()} {unit}";
    }

    #endregion

    #region Events

    //Progress Bar Update Event
    public void ProgressBarUpdate(ProgressBarUpdateArgs @event)
    {
        UpdateProgressBarUIElements(@event.Value, (float)@event.RemainingTime);
    }

    //Collector Button.
    private void OnCollectorPanelClickHandler()
    {
        OnCollectorCollect?.Invoke();
    }

    //Collector Upgrade Button.
    private void OnCollectorPanelUpgradeClickHandler()
    {
        OnCollectorUpgrade?.Invoke();
    }

    //This one will be called after collector upgraded.
    public void CollectorUpgradedHandler(CollectorEventArgs @event)
    {
        List<CostResource> costResources = @event.Collector.Data.CostResources;

        if (costResources.Count <= 4 && costResources.Count >= 1)
        {
            UpdateUpgradeUIElements(@event.Collector.Data.CostResources, @event.Collector.Data.GetCollectionRateMultiplier(), @event.Collector.Data.GetLevel());
            UpdateOutputUIElements(@event.Collector.Data.GetCollectionRate(), @event.Collector.Data.DataSO.GeneratedResource.ResourceUnit);
        }
        else
        {
            Debug.LogWarning($"Please check Cost resources of this collector: {@event.Collector.gameObject.name}");
        }
    }

    //This will be called after collector upgrade amount changed via colony panel
    public void CollectorUpgradeAmountChanged(CollectorEventArgs @event)
    {
        UpdateUpgradeUIElements(@event.Collector.Data.CostResources, @event.Collector.Data.GetCollectionRateMultiplier(), @event.Collector.Data.GetLevel());
    }

    #endregion
}
