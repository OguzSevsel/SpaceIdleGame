using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UpgradeModel 
{
    public UpgradeSO upgradeSO;
    public List<CostResource> upgradeCosts;
    public string upgradeDescription;
    private string _targetId;
    [HideInInspector] public string TargetId => GetTargetID();
    [SerializeField] private GameObject target;
    
    private string GetTargetID()
    {
        if (target.TryGetComponent<CollectorModel>(out CollectorModel collector))
        {
            _targetId = collector.CollectorGUID;
        }
        else
        {
            _targetId = "There is an issue";
        }
        return _targetId;
    }
}
