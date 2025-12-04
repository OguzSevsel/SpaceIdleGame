using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Formation
{
    public int FormationLayerCount = 1;
    public int CurrentFormationLayer = 1;
    public FormationShape FormationShape = FormationShape.Rectangle;
}

public enum FormationShape
{
    Circle,
    Rectangle,
    Triangle,
    Star,
    Line,
}

public static class FormationGenerator
{
    public static List<Vector3> Generate(FormationShape shape, int totalUnits, int layers, float spacing)
    {
        List<Vector3> positions = new();

        if (totalUnits <= 0)
            return positions;

        // Always center
        positions.Add(Vector3.zero);
        totalUnits--;

        // Fill layers
        for (int layer = 1; layer <= layers && totalUnits > 0; layer++)
        {
            List<Vector3> layerPositions = GenerateLayer(shape, layer, spacing, totalUnits);

            foreach (var pos in layerPositions)
            {
                positions.Add(pos);
                totalUnits--;
                if (totalUnits <= 0)
                    break;
            }
        }

        return positions;
    }

    private static List<Vector3> GenerateLayer(FormationShape shape, int layer, float spacing, int availableUnits)
    {
        switch (shape)
        {
            case FormationShape.Circle: return Circle(layer, spacing, availableUnits);
            case FormationShape.Triangle: return Triangle(layer, spacing, availableUnits);
            case FormationShape.Rectangle: return Square(layer, spacing, availableUnits);
            case FormationShape.Line: return Line(layer, spacing, availableUnits);
            case FormationShape.Star: return Star(layer, spacing, availableUnits);
        }

        return new();
    }

    // ---------------- Shapes -----------------

    private static List<Vector3> Circle(int layer, float spacing, int maxUnits)
    {
        List<Vector3> list = new();

        int count = Mathf.Min(maxUnits, 6 * layer);
        float radius = layer * spacing;

        for (int i = 0; i < count; i++)
        {
            float angle = (i / (float)count) * Mathf.PI * 2f;
            list.Add(new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * radius);
        }

        return list;
    }

    private static List<Vector3> Triangle(int layer, float spacing, int maxUnits)
    {
        List<Vector3> list = new();

        float size = layer * spacing;
        Vector3 p1 = new(0, size);
        Vector3 p2 = new(-size, -size);
        Vector3 p3 = new(size, -size);

        List<Vector3> perimeter = new();
        AddLine(perimeter, p1, p2, layer * 3);
        AddLine(perimeter, p2, p3, layer * 3);
        AddLine(perimeter, p3, p1, layer * 3);

        return perimeter.Take(maxUnits).ToList();
    }

    private static List<Vector3> Square(int layer, float spacing, int maxUnits)
    {
        List<Vector3> list = new();
        float half = layer * spacing;

        Vector3 p1 = new(-half, half);
        Vector3 p2 = new(half, half);
        Vector3 p3 = new(half, -half);
        Vector3 p4 = new(-half, -half);

        List<Vector3> perimeter = new();
        AddLine(perimeter, p1, p2, layer * 4);
        AddLine(perimeter, p2, p3, layer * 4);
        AddLine(perimeter, p3, p4, layer * 4);
        AddLine(perimeter, p4, p1, layer * 4);

        return perimeter.Take(maxUnits).ToList();
    }

    private static List<Vector3> Line(int layer, float spacing, int maxUnits)
    {
        List<Vector3> list = new();
        float y = layer * spacing;

        int half = Mathf.Min(maxUnits / 2, layer * 2);

        for (int i = -half; i <= half; i++)
            list.Add(new Vector3(i * spacing, y));

        return list.Take(maxUnits).ToList();
    }

    private static List<Vector3> Star(int layer, float spacing, int maxUnits)
    {
        List<Vector3> list = new();
        int points = 5;

        int unitsPerArm = Mathf.Min(maxUnits / points, layer + 1);

        for (int p = 0; p < points; p++)
        {
            float angle = (p / (float)points) * Mathf.PI * 2f;
            Vector3 dir = new(Mathf.Cos(angle), Mathf.Sin(angle));

            for (int j = 1; j <= unitsPerArm; j++)
            {
                list.Add(dir * (j * spacing));
                if (list.Count >= maxUnits)
                    return list;
            }
        }

        return list;
    }

    private static void AddLine(List<Vector3> list, Vector3 a, Vector3 b, int steps)
    {
        for (int i = 0; i <= steps; i++)
        {
            float t = i / (float)steps;
            list.Add(Vector3.Lerp(a, b, t));
        }
    }
}


