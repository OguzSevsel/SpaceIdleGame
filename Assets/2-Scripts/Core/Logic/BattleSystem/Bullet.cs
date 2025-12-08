using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Ship TargetShip;
    public float Speed;
    public float Damage;
    public bool IsActive;
    public bool IsPlayerBullet;
    public Vector3 Velocity;
    public float HitRadius;

    public void Shoot(Ship target, float dmg, bool fromPlayer = false)
    {
        gameObject.SetActive(true);

        TargetShip = target;
        Damage = dmg;
        IsPlayerBullet = fromPlayer;

        Vector3 dir = (target.transform.position - transform.position).normalized;
        Velocity = dir * Speed;

        IsActive = true;
    }

    public void Tick()
    {
        if (!(transform.position.x >= CameraBounds.Left &&
            transform.position.x <= CameraBounds.Right &&
            transform.position.y >= CameraBounds.Bottom &&
            transform.position.y <= CameraBounds.Top))
        {
            Deactivate();
            return;
        }

        transform.position = transform.position + Velocity * Time.deltaTime;
    }

    public void Deactivate()
    {
        IsActive = false;
        gameObject.SetActive(false);
        BulletManager.Instance.UnRegister(this, IsPlayerBullet);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, HitRadius);
    }
}
