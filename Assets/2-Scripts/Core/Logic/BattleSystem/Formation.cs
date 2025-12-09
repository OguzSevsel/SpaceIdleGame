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
    private FormationShape[] Shapes;
    private ShipType[] ShipTypes;
    private Layer[] Layers;
    private int[] LayerUnitCounts;

    public Formation(int layerCount, int[] unitCounts, FormationShape[] shapes, ShipType[] shipTypes, float spacing, int totalUnitCount)
    {
        Layers = new Layer[layerCount];
        ShipTypes = new ShipType[layerCount];
        Shapes = new FormationShape[layerCount];
        LayerUnitCounts = new int[layerCount];
        this.LayerUnitCounts = unitCounts;
        this.LayerCount = layerCount;
        this.Shapes = shapes;
        this.ShipTypes = shipTypes;
        this.Spacing = spacing;
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
                LayerUnitCounts[i] = unit;
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
                LayerUnitCounts[i] = unit;
            }
        }
    }

    private void InitializeLayers()
    {
        for (int i = Layers.Length - 1; i >= 0; i--)
        {
            var layer = Layers[i];
            int layerUnitCount = LayerUnitCounts[i];
            var shape = Shapes[i];
            var shipType = ShipTypes[i];
            int index = SwitchLayers(i + 1);
            layer = new Layer(shape, shipType, index, this.Spacing, layerUnitCount);
            Layers[i] = layer;
        }
    }

    private int SwitchLayers(int value)
    {
        switch (value)
        {
            case 5:
                value = 1;
                break;
            case 4:
                value = 2;
                break;
            case 3:
                value = 3;
                break;
            case 2:
                value = 4;
                break;
            case 1:
                value = 5;
                break;
            default:
                break;
        }
        return value;
    }
}


