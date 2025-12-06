using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class Ship : MonoBehaviour, IDamageable
{
    public ShipSO DataSO;
    public float CurrentHealth { get; set; }
    public GameObject BulletObjectInstance { get; set; }
    public bool CanShoot { get; set; } = false;
    public float BulletIntervalTimer { get; set; } = 0f;
    public EnemyShip Target;
    public bool IsShooting = false;
    public CursorField _cursorField;

    public virtual void Start()
    {
        ShipManager.Instance.RegisterShip(this);
        CreateBulletPool(5);
        CurrentHealth = DataSO.Health;
    }

    public void Tick()
    {
        HandleFireTimer();

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

        if (ships.Count == 0) return null;

        int index = Random.Range(0, ships.Count);

        return ships[index];
    }

    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (this is EnemyShip)
        {
            _cursorField.UnregisterShip(this as EnemyShip);
        }

        ShipManager.Instance.UnregisterShip(this);
        PoolManager.Instance.DestroyPool(this.gameObject);
        GameObject.Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Bullet>(out Bullet bullet))
        {
            if (this is PlayerShip && bullet.IsPlayerBullet) return;
            if (this is EnemyShip && !bullet.IsPlayerBullet) return;

            TakeDamage(10f);
        }
    }

    private void CreateBulletPool(int count)
    {
        PoolManager.Instance.CreatePool(DataSO.BulletObject, this.transform, Quaternion.identity, count);

        foreach (var item in PoolManager.Instance.GetPool(this.gameObject))
        {
            if (this is PlayerShip)
            {
                BulletManager.Instance.Register(item.GetComponent<Bullet>(), true);
            }
            else
            {
                BulletManager.Instance.Register(item.GetComponent<Bullet>(), false);
            }
        }
    }

    public void Shoot(Ship target)
    {
        if (this.gameObject != null)
        {
            BulletObjectInstance = PoolManager.Instance.Get(this.gameObject);
            BulletObjectInstance.transform.position = this.transform.position;
        }

        if (BulletObjectInstance != null)
        {
            if (this is PlayerShip)
            {
                BulletObjectInstance.GetComponent<Bullet>().Shoot(target, DataSO.Damage, fromPlayer: true);
            }
            else
            {
                BulletObjectInstance.GetComponent<Bullet>().Shoot(target, DataSO.Damage);
            }
        }
        
        return;
    }
}
