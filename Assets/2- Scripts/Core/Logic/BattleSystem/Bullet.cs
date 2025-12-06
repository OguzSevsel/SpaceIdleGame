using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
public struct CollisionJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<PhysicsBody> Bullets;
    [ReadOnly] public NativeArray<PhysicsBody> Ships;

    public NativeArray<int> BulletsResults; // 0 = no hit, 1 = hit
    public NativeArray<int> ShipResults; // 0 = no hit, 1 = hit

    public void Execute(int i)
    {
        PhysicsBody bullet = Bullets[i];

        for (int s = 0; s < Ships.Length; s++)
        {
            PhysicsBody ship = Ships[s];

            if (CircleCollision(bullet.Position, bullet.Radius, ship.Position, ship.Radius))
            {
                BulletsResults[i] = 1;
                ShipResults[s] = 1;
                return;
            }
        }
    }

    private bool CircleCollision(float3 position1, float radius1, float3 position2, float radius2)
    {
        float distSq = math.distancesq(position1, position2);
        float radSum = radius1 + radius2;
        return distSq <= radSum * radSum;
    }
}

public struct PhysicsBody
{
    public float3 Position;
    public float Radius;
}

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
        if (!gameObject.activeInHierarchy) return;

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
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, HitRadius);
    }

    private void OnDestroy()
    {
        BulletManager.Instance.UnRegister(this);
    }
}
