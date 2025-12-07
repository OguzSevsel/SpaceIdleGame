using System.Collections.Generic;
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
}
