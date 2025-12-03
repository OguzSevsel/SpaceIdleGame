using System.Collections.Generic;
using UnityEngine;

public class Fleet : MonoBehaviour
{
    private List<Ship> _ships;
    public IReadOnlyList<Ship> Ships => _ships;

    private void Awake()
    {
        _ships = new List<Ship>();
    }
    
    public void PopulateFleet()
    {
        throw new System.NotImplementedException();
    }

    public void AddShip(Ship shipSO)
    {
        _ships.Add(shipSO);
    }

    public void RemoveShip(Ship shipSO)
    {
        _ships.Remove(shipSO);
    }
}
