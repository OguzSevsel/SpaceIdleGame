using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
public struct BulletVsShipJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<PhysicsBody> Bullets;
    [ReadOnly] public NativeArray<PhysicsBody> Ships;
    public NativeArray<int> CollisionResults; // 0 = no hit, 1 = hit

    public void Execute(int i)
    {
        PhysicsBody bullet = Bullets[i];

        for (int s = 0; s < Ships.Length; s++)
        {
            PhysicsBody ship = Ships[s];
            float distSq = math.distancesq(bullet.Position, ship.Position);
            float radSum = bullet.Radius + ship.Radius;

            if (distSq <= radSum * radSum)
            {
                CollisionResults[i] = 1;
                return;
            }
        }

        CollisionResults[i] = 0;
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

    private void OnDestroy()
    {
        BulletManager.Instance.UnRegister(this);
    }
}
