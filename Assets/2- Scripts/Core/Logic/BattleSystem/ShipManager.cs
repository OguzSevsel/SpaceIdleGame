using System.Collections.Generic;
using UnityEngine;

public class ShipManager : MonoBehaviour
{
    public static ShipManager Instance;

    public List<PlayerShip> PlayerShips = new();
    public List<EnemyShip> EnemyShips = new();

    public List<Bullet> Bullets = new();

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        foreach (var enemyShip in EnemyShips)
        {
            enemyShip.Tick();
        }

        foreach (var playerShip in PlayerShips)
        {
            playerShip.Tick();
        }
    }

    public void RegisterShip(Ship ship)
    {
        if (ship is PlayerShip p) PlayerShips.Add(p);
        else if (ship is EnemyShip e) EnemyShips.Add(e);
    }

    public void UnregisterShip(Ship ship)
    {
        if (ship is PlayerShip p) PlayerShips.Remove(p);
        else if (ship is EnemyShip e) EnemyShips.Remove(e);
    }

    public List<Ship> GetTargetsForBullet(bool isPlayerBullet)
    {
        return isPlayerBullet ? new List<Ship>(EnemyShips) : new List<Ship>(PlayerShips);
    }
}
