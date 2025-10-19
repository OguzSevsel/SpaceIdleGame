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
    [SerializeField] private List<GameObject> _costListObjects;
    private UnityEngine.UI.Button _upgradeButton;

    public event Action<UpgradeEventArgs> OnUpgradeButtonClicked;

    private Dictionary<string, GameObject> _resourceTextMap;

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
        
        if (_model.upgradeCosts.Count <= _costListObjects.Count)
        {
            foreach (CostResource resource in _model.upgradeCosts)
            {
                GameObject costPanel = _costListObjects[index];
                costPanel.name = resource.Resource.resourceType.ToString();
                UnityEngine.UI.Image icon = costPanel.GetComponentsInChildren<UnityEngine.UI.Image>()
                      .FirstOrDefault(img => img.gameObject != costPanel);

                TextMeshProUGUI text = costPanel.GetComponentInChildren<TextMeshProUGUI>();

                text.text = resource.GetCostAmount().ToShortString();
                icon.sprite = resource.Resource.ResourceIcon;
                index++;
            }

            for (int i = index; i < _costListObjects.Count; i++)
            {
                GameObject costPanel = _costListObjects[i];
                costPanel.SetActive(false);
            }
        }
        else
        {
            index = 0;
            Debug.LogError($"There is more than 4 costs in this upgrade panel {_model.upgradeCosts.Count}");
        }

        _resourceTextMap = new Dictionary<string, GameObject>();
        foreach (var cost in _costListObjects)
        {
            _resourceTextMap[cost.name.ToLower()] = cost;
        }
    }

    public void UpgradePurchasedHandler(CollectorEventArgs args)
    {
        if (args.Collector.CollectorGUID == _model.TargetId)
        {
            this.gameObject.SetActive(false);
        }
    }

    public void OnUpgradeInfoShowHandler(InfoUpgradeEventArgs args)
    {
        if (args.TargetId == _model.TargetId)
        {
            if (_resourceTextMap != null)
            {
                if (_resourceTextMap.TryGetValue(args.Resource.Resource.resourceType.ToString().ToLower(), out GameObject cost))
                {
                    UnityEngine.UI.Image image = cost.GetComponent<UnityEngine.UI.Image>();

                    if (!args.IsEnoughResource)
                    {
                        image.color = Color.red;    
                    }
                    else
                    {
                        image.color = Color.green;    
                    }
                }
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
    }

    private void UnSubscribe()
    {
    }
}
