using System;
using System.Collections.Generic;
using UnityEngine;

public class Colony : MonoBehaviour
{
    [Header("Colony Scriptable Object")]
    public ColonySO colonyData;
    [Header("Collector List")]
    public List<Collector> Collectors;

    public static event Action<CostResourceEventArgs> OnCostResourcesSpend;
    public static event Action<ResourceEventArgs> OnMoneySpend;
    


    #region Sell Functions

    public bool CheckIfColonyHasEnoughResources(List<CostResource> costResources)
    {
        bool isEnoughResources = true;

        foreach (CostResource costResource in costResources)
        {
            Collector collector = Collectors.Find(c => c.Data.DataSO.GeneratedResource.resourceType == costResource.Resource.resourceType);

            if (collector != null)
            {
                if (collector.Data.GetResourceAmount() < costResource.GetCostAmount())
                {
                    isEnoughResources = false;
                }
            }
            else
            {
                if (costResource.Resource.resourceType is not ResourceType.Money)
                {
                    isEnoughResources = false;
                }
                else
                {
                    if (GlobalResourceManager.Instance.MoneyAmount < costResource.GetCostAmount())
                    {
                        isEnoughResources = false;
                    }
                }
            }
        }

        return isEnoughResources;
    }

    public void SpendResources(List<CostResource> costResources)
    {
        foreach (CostResource costResource in costResources)
        {
            Collector collector = Collectors.Find(c => c.Data.DataSO.GeneratedResource.resourceType == costResource.Resource.resourceType);

            if (collector != null)
            {
                collector.Data.SetResourceAmount(collector.Data.GetResourceAmount() - costResource.GetCostAmount());
                OnCostResourcesSpend?.Invoke(new CostResourceEventArgs 
                { 
                    CostResource = costResource,
                    Collector = collector
                });
            }
            else
            {
                if (costResource.Resource.resourceType is ResourceType.Money)
                {
                    GlobalResourceManager.Instance.SpendMoney(costResource.GetCostAmount());
                }
            }
        }
    }

    #endregion
}
