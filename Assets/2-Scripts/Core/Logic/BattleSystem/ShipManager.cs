using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ShipManager : MonoBehaviour
{
    public static ShipManager Instance;
    public List<PlayerShip> PlayerShips;
    public List<EnemyShip> EnemyShips;

    private void Awake()
    {
        Instance = this;
        PlayerShips = new List<PlayerShip>();
        EnemyShips = new List<EnemyShip>();
    }

    public void Update()
    {
        foreach (var ship in PlayerShips)
        {
            if (ship != null) ship.Tick();
        }

        foreach (var ship in EnemyShips)
        {
            if (ship != null) ship.Tick();
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

    public List<Ship> GetTargetForHealing(Ship ship)
    {
        var ships = new List<Ship>();

        if (ship is EnemyShip)
        {
            foreach (var enemyShip in EnemyShips)
            {
                float distancesq = math.distancesq(enemyShip.transform.position, ship.transform.position);
                float radSum = enemyShip.DataSO.HitRadius + ship.DataSO.HealRadius;

                if (distancesq <= radSum * radSum)
                {
                    ships.Add(enemyShip);
                }
            }
        }
        else
        {
            foreach (var playerShip in PlayerShips)
            {
                float distancesq = math.distancesq(playerShip.transform.position, ship.transform.position);
                float radSum = playerShip.DataSO.HitRadius + ship.DataSO.HealRadius;

                if (distancesq <= radSum * radSum)
                {
                    ships.Add(playerShip);
                }
            }
        }

        return ships;
    }
}
