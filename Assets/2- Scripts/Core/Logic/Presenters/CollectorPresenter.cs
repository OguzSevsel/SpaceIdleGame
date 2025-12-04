using System;
using UnityEngine;

public class CollectorPresenter
{
    private CollectorModel _collectorModel;
    private CollectorView _collectorView;
    private static event Action<CollectorEventArgs> OnCollectorCollect;

    public CollectorPresenter(CollectorModel model, CollectorView view)
    {
        _collectorModel = model;
        _collectorView = view;

        //View Events
        _collectorView.Initialize(_collectorModel);
        _collectorView.OnUpgradeAmountChanged += UpgradeAmountChangedHandler;
        _collectorView.OnCollectorCollect += CollectorCollectHandler;
        _collectorView.OnCollectorUpgrade += CollectorUpgradeHandler;
        

        //Model Events
        _collectorModel.OnCollectorUpgrade += CollectorUpgradedHandler;
        _collectorModel.OnCollectorUpgradeAmount += CollectorUpgradeAmountChangedHandler;
        _collectorModel.OnProgressBarUpdate += ProgressBarUpdatedHandler;
    }
    
    private void CollectorUpgradeHandler()
    {
        _collectorModel.Upgrade(_collectorModel.LevelUpgrade);
    }

    private void CollectorCollectHandler()
    {
        _collectorModel.Collect();
    }

    private void ProgressBarUpdatedHandler(ProgressBarUpdateArgs args)
    {
        _collectorView.ProgressBarUpdate(args);
    }

    private void CollectorUpgradeAmountChangedHandler(CollectorEventArgs args)
    {
        _collectorView.CollectorUpgradeAmountChanged(args);
    }

    private void CollectorUpgradedHandler(CollectorEventArgs args)
    {
        _collectorView.CollectorUpgradedHandler(args);
    }

    private void UpgradeAmountChangedHandler(int amount)
    {
        _collectorModel.ChangeUpgradeAmount(amount);
    }
}
