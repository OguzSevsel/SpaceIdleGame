using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    public static BulletManager Instance;
    public List<Bullet> bullets;
    public List<Bullet> DestroyedBullets;

    private void Awake()
    {
        Instance = this;
        bullets = new List<Bullet>();
        DestroyedBullets = new List<Bullet>();
    }

    public void Register(Bullet b)
    {
        bullets.Add(b);
    }
    public void UnRegister(Bullet b)
    {
        DestroyedBullets.Add(b);
    }

    NativeArray<PhysicsBody> nativeBullets;
    NativeArray<PhysicsBody> ships;
    NativeArray<int> results;

    private void RunCollisionJob(List<Bullet> bulletsToCheck, List<Ship> targetShips)
    {
        int bulletCount = bulletsToCheck.Count;
        int shipCount = targetShips.Count;

        if (bulletCount == 0 || shipCount == 0) return;

        var nativeBullets = new NativeArray<PhysicsBody>(bulletCount, Allocator.TempJob);
        var nativeShips = new NativeArray<PhysicsBody>(shipCount, Allocator.TempJob);
        var results = new NativeArray<int>(bulletCount, Allocator.TempJob);

        for (int i = 0; i < bulletCount; i++)
        {
            nativeBullets[i] = new PhysicsBody
            {
                Position = bulletsToCheck[i].transform.position,
                Radius = bulletsToCheck[i].HitRadius
            };
        }

        for (int s = 0; s < shipCount; s++)
        {
            nativeShips[s] = new PhysicsBody
            {
                Position = targetShips[s].transform.position,
                Radius = 0.2f
            };
        }

        var job = new BulletVsShipJob
        {
            Bullets = nativeBullets,
            Ships = nativeShips,
            CollisionResults = results
        };

        JobHandle handle = job.Schedule(bulletCount, 32);
        handle.Complete();

        // Process results
        for (int i = 0; i < bulletCount; i++)
        {
            if (results[i] == 1)
            {
                bulletsToCheck[i].Deactivate();
            }
        }

        // Dispose
        nativeBullets.Dispose();
        nativeShips.Dispose();
        results.Dispose();
    }

    private void Update()
    {
        if (bullets.Count == 0) return;

        // Partition bullets by team
        List<Bullet> playerBullets = new List<Bullet>();
        List<Bullet> enemyBullets = new List<Bullet>();

        foreach (var b in bullets)
        {
            if (!b.IsActive) continue;

            if (b == null)
            {
                continue;
            }

            if (b.IsPlayerBullet) playerBullets.Add(b);
            else enemyBullets.Add(b);
        }

        // Run collision jobs
        RunCollisionJob(playerBullets, ShipManager.Instance.EnemyShips.ToList<Ship>());
        RunCollisionJob(enemyBullets, ShipManager.Instance.PlayerShips.ToList<Ship>());

        // Move bullets
        foreach (var b in bullets)
        {
            if (b.IsActive) b.Tick();
        }

        // Remove destroyed bullets
        foreach (var b in DestroyedBullets)
            bullets.Remove(b);

        DestroyedBullets.Clear();
    }
}
