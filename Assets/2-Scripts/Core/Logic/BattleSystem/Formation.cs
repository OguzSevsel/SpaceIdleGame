using NUnit.Framework.Constraints;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public enum FormationShape
{
    Circle,
    Triangle,
    Rectangle,
    Pentagon,
    Hexagon,
}

[System.Serializable]
public class Formation
{
    public int FormationLayerCount = 1;
    public int CurrentFormationLayer = 1;
    public FormationShape FormationShape = FormationShape.Rectangle;
}

public class Layer
{
    public Dictionary<List<Quaternion>, int> rotations;
    public Dictionary<List<Vector3>, int> positions;

    public Layer(FormationShape shape, int layer, float spacing, int unitCount)
    {
        rotations = new Dictionary<List<Quaternion>, int>();
        positions = new Dictionary<List<Vector3>, int>();

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

    //private void GenerateCircle(int layer, float spacing, int unitCount)
    //{
    //    List<Vector3> list = new();

    //    float radius = spacing * layer;

    //    for (int i = 0; i < unitCount; i++)
    //    {
    //        float angle = (i / (float)unitCount) * Mathf.PI * 2f;
    //        list.Add(new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * radius);
    //    }

    //    AdjustDirections(list, new Vector3(0, 0));
    //}

    private void GenerateCircle(int layers, float spacing, int unitCount)
    {
        Dictionary<List<Vector3>, int> final = new();
        List<Vector3> list = new();

        int remaining = unitCount;
        int layerIndex = 1;

        while (remaining > 0 && layerIndex <= layers)
        {
            float radius = spacing * layerIndex;

            // how many points this ring should have
            int ringCount = Mathf.Min(remaining, layerIndex * 6);   // good density scaling

            for (int i = 0; i < ringCount; i++)
            {
                float angle = (i / (float)ringCount) * Mathf.PI * 2f;
                Vector3 p = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
                list.Add(p);
            }

            final.Add(list, layerIndex);
            remaining -= ringCount;
            list = new();
            layerIndex++;
        }

        AdjustDirections(final, Vector3.zero);
    }

    private void GeneratePolygonWithLayers(int unitCount, float spacing, int sides, int layerCount)
    {
        Dictionary<List<Vector3>, int> final = new();
        List<Vector3> list = new();

        int layer = 1;
        int remaining = unitCount;

        while (remaining > 0 && layer <= layerCount)
        {
            int desiredUnits = sides * layer;   // example rule (scales nicely)
            int take = Mathf.Min(desiredUnits, remaining);

            float radius = layer * spacing * 1f; // scale radius per layer

            if (layer != 1)
            {
                radius = layer * spacing * 0.7f; // scale radius per layer
            }

            var ring = GeneratePolygonLayer(sides, radius, take);
            list.AddRange(ring);
            final.Add(list, layer);
            list = new List<Vector3>();
            remaining -= take;
            layer++;
        }

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



public static class FormationGenerator
{
    public static (Dictionary<List<Vector3>, int>, Dictionary<List<Quaternion>, int>) 
        Generate(FormationShape shape, int unitCount, int layerCount, float spacing)
    {
        Dictionary<List<Vector3>, int> positions = new();
        Dictionary<List<Quaternion>, int> rotations = new();

        if (unitCount <= 0)
            return (null, null);

        for (int i = 1; i <= layerCount; i++)
        {
            Layer layer = new Layer(shape, i, spacing, unitCount);
            positions = layer.positions;
            rotations = layer.rotations;
        }

        return (positions, rotations);
    }
}


