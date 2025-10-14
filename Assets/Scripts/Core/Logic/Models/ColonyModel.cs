using System;
using System.Collections.Generic;
using UnityEngine;

public class ColonyModel : MonoBehaviour
{
    public ColonySO colonyData;
    public List<CollectorModel> Collectors;
    public List<Resource> Resources;

    public event Action<Resource, int>  OnResourceAdded;

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

    public void SpendResources(List<CostResource> costResources)
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
                        newResource.SpendResource(resource.GetCostAmount());
                    }
                    else
                    {
                        Debug.LogWarning($"Cant spend resources because there is not enough of  {resource.Resource.resourceType}");
                    }
                }
            }
            else
            {
                if (resource.GetCostAmount() < GlobalResourceManager.Instance.MoneyAmount)
                {
                    GlobalResourceManager.Instance.SpendMoney(resource.GetCostAmount());
                }
                else
                {
                    Debug.LogWarning($"Cant spend resources because there is not enough of {resource.Resource.resourceType}");
                }
            }
        }
    }
}
