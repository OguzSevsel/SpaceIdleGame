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

    public List<UpgradeModel> GetUpgrades()
    {
        var upgradeManager = GetComponentInChildren<UpgradePresenter>();
        return upgradeManager.GetUpgrades();
    }

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
    /// Check single resource that colony has enough of.
    /// </summary>
    /// <param name="costResources"> This is the cost, which function checks </param>
    /// <returns></returns>
    public bool CheckSingleResource(CostResource costResource)
    {
        Resource resource = Resources.Find(r => r.ResourceSO.resourceType == costResource.Resource.resourceType);
        bool isEnough = false;

        if (resource != null)
        {
            if (resource.CheckEnoughResource(costResource.GetCostAmount()))
            {
                isEnough = true;
            }
        }
        else
        {
            if (costResource.Resource.resourceType == ResourceType.Money)
            {
                if (GlobalResourceManager.Instance.MoneyAmount >= costResource.GetCostAmount())
                {
                    isEnough = true;
                }
            }
        }
        return isEnough;
    }

    /// <summary>
    /// Checking Resources that colony has enough of them.
    /// </summary>
    /// <param name="costResources"> This is the cost, which function checks </param>
    /// <returns></returns>
    public bool CheckResources(List<CostResource> costResources)
    {
        bool isEnough = true;

        foreach (CostResource resource in costResources)
        {
            if (resource.Resource.resourceType != ResourceType.Money)
            {
                Resource newResource = Resources.Find(r => r.ResourceSO.resourceType == resource.Resource.resourceType);

                if (newResource != null)
                {
                    if (!newResource.CheckEnoughResource(resource.GetCostAmount()))
                    {
                        isEnough = false;
                    }
                }
            }
            else
            {
                if (resource.GetCostAmount() > GlobalResourceManager.Instance.MoneyAmount)
                {
                    isEnough = false;
                }
            }
        }
        return isEnough;
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
        bool isEnough = CheckResources(costResources);

        if (isEnough)
        {
            foreach (CostResource costResource in costResources)
            {
                if (costResource.Resource.resourceType != ResourceType.Money)
                {
                    Resource resource = Resources.Find(r => r.ResourceSO == costResource.Resource);

                    if (resource != null)
                    {
                        resource.SpendResource(costResource.GetCostAmount());
                        OnResourceSpend?.Invoke(resource, Resources.IndexOf(resource));
                    }
                }
                else
                {
                    GlobalResourceManager.Instance.SpendMoney(costResource.GetCostAmount());
                }
            }
        }
    }
}
