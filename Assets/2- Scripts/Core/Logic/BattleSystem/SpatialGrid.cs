using System.Collections.Generic;
using UnityEngine;

public class SpatialGrid : MonoBehaviour
{
    public static SpatialGrid Instance;

    public float CellSize = 2f;
    private Dictionary<Vector2Int, List<Ship>> grid = new();

    private void Awake()
    {
        Instance = this;
    }

    public void RegisterShip(Ship ship)
    {
        Vector2Int cell = WorldToCell(ship.transform.position);
        if (!grid.ContainsKey(cell))
            grid[cell] = new List<Ship>();

        grid[cell].Add(ship);
    }

    public void UpdateShipCell(Ship ship, Vector3 lastPos)
    {
        Vector2Int oldCell = WorldToCell(lastPos);
        Vector2Int newCell = WorldToCell(ship.transform.position);

        if (oldCell == newCell)
            return;

        if (grid.ContainsKey(oldCell))
        {
            grid[oldCell].Remove(ship);
            if (grid[oldCell].Count == 0)
                grid.Remove(oldCell);
        }

        if (!grid.ContainsKey(newCell))
            grid[newCell] = new List<Ship>();

        grid[newCell].Add(ship);
    }


    public void UnregisterShip(Ship ship)
    {
        Vector2Int cell = WorldToCell(ship.transform.position);

        if (grid.ContainsKey(cell))
        {
            grid[cell].Remove(ship);
            if (grid[cell].Count == 0)
                grid.Remove(cell);
        }
    }


    public List<Ship> GetNearbyShips(Vector3 pos)
    {
        Vector2Int baseCell = WorldToCell(pos);
        List<Ship> result = new();

        for (int x = -1; x <= 1; x++)
            for (int y = -1; y <= 1; y++)
            {
                Vector2Int cell = new(baseCell.x + x, baseCell.y + y);
                if (grid.ContainsKey(cell))
                    result.AddRange(grid[cell]);
            }

        return result;
    }

    private Vector2Int WorldToCell(Vector3 pos)
    {
        return new Vector2Int(
            Mathf.FloorToInt(pos.x / CellSize),
            Mathf.FloorToInt(pos.y / CellSize)
        );
    }
}
