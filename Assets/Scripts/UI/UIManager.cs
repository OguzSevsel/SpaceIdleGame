using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

public class UIManager : MonoBehaviour
{
    public List<ColonyPanel> ColonyPanels;

    private void SubscribeToEvents()
    {
        EventBus.Subscribe<CollectorRequestedEvent>(OnCollectorRequested);
        EventBus.Subscribe<CollectorUpgradeRequestedEvent>(OnCollectorUpgradeRequested);
        EventBus.Subscribe<CollectorLevelAmountRequestedEvent>(OnCollectorLevelAmountRequested);
    }

    private void OnCollectorLevelAmountRequested(CollectorLevelAmountRequestedEvent e)
    {
        EventBus.Publish(new CollectorLevelAmountStartedEvent() {  amount = e.amount});
    }

    private void OnCollectorUpgradeRequested(CollectorUpgradeRequestedEvent e)
    {
        EventBus.Publish(new CollectorUpgradeStartedEvent()
        {
            collectorType = e.collectorType,
            colonyType = e.colonyType,
            upgradeCategory = UpgradeCategory.Local,
            upgradeType = UpgradeType.CollectorAmount,
            upgradeMultiplier = 1.1d
        });
    }

    private void OnCollectorRequested(CollectorRequestedEvent e)
    {
        EventBus.Publish(new CollectorStartedEvent() { collectorType = e.collectorType, colonyType = e.colonyType });
    }

    private void UnSubscribeFromEvents()
    {
        EventBus.Unsubscribe<CollectorRequestedEvent>(OnCollectorRequested);
        EventBus.Unsubscribe<CollectorUpgradeRequestedEvent>(OnCollectorUpgradeRequested);
        EventBus.Unsubscribe<CollectorLevelAmountRequestedEvent>(OnCollectorLevelAmountRequested);
    }

    private void OnEnable()
    {
        SubscribeToEvents();
    }

    private void OnDisable()
    {
        UnSubscribeFromEvents();
    }
}
