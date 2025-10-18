using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UpgradeView : MonoBehaviour
{
    private UpgradeModel _model;
    private string _targetId;

    [SerializeField] private TextMeshProUGUI _upgradeDescriptionText;
    [SerializeField] private List<GameObject> _upgradeCostList;
    private UnityEngine.UI.Button _upgradeButton;

    public event Action<UpgradeEventArgs> OnUpgradeButtonClicked;

    public void Initialize(UpgradeModel model, string targetId)
    {
        this._model = model;
        this._targetId = targetId;

        InitializeUpgradeCosts();
        _upgradeDescriptionText.text = _model.upgradeDescription;

        _upgradeButton = GetComponentInChildren<UnityEngine.UI.Button>();
        _upgradeButton.onClick.AddListener(UpgradeButtonClickHandler);
    }

    private void UpgradeButtonClickHandler()
    {
        OnUpgradeButtonClicked?.Invoke(new UpgradeEventArgs { TargetId = _targetId, UpgradeModel = _model});
    }

    private void InitializeUpgradeCosts()
    {
        int index = 0;
        
        if (_model.upgradeCosts.Count <= _upgradeCostList.Count)
        {
            foreach (CostResource resource in _model.upgradeCosts)
            {
                GameObject costPanel = _upgradeCostList[index];

                UnityEngine.UI.Image icon = costPanel.GetComponentsInChildren<UnityEngine.UI.Image>()
                      .FirstOrDefault(img => img.gameObject != costPanel);

                TextMeshProUGUI text = costPanel.GetComponentInChildren<TextMeshProUGUI>();

                text.text = resource.GetCostAmount().ToShortString();
                icon.sprite = resource.Resource.ResourceIcon;
                index++;
            }

            if (_upgradeCostList[index] != null)
            {
                GameObject costPanel = _upgradeCostList[index];
                costPanel.SetActive(false);
                index = 0;
            }
        }
        else
        {
            index = 0;
            Debug.LogError("There is more than 4 costs in this upgrade panel");
        }
    }

    public void UpgradePurchasedHandler(CollectorEventArgs args)
    {
        if (args.Collector.CollectorGUID == _model.TargetId)
        {
            this.gameObject.SetActive(false);
        }
    }
}
