using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class ColonyCreatorWindow : EditorWindow
{
    [MenuItem("Tools/Colony Creator (UI Toolkit)")]
    public static void ShowWindow()
    {
        var window = GetWindow<ColonyCreatorWindow>();
        window.titleContent = new GUIContent("Colony Creator");
    }

    private ColonySO colonySO;
    private List<CollectorModel> collectors = new List<CollectorModel>();

    private EnumField colonyTypeField;
    private TextField colonyNameField;
    private Foldout collectorsFoldout;
    private VisualTreeAsset _collectorCard;

    private string _mainVisualTreePath = "Assets/Editor/ColonyCreatorWindow.uxml";
    private string _mainStylePath = "Assets/Editor/ColonyCreatorWindow.uss";
    private string _collectorCardStylePath = "Assets/Editor/CollectorCard.uss";
    private string _collectorCardVisualTreePath = "Assets/Editor/CollectorCard.uxml";

    public void CreateGUI()
    {
        // Load UXML + USS
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(_mainVisualTreePath);
        visualTree.CloneTree(rootVisualElement);

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(_mainStylePath);
        rootVisualElement.styleSheets.Add(styleSheet);

        _collectorCard = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(_collectorCardVisualTreePath);
        var collectorCardStyle = AssetDatabase.LoadAssetAtPath<StyleSheet>(_collectorCardStylePath);

        // Get references
        colonyTypeField = rootVisualElement.Q<EnumField>("colonyType");
        colonyNameField = rootVisualElement.Q<TextField>("colonyName");
        collectorsFoldout = rootVisualElement.Q<Foldout>("collectorsFoldout");

        var addCollectorButton = rootVisualElement.Q<Button>("addCollectorButton");
        var createColonyButton = rootVisualElement.Q<Button>("createColonyButton");

        // Init SO
        colonySO = ScriptableObject.CreateInstance<ColonySO>();

        // Bind enum
        colonyTypeField.Init(ColonyType.Earth);
        colonyTypeField.RegisterValueChangedCallback(evt =>
        {
            colonySO.ColonyType = (ColonyType)evt.newValue;
            colonyNameField.value = $"{colonySO.ColonyType} Colony";
        });

        // Add Collector
        addCollectorButton.clicked += AddCollectorUI;

        // Create Colony
        createColonyButton.clicked += () =>
        {
            CreateColonyInScene();
        };
    }

    private void AddCollectorUI()
    {
        VisualElement collectorUI = _collectorCard.CloneTree();

        var SOField = collectorUI.Q<UnityEditor.UIElements.ObjectField>("CollectorSO");

        SOField.objectType = typeof(CollectorSO);
        SOField.allowSceneObjects = false;

        var rateField = collectorUI.Q<Slider>("CollectorRate");
        rateField.value = rateField.value / 100;

        var rateMultField = collectorUI.Q<Slider>("CollectorRateMultiplier");
        rateMultField.value = rateMultField.value / 100;

        var speedField = collectorUI.Q<Slider>("CollectorSpeed");
        speedField.value = speedField.value / 100;

        var speedMultField = collectorUI.Q<Slider>("CollectorSpeedMultiplier");
        speedMultField.value = speedMultField.value / 100;

        var levelField = collectorUI.Q<Slider>("CollectorLevel");
        var levelIncField = collectorUI.Q<Slider>("CollectorLevelIncrement");

        collectorsFoldout.Add(collectorUI);
    }

    private void CreateColonyInScene()
    {
        if (colonySO == null) return;

        GameObject colony = new GameObject($"{colonySO.ColonyType}_Colony");
        var colonyModel = colony.AddComponent<ColonyModel>();
        colonyModel.colonyData = colonySO;
        colonyModel.Collectors = new List<CollectorModel>();

        foreach (var c in collectors)
        {
            GameObject collectorObj = new GameObject("Collector");
            var cm = collectorObj.AddComponent<CollectorModel>();
            cm.Data = new CollectorData();
            cm.Data.SetCollectionRate(Random.Range(1f, 10f)); // placeholder

            collectorObj.transform.SetParent(colony.transform);
            colonyModel.Collectors.Add(cm);
        }

        Debug.Log($"Created Colony '{colony.name}' with {collectors.Count} collectors.");
    }
}
