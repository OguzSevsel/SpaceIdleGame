using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CollectorData
{
    public CollectorSO DataSO;
    public List<CostResource> CostResources;

    [Header("Collection Rates")]
    [SerializeField][Range(0f, 2f)] private double _collectionRate;
    [SerializeField][Range(0f, 2f)] private double _collectionRateMultiplier;

    [Header("Speed Rates")]
    [SerializeField][Range(0f, 2f)] private double _speed;
    [SerializeField][Range(0f, 1f)] private double _speedMultiplier;

    [Header("Collector Level Rates")]
    [SerializeField] private int _level = 1;
    [SerializeField] private int _levelIncrement = 1;

    public CollectorData()
    {

    }

    public CollectorData(CollectorSO SO, List<CostResource> costResources, double collectionRate, double collectionRateMult, double speed, double speedMult, int level, int levelIncrement)
    {
        this.DataSO = SO;
        this.CostResources = costResources;
        this._collectionRate = collectionRate;
        this._collectionRateMultiplier = collectionRateMult;
        this._speed = speed;
        this._speedMultiplier = speedMult;
        this._level = level;
        this._levelIncrement = levelIncrement;
    }

    public double GetSpeed() { return _speed; }
    public void SetSpeed(double value) { _speed = value; }


    public double GetSpeedMultiplier() { return _speedMultiplier; }
    public void SetSpeedMultiplier(double value) { _speedMultiplier = value; }


    public string GetResourceUnit() { return DataSO.GeneratedResource.ResourceUnit; }
    public void SetResourceUnit(string value) { DataSO.GeneratedResource.ResourceUnit = value; }


    public double GetCollectionRateMultiplier() { return _collectionRateMultiplier; }
    public void SetCollectionRateMultiplier(double value) { _collectionRateMultiplier = value; }


    public double GetCollectionRate() { return _collectionRate; }
    public void SetCollectionRate(double value) { _collectionRate = value; }


    public int GetLevel() { return _level; }
    public void SetLevel(int value) { _level = value; }


    public int GetLevelIncrement() { return _levelIncrement; }
    public void SetLevelIncrement(int value) { _levelIncrement = value; }
}   