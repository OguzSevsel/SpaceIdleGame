using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleSystemUI : MonoBehaviour
{
    [SerializeField] private Button _startLevelButton; 
    [SerializeField] private Button _createFormationButton;
    [SerializeField] private TextMeshProUGUI _formationText;
    public static event Action OnStartLevel;

    private void Start()
    {
        _startLevelButton.onClick.AddListener(StartLevelClickHandler);
        _createFormationButton.onClick.AddListener(CreateFormationClickHandler);
        //FormationCreationUI.OnFormationCreated += OnFormationCreatedHandler;
    }

    private void CreateFormationClickHandler()
    {

    }

    private void StartLevelClickHandler()
    {
        OnStartLevel?.Invoke();
    }
}
