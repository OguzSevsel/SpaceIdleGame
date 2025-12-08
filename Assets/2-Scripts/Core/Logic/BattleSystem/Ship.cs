using TMPro;
using UnityEngine;

public class Ship : MonoBehaviour, IDamageable
{
    public ShipSO DataSO;
    public float CurrentHealth { get; private set; }
    public GameObject BulletObjectInstance { get; private set; }
    public bool CanAction { get; set; } = false;
    public float ActionIntervalTimer { get; set; } = 0f;
    [HideInInspector] public EnemyShip Target;
    [HideInInspector] public CursorField _cursorField;
    [SerializeField] private TextMeshProUGUI healthText;
    public SpriteRenderer healthSpriteRenderer;

    #region Start

    public virtual void Start()
    {
        ShipManager.Instance.RegisterShip(this);
        CreateBulletPool(10);
        CurrentHealth = DataSO.Health;
        if (healthSpriteRenderer != null)
        {
            healthSpriteRenderer.enabled = false;
        }
    }

    private void CreateBulletPool(int count)
    {
        if (DataSO.ShipType == ShipType.Dps || DataSO.ShipType == ShipType.Tank)
        {
            PoolManager.Instance.CreatePool(DataSO.BulletObjectPrefab, this.transform, Quaternion.identity, count);
        }
    }

    #endregion

    #region Update

    public virtual void Tick()
    {
        HandleActionTimer();
    }

    private void HandleActionTimer()
    {
        if (!CanAction)
        {
            ActionIntervalTimer -= Time.deltaTime;
            if (ActionIntervalTimer <= 0f)
            {
                CanAction = true;
                ActionIntervalTimer = 0f;
            }
        }
    }

    #endregion

    #region Actions

    public void Shoot(Ship target)
    {
        if (this.gameObject != null)
        {
            BulletObjectInstance = PoolManager.Instance.Get(this.gameObject);
            BulletObjectInstance.transform.position = this.transform.position;

            if (this is PlayerShip)
            {
                BulletManager.Instance.Register(BulletObjectInstance.GetComponent<Bullet>(), true);
            }
            else
            {
                BulletManager.Instance.Register(BulletObjectInstance.GetComponent<Bullet>(), false);
            }
        }

        if (BulletObjectInstance != null)
        {
            if (this is PlayerShip)
            {
                BulletObjectInstance.GetComponent<Bullet>().Shoot(target, DataSO.ActionAmount, fromPlayer: true);
            }
            else
            {
                BulletObjectInstance.GetComponent<Bullet>().Shoot(target, DataSO.ActionAmount);
            }
        }

        return;
    }

    public void Heal()
    {
        var shipsToHeal = ShipManager.Instance.GetTargetForHealing(this);

        if (shipsToHeal != null)
        {
            foreach (var ship in shipsToHeal)
            {
                ship.IncreaseHealth(this.DataSO.ActionAmount);
            }
        }
    }

    #endregion

    #region Utilities

    public Ship GetTarget()
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
        healthText.text = $"{CurrentHealth}/{DataSO.Health}";
        ExplosionManager.Instance.CreateExplosion(this.transform.position);
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

    public void IncreaseHealth(float amount)
    {
        CurrentHealth += amount;
        healthText.text = $"{CurrentHealth} / {DataSO.Health}";
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, DataSO.HitRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, DataSO.HealRadius);
    }

    #endregion
}
