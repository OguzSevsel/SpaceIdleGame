using System;
using System.Collections.Generic;
using UnityEngine;

public class ColonyModel : MonoBehaviour
{
    public ColonySO colonyData;
    public List<CollectorModel> Collectors;
    public List<Resource> Resources;

    public event Action<Resource, int>  OnResourceAdded;
    public event Action<Resource, int>  OnResourceSpend;

    /// <summary>
    /// Sells a specified amount of resource and converts it to money.    
    /// </summary>
    /// <param name="resource"></param>
    /// <param name="amount"></param>
    public void SellResource(Resource resource, double amount)
    {
        foreach (Resource colonyResource in Resources)
        {
            if (resource == colonyResource)
            {
                resource.SpendResource(amount);
                GlobalResourceManager.Instance.AddMoney(amount * resource.SellRate);
            }
        }
    }


    /// <summary>
    /// Checking Resources that colony has enough of them.
    /// </summary>
    /// <param name="costResources"> This is the cost, which function checks </param>
    /// <returns></returns>
    public bool CheckResources(List<CostResource> costResources)
    {
        foreach (CostResource resource in costResources)
        {
            if (resource.Resource.resourceType != ResourceType.Money)
            {
                Resource newResource = Resources.Find(r => r.ResourceSO.resourceType == resource.Resource.resourceType);

                if (newResource != null)
                {
                    if (newResource.CheckEnoughResource(resource.GetCostAmount()))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (resource.GetCostAmount() < GlobalResourceManager.Instance.MoneyAmount)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        return false;
    }


    /// <summary>
    /// Add resource to the colony which then colony view update its resource text UI.
    /// </summary>
    /// <param name="resourceSO"></param>
    /// <param name="amount"></param>
    public void AddResource(ResourceSO resourceSO, double amount)
    {
        foreach (Resource resource in Resources)
        {
            if (resourceSO == resource.ResourceSO)
            {
                resource.AddResource(amount);
                OnResourceAdded?.Invoke(resource, Resources.IndexOf(resource));
            }
        }
    }

    /// <summary>
    /// Spend resources from the colony.
    /// </summary>
    /// <param name="costResources"></param>
    public void SpendResources(List<CostResource> costResources)
    {
        foreach (CostResource costResource in costResources)
        {
            if (costResource.Resource.resourceType != ResourceType.Money)
            {
                Resource resource = Resources.Find(r => r.ResourceSO == costResource.Resource);

                if (resource != null)
                {
                    if (resource.CheckEnoughResource(costResource.GetCostAmount()))
                    {
                        resource.SpendResource(costResource.GetCostAmount());
                        OnResourceSpend?.Invoke(resource, Resources.IndexOf(resource));
                    }
                    else
                    {
                        Debug.LogWarning($"Cant spend resources because there is not enough of  {costResource.Resource.resourceType}");
                    }
                }
            }
            else
            {
                if (costResource.GetCostAmount() < GlobalResourceManager.Instance.MoneyAmount)
                {
                    GlobalResourceManager.Instance.SpendMoney(costResource.GetCostAmount());
                }
                else
                {
                    Debug.LogWarning($"Cant spend resources because there is not enough of {costResource.Resource.resourceType}");
                }
            }
        }
    }
}
