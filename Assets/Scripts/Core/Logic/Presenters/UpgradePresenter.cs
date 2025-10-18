using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpgradePresenter : MonoBehaviour
{
    [SerializeField] private List<UpgradeModel> _upgradeModels;
    [SerializeField] private List<UpgradeView> _upgradeViews;
    [SerializeField] private GameObject _upgradeViewParent;
    [SerializeField] private GameObject _upgradeViewPrefab;

    private List<IUpgradeable> upgradeables;
    public event Action<CollectorEventArgs> OnCollectorUpgrade;

    private void Awake()
    {
        upgradeables = new List<IUpgradeable>();
        foreach (UpgradeModel upgradeModel in _upgradeModels)
        {
            CreateUpgradePanel(upgradeModel, upgradeModel.TargetId);
        }
    }

    private void RegisterCollector(CollectorEventArgs args)
    {
        upgradeables.Add(args.Collector);
    }

    //When creating upgrade views we will assing them a target ID and UpgradeModel.
    private void CreateUpgradePanel(UpgradeModel model, string targetId)
    {
        GameObject viewObject = Instantiate(_upgradeViewPrefab, _upgradeViewParent.transform);
        UpgradeView view = viewObject.GetComponent<UpgradeView>();
        viewObject.transform.localScale = Vector3.one;

        view.Initialize(model, targetId);
        _upgradeViews.Add(view);

        view.OnUpgradeButtonClicked += UpgradeButtonClickHandler;
        OnCollectorUpgrade += view.UpgradePurchasedHandler;
    }

    private void UpgradeButtonClickHandler(UpgradeEventArgs args)
    {
        List<IUpgradeable> upgradeableCollectors = upgradeables.FindAll(c => c is CollectorModel).ToList();

        foreach (var item in upgradeableCollectors)
        {
            CollectorModel collector = item as CollectorModel;

            if (args.UpgradeModel.TargetId == collector.CollectorGUID)
            {
                collector.Upgrade(args.UpgradeModel);
                collector.OnCollectorUpgrade += CollectorUpgradedHandler;
            }
        }
    }

    private void CollectorUpgradedHandler(CollectorEventArgs args)
    {
        OnCollectorUpgrade?.Invoke(args);
    }

    public void AddToModels(UpgradeModel model)
    {
        _upgradeModels.Add(model);
    }

    public void AddToViews(UpgradeView view)
    {
        _upgradeViews.Add(view);
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
        CollectorModel.OnCollectorRegister += RegisterCollector;
    }

    private void UnSubscribe()
    {
        CollectorModel.OnCollectorRegister -= RegisterCollector;
    }
}
