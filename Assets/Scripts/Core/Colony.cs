using System;
using System.Collections.Generic;
using UnityEngine;

public class Colony : MonoBehaviour
{
    public ColonyType ColonyType;
    public List<Collector> Collectors;

    private void OnEnable()
    {
        EventBus.Subscribe<CollectorStartedEvent>(OnCollectorStarted);
    }

    private void OnCollectorStarted(CollectorStartedEvent e)
    {
        if (e.colonyType == ColonyType.ColonyTypeName)
        {
            Collector foundCollector = GetCollector(e.collectorType);
            if (foundCollector != null)
            {
                foundCollector.Collect();
            }
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
