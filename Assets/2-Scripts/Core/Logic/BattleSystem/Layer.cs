using System.Collections.Generic;
using UnityEngine;

public class Layer
{
    public Dictionary<List<Quaternion>, int> rotations;
    public Dictionary<List<Vector3>, int> positions;
    public ShipType ShipType;

    public Layer(FormationShape shape, ShipType shipType, int layer, float spacing, int unitCount)
    {
        rotations = new Dictionary<List<Quaternion>, int>();
        positions = new Dictionary<List<Vector3>, int>();
        this.ShipType = shipType;

        switch (shape)
        {
            case FormationShape.Circle:
                GenerateCircle(layer, spacing, unitCount);
                break;
            case FormationShape.Triangle:
                GeneratePolygonWithLayers(unitCount, spacing, sides: 3, layer);
                break;
            case FormationShape.Rectangle:
                GeneratePolygonWithLayers(unitCount, spacing, sides: 4, layer);
                break;
            case FormationShape.Pentagon:
                GeneratePolygonWithLayers(unitCount, spacing, sides: 5, layer);
                break;
            case FormationShape.Hexagon:
                GeneratePolygonWithLayers(unitCount, spacing, sides: 6, layer);
                break;
            default:
                break;
        }
    }

    public void AdjustDirections(Dictionary<List<Vector3>, int> layerPositions, Vector3 center)
    {
        var rotations = new List<Quaternion>();

        foreach (var layer in layerPositions)
        {
            foreach (var position in layer.Key)
            {
                Vector2 direction = (position - center).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                Quaternion rotation = Quaternion.Euler(0, 0, angle - 90f);
                rotations.Add(rotation);
            }
            this.rotations.Add(rotations, layer.Value);
            rotations = new List<Quaternion>();
            this.positions.Add(layer.Key, layer.Value);
        }
    }

    private void GenerateCircle(int currentLayer, float spacing, int unitCount)
    {
        Dictionary<List<Vector3>, int> final = new();
        List<Vector3> list = new();

        float radius = spacing * currentLayer * 0.5f;

        int ringCount = Mathf.Min(unitCount, currentLayer * 6);

        for (int i = 0; i < ringCount; i++)
        {
            float angle = (i / (float)ringCount) * Mathf.PI * 2f;
            Vector3 p = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            list.Add(p);
        }

        final.Add(list, currentLayer);
        list = new();

        AdjustDirections(final, Vector3.zero);
    }

    private void GeneratePolygonWithLayers(int unitCount, float spacing, int sides, int currentLayer)
    {
        Dictionary<List<Vector3>, int> final = new();
        List<Vector3> list = new();

        float radius = currentLayer * spacing * 0.5f; // scale radius per layer

        if (currentLayer != 1)
        {
            radius = currentLayer * spacing * 0.5f; // scale radius per layer
        }

        var ring = GeneratePolygonLayer(sides, radius, unitCount);
        list.AddRange(ring);
        final.Add(list, currentLayer);
        list = new List<Vector3>();

        AdjustDirections(final, Vector3.zero);
    }

    private List<Vector3> GeneratePolygonLayer(int sides, float radius, int units)
    {
        List<Vector3> pts = new List<Vector3>();

        float angleStep = (-Mathf.PI * 2f) / sides;
        float startAngle = -Mathf.PI / 2f - (Mathf.PI / sides);

        // points per side
        int perSide = units / sides;
        int extra = units % sides;

        for (int side = 0; side < sides; side++)
        {
            int count = perSide + (side < extra ? 1 : 0);

            Vector3 a = new Vector3(
                Mathf.Cos(startAngle + angleStep * side),
                Mathf.Sin(startAngle + angleStep * side)
            ) * radius;

            Vector3 b = new Vector3(
                Mathf.Cos(startAngle + angleStep * (side + 1)),
                Mathf.Sin(startAngle + angleStep * (side + 1))
            ) * radius;

            for (int i = 0; i < count; i++)
            {
                float t = (float)i / count;
                pts.Add(Vector3.Lerp(a, b, t));
            }
        }

        return pts;
    }
}


