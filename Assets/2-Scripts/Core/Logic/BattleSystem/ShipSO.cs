using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "ShipSO", menuName = "ScriptableObjects/ShipSO")]
public class ShipSO : ScriptableObject
{
    [SerializeField] private string _shipName;
    [SerializeField] private Sprite _sprite;
    [SerializeField] private GameObject _bulletObject;
    [SerializeField] private float _damage;
    [SerializeField] private float _health;
    [SerializeField] private float _fireInterval;
    [SerializeField] private int _bulletCount;
    [SerializeField] private int _bulletCapacity;
    [SerializeField] private float _hitRadius;

    public GameObject BulletObject => _bulletObject;
    public float FireInterval => _fireInterval;
    public float Health => _health;
    public float Damage => _damage;
    public float HitRadius => _hitRadius;
}
