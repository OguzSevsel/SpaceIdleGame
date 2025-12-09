using System.Collections.Generic;
using UnityEngine;

public enum FormationShape
{
    Circle,
    Triangle,
    Rectangle,
    Pentagon,
    Hexagon,
}

public class Formation
{
    private int LayerCount = 1;
    private float Spacing = 1.0f;
    private int TotalUnitCount;
    private FormationShape[] Shapes;
    private ShipType[] ShipTypes;
    private Layer[] Layers;
    private int[] LayerUnitCounts;

    public Formation(int layerCount, FormationShape[] shape, ShipType[] shipTypes, float spacing, int totalUnitCount)
    {
        Layers = new Layer[layerCount];
        ShipTypes = new ShipType[layerCount];
        Shapes = new FormationShape[layerCount];
        this.LayerCount = layerCount;
        this.Shapes = shape;
        this.ShipTypes = shipTypes;
        this.Spacing = spacing;
        this.TotalUnitCount = totalUnitCount;
        InitializeLayerUnitCounts(this.TotalUnitCount, this.LayerCount);
        InitializeLayers();
    }

    public Layer[] GetLayers()
    {
        return Layers;
    }

    private void InitializeLayerUnitCounts(int totalUnitCount, int layerCount)
    {
        int count = totalUnitCount % layerCount;

        if (count == 0)
        {
            LayerUnitCounts = new int[layerCount];
            for (int i = 0; i < LayerUnitCounts.Length; i++)
            {
                int unit = LayerUnitCounts[i];
                unit = (int)totalUnitCount / layerCount;
            }
        }
        else
        {
            LayerUnitCounts = new int[layerCount];
            for (int i = 0; i < LayerUnitCounts.Length; i++)
            {
                int unit = LayerUnitCounts[i];
                unit = (int)totalUnitCount / layerCount;

                if (i == LayerUnitCounts.Length - 1)
                {
                    unit = count;
                }
            }
        }
    }

    private void InitializeLayers()
    {
        for (int i = 0; i < Layers.Length; i++)
        {
            var layer = Layers[i];
            int index = System.Array.IndexOf(Layers, layer);
            int layerUnitCount = LayerUnitCounts[index];
            var shape = Shapes[i];
            var shipType = ShipTypes[i];
            layer = new Layer(shape, shipType, index + 1, this.Spacing, layerUnitCount);
        }
    }
}

public static class FormationGenerator
{
    public static Formation Generate(FormationShape[] shapes, ShipType[] shipTypes, int unitCount, int layerCount, float spacing)
    {
        List<GameObject> gameObjectList = new List<GameObject>();
        List<Formation> formationList = new List<Formation>();

        if (shapes.Length != layerCount)
        {
            Debug.LogWarning("Shape count and layer count dont match!");
            return null;
        }

        if (unitCount <= 0)
        {
            Debug.LogWarning("Unit count cant be 0 or smaller.");
            return null;
        }

        Formation formation = new Formation(layerCount, shapes, shipTypes, spacing, unitCount);
        return formation;
    }
}


