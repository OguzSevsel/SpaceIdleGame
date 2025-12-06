using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    public static BulletManager Instance;
    public List<Bullet> EnemyBullets;
    public List<Bullet> PlayerBullets;
    public List<Bullet> DestroyedBullets;

    private JobHandle collisionJobHandle;
    private CollisionJob collisionJob;
    private NativeArray<PhysicsBody> _nativeBullets;
    private NativeArray<PhysicsBody> _nativeShips;
    private NativeArray<int> _bulletsResults;
    private NativeArray<int> _shipResults;

    private void Awake()
    {
        Instance = this;
        EnemyBullets = new List<Bullet>();
        PlayerBullets = new List<Bullet>();
        DestroyedBullets = new List<Bullet>();
    }

    public void Register(Bullet bullet, bool isPlayerBullet)
    {
        if (isPlayerBullet)
        {
            PlayerBullets.Add(bullet);
        }
        else
        {
            EnemyBullets.Add(bullet);
        }
    }
    public void UnRegister(Bullet bullet)
    {
        DestroyedBullets.Add(bullet);
    }

    private void RunCollisionJob(List<Bullet> bullets, List<Ship> ships)
    {
        int bulletCount = bullets.Count;
        int shipCount = ships.Count;

        if (bulletCount == 0 || shipCount == 0) return;

        _nativeBullets = new NativeArray<PhysicsBody>(bulletCount, Allocator.TempJob);
        _bulletsResults = new NativeArray<int>(bulletCount, Allocator.TempJob);

        _nativeShips = new NativeArray<PhysicsBody>(shipCount, Allocator.TempJob);
        _shipResults = new NativeArray<int>(shipCount, Allocator.TempJob);

        for (int i = 0; i < bulletCount; i++)
        {
            _nativeBullets[i] = new PhysicsBody
            {
                Position = bullets[i].transform.position,
                Radius = bullets[i].HitRadius
            };
        }

        for (int s = 0; s < shipCount; s++)
        {
            _nativeShips[s] = new PhysicsBody
            {
                Position = ships[s].transform.position,
                Radius = ships[s].DataSO.HitRadius
            };
        }

        for (int i = 0; i < bulletCount; i++)
            _bulletsResults[i] = 0;

        for (int s = 0; s < shipCount; s++)
            _shipResults[s] = 0;

        var job = new CollisionJob
        {
            Bullets = _nativeBullets,
            BulletsResults = _bulletsResults,
            Ships = _nativeShips,
            ShipResults = _shipResults
        };

        JobHandle handle = job.Schedule(bulletCount, 32);
        handle.Complete();

        // Process results
        for (int i = 0; i < bulletCount; i++)
        {
            if (_bulletsResults[i] == 1)
            {
                bullets[i].Deactivate();
            }
        }

        for (int i = 0; i < shipCount; i++)
        {
            if (_shipResults[i] == 1)
            {
                ships[i].TakeDamage(10f);
            }
        }

        // Dispose
        _nativeBullets.Dispose();
        _nativeShips.Dispose();
        _bulletsResults.Dispose();
        _shipResults.Dispose();
    }
    private void LateUpdate()
    {
        
    }

    private void Update()
    {
        if (EnemyBullets.Count == 0 && PlayerBullets.Count == 0) return;

        foreach (var bullet in EnemyBullets)
        {
            if (bullet.IsActive)
            {
                bullet.Tick();
            }
        }

        foreach (var bullet in PlayerBullets)
        {
            if (bullet.IsActive)
            {
                bullet.Tick();
            }
        }

        RunCollisionJob(PlayerBullets, ShipManager.Instance.EnemyShips);
        RunCollisionJob(EnemyBullets, ShipManager.Instance.PlayerShips);

        // Remove destroyed bullets
        foreach (var bullet in DestroyedBullets)
        {
            if (EnemyBullets.Contains(bullet))
            {
                EnemyBullets.Remove(bullet);
            }

            if (PlayerBullets.Contains(bullet))
            {
                PlayerBullets.Remove(bullet);
            }
        }

        DestroyedBullets.Clear();
    }
}

