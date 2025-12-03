using UnityEngine;
using System.Collections.Generic;

public static class FormationGenerator
{
    public static Formation Line(int count, float spacing)
    {
        Formation f = new Formation();
        float start = -(count - 1) * spacing * 0.5f;

        for (int i = 0; i < count; i++)
        {
            f.offsets.Add(new Vector3(start + i * spacing, 0, 0));
        }

        return f;
    }

    public static Formation VShape(int count, float spacing)
    {
        Formation f = new Formation();
        int half = count / 2;

        for (int i = -half; i <= half; i++)
        {
            float x = i * spacing;
            float y = Mathf.Abs(i) * spacing * 0.5f;  // creates the V
            f.offsets.Add(new Vector3(x, y, 0));
        }

        return f;
    }

    public static Formation Circle(int count, float radius)
    {
        Formation f = new Formation();

        for (int i = 0; i < count; i++)
        {
            float angle = i * Mathf.PI * 2f / count;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            f.offsets.Add(new Vector3(x, y, 0));
        }

        return f;
    }

    public static Formation Box(int rows, int cols, float spacing)
    {
        Formation f = new Formation();
        float width = (cols - 1) * spacing * 0.5f;
        float height = (rows - 1) * spacing * 0.5f;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                float x = -width + c * spacing;
                float y = -height + r * spacing;
                f.offsets.Add(new Vector3(x, y, 0));
            }
        }

        return f;
    }
}
