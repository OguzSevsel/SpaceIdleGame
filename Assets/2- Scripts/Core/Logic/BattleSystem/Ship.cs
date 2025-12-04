using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ship : MonoBehaviour, IDamageable
{
    public ShipSO DataSO;
    public float CurrentHealth { get; set; }
    public GameObject BulletObjectInstance { get; set; }
    public bool CanShoot { get; set; } = false;
    public float BulletIntervalTimer { get; set; } = 0f;

    public List<GameObject> _bullets;

    public EnemyShip Target;
    public bool IsShooting = false;
    public float HitRadius = 0.3f;
    private Vector3 _lastPos;

    public void Start()
    {
        ShipManager.Instance.RegisterShip(this);
        SpatialGrid.Instance.RegisterShip(this);
        _lastPos = transform.position;
        _bullets = new List<GameObject>();
        CreateBulletPool(5);
        CurrentHealth = DataSO.Health;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, HitRadius);
    }

    public void Tick()
    {
        HandleFireTimer();
        SpatialGrid.Instance.UpdateShipCell(this, _lastPos);
        _lastPos = transform.position;

        if (CanShoot)
        {
            if (this is EnemyShip)
            {
                var target = GetTarget();

                if (target != null)
                {
                    Shoot(target);
                    CanShoot = false;
                    BulletIntervalTimer = DataSO.FireInterval;
                }
            }
            else
            {
                if (IsShooting)
                {
                    if (Target != null)
                    {
                        Shoot(Target);
                        CanShoot = false;
                        IsShooting = false;
                        BulletIntervalTimer = DataSO.FireInterval;
                    }
                }
                else
                {
                    IsShooting = false;
                }
            }
        }
    }

    private Ship GetTarget()
    {
        var ships = ShipManager.Instance.GetTargetsForBullet(isPlayerBullet: false);

        if (this is PlayerShip)
        {
            ships = ShipManager.Instance.GetTargetsForBullet(isPlayerBullet: true);
        }
        else
        {
            ships = ShipManager.Instance.GetTargetsForBullet(isPlayerBullet: false);
        }

        return ships.FirstOrDefault();
    }

    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        
        if (CurrentHealth <= 0)
        {
            _bullets.Clear();
            BulletObjectInstance = null;
            Destroy(gameObject);
        }
    }

    private void HandleFireTimer()
    {
        if (!CanShoot)
        {
            BulletIntervalTimer -= Time.deltaTime;
            if (BulletIntervalTimer <= 0f)
            {
                CanShoot = true;
                BulletIntervalTimer = 0f;
            }
        }
    }

    private void CreateBulletPool(int count)
    {
        for (int i = 0; i < count; i++)
        {
            BulletObjectInstance = Instantiate(DataSO.BulletObject, transform.position, Quaternion.identity);
            BulletObjectInstance.SetActive(false);
            _bullets.Add(BulletObjectInstance);
            BulletManager.Instance.Register(BulletObjectInstance.GetComponent<Bullet>());
        }
    }

    public void Shoot(Ship target)
    {
        foreach (var bullet in _bullets)
        {
            if (!bullet.activeInHierarchy)
            {
                bullet.transform.position = this.transform.position;
                bullet.SetActive(true);

                if (this is PlayerShip)
                {
                    bullet.GetComponent<Bullet>().Shoot(target, DataSO.Damage, true);
                }
                else
                {
                    bullet.GetComponent<Bullet>().Shoot(target, DataSO.Damage, false);
                }
                return;
            }
        }
    }

    private void OnDestroy()
    {
        ShipManager.Instance.UnregisterShip(this);
        SpatialGrid.Instance.UnregisterShip(this);
    }

    private void OnDisable()
    {
        ShipManager.Instance.UnregisterShip(this);

        if (SpatialGrid.Instance != null)
            SpatialGrid.Instance.UnregisterShip(this);
    }
}
