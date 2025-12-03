using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "ShipSO", menuName = "ScriptableObjects/ShipSO")]
public class ShipSO : ScriptableObject
{
    [SerializeField] private string _shipName;
    [SerializeField] private Sprite _sprite;
    [SerializeField] private double _damage;
    [SerializeField] private double _health;
    [SerializeField] private float _speed;
    [SerializeField] private int _bulletCount;
    [SerializeField] private int _bulletCapacity;

    public float Speed => _speed;
}
