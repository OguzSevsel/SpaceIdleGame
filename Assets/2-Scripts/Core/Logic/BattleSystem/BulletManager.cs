using System;
using System.Collections.Generic;
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
    public NativeArray<int> BulletHitShip; // -1 = no hit, otherwise ship index

    public void Execute(int index)
    {
        PhysicsBody b = Bullets[index];
        int hit = -1;
        for (int s = 0; s < Ships.Length; s++)
        {
            PhysicsBody sh = Ships[s];
            float distSq = math.distancesq(b.Position, sh.Position);
            float radSum = b.Radius + sh.Radius;
            if (distSq <= radSum * radSum)
            {
                hit = s;
                break;
            }
        }
        BulletHitShip[index] = hit;
    }
}

public struct PhysicsBody
{
    public float3 Position;
    public float Radius;
}

public class BulletManager : MonoBehaviour
{
    public static BulletManager Instance;
    public List<Bullet> EnemyBullets;
    public List<Bullet> PlayerBullets;

    // persistent native arrays to reduce churn
    private NativeArray<PhysicsBody> _nativeBullets;
    private NativeArray<PhysicsBody> _nativeShips;
    private NativeArray<int> _bulletHitResults;

    private int _nativeBulletsCap = 0;
    private int _nativeShipsCap = 0;

    private void Awake()
    {
        Instance = this;
        EnemyBullets = new List<Bullet>();
        PlayerBullets = new List<Bullet>();
    }

    private void OnDestroy()
    {
        if (_nativeBullets.IsCreated) _nativeBullets.Dispose();
        if (_nativeShips.IsCreated) _nativeShips.Dispose();
        if (_bulletHitResults.IsCreated) _bulletHitResults.Dispose();
    }

    private void EnsureBulletCapacity(int required)
    {
        if (_nativeBulletsCap >= required) return;
        if (_nativeBullets.IsCreated) _nativeBullets.Dispose();
        if (_bulletHitResults.IsCreated) _bulletHitResults.Dispose();

        int newCap = Math.Max(required, Mathf.CeilToInt(required * 1.25f));
        _nativeBullets = new NativeArray<PhysicsBody>(newCap, Allocator.Persistent);
        _bulletHitResults = new NativeArray<int>(newCap, Allocator.Persistent);
        _nativeBulletsCap = newCap;
    }

    private void EnsureShipCapacity(int required)
    {
        if (_nativeShipsCap >= required) return;
        if (_nativeShips.IsCreated) _nativeShips.Dispose();

        int newCap = Math.Max(required, Mathf.CeilToInt(required * 1.25f));
        _nativeShips = new NativeArray<PhysicsBody>(newCap, Allocator.Persistent);
        _nativeShipsCap = newCap;
    }

    public void Register(Bullet bullet, bool isPlayerBullet)
    {
        if (isPlayerBullet)
        {
            if (!PlayerBullets.Contains(bullet)) PlayerBullets.Add(bullet);
        }
        else
        {
            if (!EnemyBullets.Contains(bullet)) EnemyBullets.Add(bullet);
        }
    }

    public void UnRegister(Bullet bullet, bool isPlayerBullet)
    {
        // main-thread-only removal (safe)
        if (isPlayerBullet)
        {
            if (PlayerBullets.Contains(bullet)) PlayerBullets.Remove(bullet);
        }
        else
        {
            if (EnemyBullets.Contains(bullet)) EnemyBullets.Remove(bullet);
        }
    }

    private void Update()
    {
        if (!Application.isPlaying) return;

        // 1) Move bullets on main thread first (use Tick() - safe)
        // we iterate backwards to remove nulls or inactive bullets if any
        for (int i = EnemyBullets.Count - 1; i >= 0; i--)
        {
            var b = EnemyBullets[i];
            if (b == null) { EnemyBullets.RemoveAt(i); continue; }
            if (b.IsActive)
            {
                b.Tick(); // move on main thread before collision snapshot
            }
            else
            {
                // if bullet is already inactive, remove it now (main thread)
                EnemyBullets.RemoveAt(i);
            }
        }

        for (int i = PlayerBullets.Count - 1; i >= 0; i--)
        {
            var b = PlayerBullets[i];
            if (b == null) { PlayerBullets.RemoveAt(i); continue; }
            if (b.IsActive)
            {
                b.Tick();
            }
            else
            {
                PlayerBullets.RemoveAt(i);
            }
        }

        // 2) Run collisions (player bullets -> enemy ships, enemy bullets -> player ships)
        if (PlayerBullets.Count > 0 && ShipManager.Instance.EnemyShips.Count > 0)
        {
            RunCollisionJob(PlayerBullets, ShipManager.Instance.EnemyShips, isPlayerBullet: true);
        }

        if (EnemyBullets.Count > 0 && ShipManager.Instance.PlayerShips.Count > 0)
        {
            RunCollisionJob(EnemyBullets, ShipManager.Instance.PlayerShips, isPlayerBullet: false);
        }
    }

    // bullets: list of Bullet (either PlayerBullets or EnemyBullets)
    // ships: list of Ship (EnemyShip or PlayerShip)
    private void RunCollisionJob<TShip>(List<Bullet> bullets, List<TShip> ships, bool isPlayerBullet)
        where TShip : Ship
    {
        int bulletCount = bullets.Count;
        int shipCount = ships.Count;
        if (bulletCount == 0 || shipCount == 0) return;

        // snapshot lists (prevent changes affecting this run)
        Bullet[] bulletSnapshot = new Bullet[bulletCount];
        for (int i = 0; i < bulletCount; i++) bulletSnapshot[i] = bullets[i];

        TShip[] shipSnapshot = new TShip[shipCount];
        for (int s = 0; s < shipCount; s++) shipSnapshot[s] = ships[s];

        // ensure native capacities
        EnsureBulletCapacity(bulletCount);
        EnsureShipCapacity(shipCount);

        // fill native arrays for the used range
        for (int i = 0; i < bulletCount; i++)
        {
            var b = bulletSnapshot[i];
            _nativeBullets[i] = new PhysicsBody { Position = b.transform.position, Radius = b.HitRadius };
            _bulletHitResults[i] = -1;
        }

        for (int s = 0; s < shipCount; s++)
        {
            var sh = shipSnapshot[s];
            _nativeShips[s] = new PhysicsBody { Position = sh.transform.position, Radius = sh.DataSO.HitRadius };
        }

        // schedule job using only the valid ranges (we use the arrays but only indices < count are used)
        var job = new CollisionJob
        {
            Bullets = _nativeBullets,
            Ships = _nativeShips,
            BulletHitShip = _bulletHitResults
        };

        JobHandle handle = job.Schedule(bulletCount, 256);
        handle.Complete();

        // apply results on main thread (backwards to avoid index-shift issues if we remove)
        for (int i = bulletCount - 1; i >= 0; i--)
        {
            int hit = _bulletHitResults[i]; // -1 = no hit
            if (hit == -1) continue;

            if (hit < 0 || hit >= shipSnapshot.Length) continue; // safety

            var bullet = bulletSnapshot[i];
            var ship = shipSnapshot[hit];

            if (bullet == null || ship == null) continue;

            // apply damage and deactivate on main thread ONLY
            ship.TakeDamage(bullet.Damage);

            // deactivate (this will call UnRegister inside Bullet.Deactivate which we want to avoid during job)
            // To keep behavior consistent, call Deactivate first then UnRegister here:
            bullet.Deactivate();
            UnRegister(bullet, isPlayerBullet);
        }

        // done. do NOT dispose the persistent arrays here
    }
}
