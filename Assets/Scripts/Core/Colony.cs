using Mono.Cecil;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Colony : MonoBehaviour
{
    public ColonyType ColonyType;
    public List<Collector> Collectors;
    public CostResource Money;
    public UpgradeMultipliers Multiplier;

    private bool _isShowingSellUI = false;

    private void OnEnable()
    {
        EventBus.Subscribe<CollectorStartedEvent>(OnCollectorStarted);
        EventBus.Subscribe<SellUIStartedEvent>(OnSellUIStarted);
    }

    private void OnSellUIStarted(SellUIStartedEvent e)
    {
        if (e.TabButton.gameObject.name == "Button_Convert")
        {
            _isShowingSellUI = true;
        }
        else
        {
            _isShowingSellUI = false;
        }
    }

    private void Update()
    {
        if (_isShowingSellUI)
        {
            EventBus.Publish(new SellUIChangeEvent()
            {
                ColonyType = ColonyType.ColonyTypeName,
                Collectors = this.Collectors,
            });
        }
    }

    private void OnCollectorStarted(CollectorStartedEvent e)
    {
        if (e.ColonyType == ColonyType.ColonyTypeName)
        {
            Collector foundCollector = GetCollector(e.CollectorType);
            if (foundCollector != null)
            {
                foundCollector.Collect();
            }
        }
    }

    public bool CheckIfColonyHasEnoughResourcesForUpgrade(CostResource resourceType, double resourceAmount)
    {
        if (resourceType.resourceType == Money.resourceType)
        {
            if (Money.amount >= resourceAmount)
            {
                Money.amount = Money.amount - resourceAmount;
                return true;
            }
            return false;
        }
        else
        {
            foreach (Collector collector in Collectors)
            {
                foreach (CostResource resource in collector.CollectorType.CostResourcesToUpgrade)
                {
                    if (resource.resourceType == resourceType.resourceType)
                    {
                        if (collector.ResourceAmount >= resource.amount)
                        {
                            collector.ResourceAmount -= resource.amount;
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }

    private Collector GetCollector(CollectorTypeEnum collector)
    {
        Collector foundCollector = null;

        switch (collector)
        {
            case CollectorTypeEnum.EnergyCollector:
                foundCollector = Collectors.Find(c => c.CollectorType.CollectorTypeName == CollectorTypeEnum.EnergyCollector);
                break;
            case CollectorTypeEnum.IronCollector:
                foundCollector = Collectors.Find(c => c.CollectorType.CollectorTypeName == CollectorTypeEnum.IronCollector);
                break;
            case CollectorTypeEnum.CopperCollector:
                foundCollector = Collectors.Find(c => c.CollectorType.CollectorTypeName == CollectorTypeEnum.CopperCollector);
                break;
            case CollectorTypeEnum.SiliconCollector:
                foundCollector = Collectors.Find(c => c.CollectorType.CollectorTypeName == CollectorTypeEnum.SiliconCollector);
                break;
            case CollectorTypeEnum.LimeStoneCollector:
                foundCollector = Collectors.Find(c => c.CollectorType.CollectorTypeName == CollectorTypeEnum.LimeStoneCollector);
                break;
            case CollectorTypeEnum.GoldCollector:
                foundCollector = Collectors.Find(c => c.CollectorType.CollectorTypeName == CollectorTypeEnum.GoldCollector);
                break;
            case CollectorTypeEnum.AluminiumCollector:
                foundCollector = Collectors.Find(c => c.CollectorType.CollectorTypeName == CollectorTypeEnum.AluminiumCollector);
                break;
            case CollectorTypeEnum.CarbonCollector:
                foundCollector = Collectors.Find(c => c.CollectorType.CollectorTypeName == CollectorTypeEnum.CarbonCollector);
                break;
            case CollectorTypeEnum.DiamondCollector:
                foundCollector = Collectors.Find(c => c.CollectorType.CollectorTypeName == CollectorTypeEnum.DiamondCollector);
                break;
            default:
                break;
        }

        return foundCollector;
    }
}
