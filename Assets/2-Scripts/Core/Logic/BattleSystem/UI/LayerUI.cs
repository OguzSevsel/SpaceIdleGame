using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;

public class LayerUI : MonoBehaviour, IPointerClickHandler
{
    private ShipType _shipType;
    private FormationShape _formationShape;
    public int layerNumber = 1;
    [SerializeField] private List<GameObject> Images;
    private Image _image;
    [SerializeField] private TextMeshProUGUI _layerNumberText;
    [SerializeField] private TextMeshProUGUI _shipTypeText;
    [SerializeField] private TextMeshProUGUI _layerShapeText;
    [SerializeField] private Color _dpsColor;
    [SerializeField] private Color _healerColor;
    [SerializeField] private Color _tankColor;

    private void Start()
    {
        _image = GetComponent<Image>();
        gameObject.SetActive(false);
    }

    private void UpdateLayerInfo()
    {
        _layerNumberText.text = $"Layer {layerNumber}";
        _shipTypeText.text = $"Ship Type: {_shipType.ToString()}";
        _layerShapeText.text = $"Layer Shape: {_formationShape.ToString()}";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            CreateContextMenu(eventData);
        }
        else if (eventData.button == PointerEventData.InputButton.Left)
        {
            ContextMenuManager.Instance.HideAll();
        }
    }

    public (ShipType shipType, FormationShape shape) GetLayerInfo()
    {
        return (_shipType, _formationShape);
    }

    private void CreateContextMenu(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            var shapeSubMenu = new List<ContextMenuOption>()
            {
                new ContextMenuOption("Circle", OnCircleClicked),
                new ContextMenuOption("Triangle", OnTriangleClicked),
                new ContextMenuOption("Rectangle", OnRectangleClicked),
                new ContextMenuOption("Pentagon", OnPentagonClicked),
                new ContextMenuOption("Hexagon", OnHexagonClicked),
            };

            var shipTypeSubMenu = new List<ContextMenuOption>()
            {
                new ContextMenuOption("Dps", OnDpsClicked),
                new ContextMenuOption("Healer", OnHealerClicked),
                new ContextMenuOption("Tank", OnTankClicked),
            };

            var options = new List<ContextMenuOption>()
            {
                new ContextMenuOption("Change Ship Type", null, shipTypeSubMenu),
                new ContextMenuOption("Change Shape", null, shapeSubMenu),
            };

            Vector2 mousePos = Mouse.current.position.ReadValue();
            ContextMenuManager.Instance.Show(mousePos, options);
        }
    }

    private void OnTankClicked()
    {
        this._shipType = ShipType.Tank;
        UpdateLayerInfo();
        _image.color = _tankColor;
    }

    private void OnHealerClicked()
    {
        this._shipType = ShipType.Healer;
        UpdateLayerInfo();
        _image.color = _healerColor;
    }

    private void OnDpsClicked()
    {
        this._shipType = ShipType.Dps;
        UpdateLayerInfo();
        _image.color = _dpsColor;
    }

    private void OnHexagonClicked()
    {
        this._formationShape = FormationShape.Hexagon;
        UpdateLayerInfo();
    }

    private void OnPentagonClicked()
    {
        this._formationShape = FormationShape.Pentagon;
        UpdateLayerInfo();
    }

    private void OnRectangleClicked()
    {
        this._formationShape = FormationShape.Rectangle;
        UpdateLayerInfo();
    }

    private void OnTriangleClicked()
    {
        this._formationShape = FormationShape.Triangle;
        UpdateLayerInfo();
    }

    private void OnCircleClicked()
    {
        this._formationShape = FormationShape.Circle;
        UpdateLayerInfo();
    }
}
