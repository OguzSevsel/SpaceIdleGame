using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "ShipSO", menuName = "ScriptableObjects/ShipSO")]
public class ShipSO : ScriptableObject
{
    [SerializeField] private string _shipName;
    [SerializeField] private ShipType _shipType;
    [SerializeField] private Sprite _sprite;
    [SerializeField] private GameObject _bulletObjectPrefab;
    
    [SerializeField] private float _actionAmount;
    [SerializeField] private float _health;
    [SerializeField] private float _actionInterval;
    [SerializeField] private float _hitRadius;
    [SerializeField] private float _healRadius;

    public GameObject BulletObjectPrefab => _bulletObjectPrefab;
    public float ActionInterval => _actionInterval;
    public float Health => _health;
    public float ActionAmount => _actionAmount;
    public float HitRadius => _hitRadius;
    public float HealRadius => _healRadius;
    public ShipType ShipType => _shipType;
}

public enum ShipType
{
    Dps,
    Tank,
    Healer
}
