using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LayerUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private ShipType _shipType;
    private FormationShape _formationShape;
    public int layerNumber = 1;
    [SerializeField] private List<Sprite> _images;
    [SerializeField] private List<string> _imageNames;
    private Dictionary<string, Sprite> _imageDict;
    private Image _image;
    [SerializeField] private TextMeshProUGUI _layerNumberText;
    [SerializeField] private Color _dpsColor;
    [SerializeField] private Color _healerColor;
    [SerializeField] private Color _tankColor;
    private Outline _outline;

    private void Start()
    {
        _image = GetComponent<Image>();
        gameObject.SetActive(false);
        _imageDict = new Dictionary<string, Sprite>();

        for (int i = 0; i < _images.Count; i++)
        {
            Sprite image = _images[i];
            string imageName = _imageNames[i];

            _imageDict.Add(imageName, image);
        }

        _image.alphaHitTestMinimumThreshold = 0.1f;
        this._formationShape = FormationShape.Rectangle;
        this._shipType = ShipType.Dps;

        _outline = gameObject.AddComponent<Outline>();
        _outline.effectDistance = new Vector2(2f, 2f);
        _outline.enabled = false;
        _outline.effectColor = _dpsColor;

        SwitchImage(_formationShape, _shipType);
        FormationCreationUI.OnFormationShapeChanged += OnFormationShapeChangedHandler;
    }

    private void OnFormationShapeChangedHandler(FormationShape shape)
    {
        this._formationShape = shape;
        UpdateLayerInfo();
        SwitchImage(_formationShape, _shipType);
    }

    private string GetImageName(FormationShape shape, ShipType type)
    {
        return shape.ToString() + type.ToString();
    }

    private void SwitchImage(FormationShape shape, ShipType type)
    {
        string imageName = GetImageName(shape, type);

        var image = _imageDict[imageName];

        if (image != null)
        {
            _image.sprite = image;
        }
    }

    private void UpdateLayerInfo()
    {
        _layerNumberText.text = $"Layer {layerNumber}";
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
            var shipTypeSubMenu = new List<ContextMenuOption>()
            {
                new ContextMenuOption("Dps", OnDpsClicked),
                new ContextMenuOption("Healer", OnHealerClicked),
                new ContextMenuOption("Tank", OnTankClicked),
            };

            var options = new List<ContextMenuOption>()
            {
                new ContextMenuOption("Change Ship Type", null, shipTypeSubMenu),
            };

            Vector2 mousePos = Mouse.current.position.ReadValue();
            ContextMenuManager.Instance.Show(mousePos, options);
        }
    }
    private void SetOutlineEffectColor(ShipType type)
    {
        switch (type)
        {
            case ShipType.Dps:
                _outline.effectColor = _dpsColor;
                break;
            case ShipType.Tank:
                _outline.effectColor = _tankColor;
                break;
            case ShipType.Healer:
                _outline.effectColor = _healerColor;
                break;
            default:
                break;
        }
    }

    private void OnTankClicked()
    {
        this._shipType = ShipType.Tank;
        UpdateLayerInfo();
        SwitchImage(_formationShape, _shipType);
        SetOutlineEffectColor(_shipType);
    }

    private void OnHealerClicked()
    {
        this._shipType = ShipType.Healer;
        UpdateLayerInfo();
        SwitchImage(_formationShape, _shipType);
        SetOutlineEffectColor(_shipType);
    }

    private void OnDpsClicked()
    {
        this._shipType = ShipType.Dps;
        UpdateLayerInfo();
        SwitchImage(_formationShape, _shipType);
        SetOutlineEffectColor(_shipType);
    }
    private void OnDestroy()
    {
        FormationCreationUI.OnFormationShapeChanged -= OnFormationShapeChangedHandler;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _outline.enabled = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _outline.enabled = true;
    }
}
