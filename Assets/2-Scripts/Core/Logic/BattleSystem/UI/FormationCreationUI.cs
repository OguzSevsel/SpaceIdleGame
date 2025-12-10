using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;

public class FormationCreationUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Button _saveFormationButton;
    [SerializeField] private Button _addLayerButton;
    [SerializeField] private Button _removeLayerButton;
    [SerializeField] private GameObject[] layerUIs;
    [SerializeField] private Dictionary<int, GameObject> layerUIDictionary;

    private int _currentLayer;
    public static event Action<Formation> OnFormationSaved;
    public static event Action<FormationShape> OnFormationShapeChanged;

    private void Start()
    {
        layerUIDictionary = new Dictionary<int, GameObject>();
        _currentLayer = 0;
        _saveFormationButton.onClick.AddListener(SaveFormationButtonClickHandler);
        _addLayerButton.onClick.AddListener(AddLayerButtonClickHandler);
        _removeLayerButton.onClick.AddListener(RemoveLayerButtonClickHandler);

        foreach (var layerUI in layerUIs)
        {
            layerUIDictionary.Add(layerUI.GetComponent<LayerUI>().layerNumber, layerUI);
        }
    }

    private void RemoveLayerButtonClickHandler()
    {
        var gameObject = layerUIDictionary[_currentLayer];
        gameObject.SetActive(false);

        _currentLayer--;
        if (_currentLayer <= 0)
        {
            _currentLayer = 0;
        }
        Debug.Log($"Current Layer: {_currentLayer}");
    }

    private void AddLayerButtonClickHandler()
    {
        _currentLayer++;
        if (_currentLayer >= 5)
        {
            _currentLayer = 5;
        }
        Debug.Log($"Current Layer: {_currentLayer}");
        var gameObject = layerUIDictionary[_currentLayer];
        gameObject.SetActive(true);
    }
    private int[] LayerUnitCounts;

    private void SaveFormationButtonClickHandler()
    {
        int layerCount = 0;

        foreach (KeyValuePair<int, GameObject> layer in layerUIDictionary)
        {
            if (layer.Value.gameObject.activeInHierarchy)
            {
                layerCount++;
            }
        }

        var formationShapes = new FormationShape[layerCount];
        var shipTypes = new ShipType[layerCount];
        float spacing = 2f;
        int unitCount = 100;

        for (int i = 1; i <= layerCount; i++)
        {
            var gameObject = layerUIDictionary[i];

            if (gameObject.activeInHierarchy)
            {
                (ShipType type, FormationShape shape) = gameObject.GetComponent<LayerUI>().GetLayerInfo();
                shipTypes[i - 1] = type;
                formationShapes[i - 1] = shape;
            }
        }

        InitializeLayerUnitCounts(unitCount, layerCount);

        Formation formation = new Formation(layerCount, LayerUnitCounts, formationShapes, shipTypes, spacing, unitCount);

        OnFormationSaved?.Invoke(formation);
        this.gameObject.SetActive(false);
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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            var options = new List<ContextMenuOption>()
            {
                new ContextMenuOption("Circle", OnCircleClicked),
                new ContextMenuOption("Triangle", OnTriangleClicked),
                new ContextMenuOption("Rectangle", OnRectangleClicked),
                new ContextMenuOption("Pentagon", OnPentagonClicked),
                new ContextMenuOption("Hexagon", OnHexagonClicked),
            };

            Vector2 mousePos = Mouse.current.position.ReadValue();
            ContextMenuManager.Instance.Show(mousePos, options);
        }
    }

    private void OnHexagonClicked()
    {
        OnFormationShapeChanged?.Invoke(FormationShape.Hexagon);
    }

    private void OnPentagonClicked()
    {
        OnFormationShapeChanged?.Invoke(FormationShape.Pentagon);
    }

    private void OnRectangleClicked()
    {
        OnFormationShapeChanged?.Invoke(FormationShape.Rectangle);
    }

    private void OnTriangleClicked()
    {
        OnFormationShapeChanged?.Invoke(FormationShape.Triangle);
    }

    private void OnCircleClicked()
    {
        OnFormationShapeChanged?.Invoke(FormationShape.Circle);
    }
}
