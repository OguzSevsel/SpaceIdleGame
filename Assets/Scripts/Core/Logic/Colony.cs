using Mono.Cecil.Cil;
using System.Collections.Generic;
using UnityEngine;

public class Colony : MonoBehaviour
{
    public ColonySO colonyData;
    public List<Collector> Collectors;
    public double MoneyAmount = 0d;



    public bool CheckIfColonyHasEnoughResources(List<CostResource> costResources)
    {
        bool isEnoughResources = true;

        foreach (CostResource costResource in costResources)
        {
            Collector collector = Collectors.Find(c => c.CollectorData.GeneratedResource.resourceType == costResource.Resource.resourceType);

            if (collector != null)
            {
                if (collector.GetResourceAmount() < costResource.GetCostAmount())
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
                    if (MoneyAmount < costResource.GetCostAmount())
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
            Collector collector = Collectors.Find(c => c.CollectorData.GeneratedResource.resourceType == costResource.Resource.resourceType);

            if (collector != null)
            {
                collector.SetResourceAmount(collector.GetResourceAmount() - costResource.GetCostAmount());
            }
            else
            {
                if (costResource.Resource.resourceType is ResourceType.Money)
                {
                    MoneyAmount = MoneyAmount - costResource.GetCostAmount();
                }
            }
        }
    }
}
