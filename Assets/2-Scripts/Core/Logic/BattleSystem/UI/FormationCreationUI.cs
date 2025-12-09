using System;
using UnityEngine;
using UnityEngine.UI;

public class FormationCreationUI : MonoBehaviour
{
    [SerializeField] private Button _saveFormationButton;
    [SerializeField] private Button _addLayerButton;
    [SerializeField] private Button _removeLayerButton;
    private int _currentLayer;
    public static event Action OnFormationSaved;
    public static event Action<int> OnLayerChanged;

    private void Start()
    {
        _currentLayer = 0;
        _saveFormationButton.onClick.AddListener(SaveFormationButtonClickHandler);
        _addLayerButton.onClick.AddListener(AddLayerButtonClickHandler);
        _removeLayerButton.onClick.AddListener(RemoveLayerButtonClickHandler);
    }

    private void RemoveLayerButtonClickHandler()
    {
        _currentLayer--;
        if (_currentLayer <= 0)
        {
            _currentLayer = 0;
        }
        Debug.Log($"Current Layer: {_currentLayer}");
        OnLayerChanged?.Invoke(_currentLayer);
    }

    private void AddLayerButtonClickHandler()
    {
        _currentLayer++;
        if (_currentLayer >= 5)
        {
            _currentLayer = 5;
        }
        Debug.Log($"Current Layer: {_currentLayer}");
        OnLayerChanged?.Invoke(_currentLayer);
    }

    private void SaveFormationButtonClickHandler()
    {

    }
}
