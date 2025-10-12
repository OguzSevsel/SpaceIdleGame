using System;
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

    private List<CollectorModel> collectors = new List<CollectorModel>();
    private List<ResourceSO> resourceSOs = new List<ResourceSO>();
    private List<CollectorSO> collectorSOs = new List<CollectorSO>();

    private Foldout collectorsFoldout;

    private VisualTreeAsset _collectorCard;
    private VisualTreeAsset _collectorSOCard;
    private VisualTreeAsset _resourceSOCard;
    private VisualTreeAsset _colonySOCard;

    private string _mainVisualTreePath = "Assets/Editor/VisualTree/ColonyCreatorWindow.uxml";
    private string _collectorCardVisualTreePath = "Assets/Editor/VisualTree/CollectorCard.uxml";
    private string _collectorSOCardVisualTreePath = "Assets/Editor/VisualTree/CollectorSOCard.uxml";
    private string _resourceSOCardVisualTreePath = "Assets/Editor/VisualTree/ResourceSOCard.uxml";
    private string _colonySOCardVisualTreePath = "Assets/Editor/VisualTree/ColonySOCard.uxml";


    private string _mainStylePath = "Assets/Editor/Style/ColonyCreatorWindow.uss";
    private string _collectorCardStylePath = "Assets/Editor/Style/CollectorCard.uss";
    private string _collectorSOCardStylePath = "Assets/Editor/Style/CollectorSOCard.uss";
    private string _resourceSOCardStylePath = "Assets/Editor/Style/ResourceSOCard.uss";
    private string _colonySOCardStylePath = "Assets/Editor/Style/ColonySOCard.uss";


    private string _SOFolder = "Assets/ScriptableObjects/";
    private TreeView _treeView;
    private List<TreeViewItemData<TreeItem>> _rootItems = new List<TreeViewItemData<TreeItem>>();
    private int _parentId = 0;


    public void CreateGUI()
    {
        // Load UXML + USS
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(_mainVisualTreePath);
        visualTree.CloneTree(rootVisualElement);

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(_mainStylePath);
        rootVisualElement.styleSheets.Add(styleSheet);
        _treeView = rootVisualElement.Q<TreeView>("Hierarchy");
        TreeItem colony = new TreeItem(_parentId, label: "Colony");

        TreeViewItemData<TreeItem> rootItem = new TreeViewItemData<TreeItem>(_parentId, colony);
        _rootItems.Add(rootItem);

        _treeView.SetRootItems(_rootItems);

        RegisterVisualTrees();

        // Get references
        collectorsFoldout = rootVisualElement.Q<Foldout>("collectorsFoldout");

        var addCollectorButton = rootVisualElement.Q<Button>("addCollectorButton");
        var createColonyButton = rootVisualElement.Q<Button>("createColonyButton");
        var addColonySOButton = rootVisualElement.Q<Button>("addColonySOButton");

        addCollectorButton.clicked += AddCollectorUI;
        addColonySOButton.clicked += AddColonySOUI;
    }








    #region Utils

    private void RegisterVisualTrees()
    {
        _collectorCard = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(_collectorCardVisualTreePath);
        var collectorCardStyle = AssetDatabase.LoadAssetAtPath<StyleSheet>(_collectorCardStylePath);

        _collectorSOCard = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(_collectorSOCardVisualTreePath);
        var collectorSOCardStyle = AssetDatabase.LoadAssetAtPath<StyleSheet>(_collectorSOCardStylePath);

        _resourceSOCard = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(_resourceSOCardVisualTreePath);
        var resourceSOCardStyle = AssetDatabase.LoadAssetAtPath<StyleSheet>(_resourceSOCardStylePath);

        _colonySOCard = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(_colonySOCardVisualTreePath);
        var colonySOCardStyle = AssetDatabase.LoadAssetAtPath<StyleSheet>(_colonySOCardStylePath);
    }

    private void AddChildToItem(int parentId, TreeItem childName)
    {
        int newId = GetUniqueId();
        var childItem = new TreeViewItemData<TreeItem>(newId, childName);

        _treeView.AddItem(childItem, parentId, -1, true);
    }

    private int GetUniqueId()
    {
        int maxId = 0;
        foreach (var item in _rootItems)
            if (item.id > maxId) maxId = item.id;
        return maxId + 1;
    }

    #endregion

    #region Add Scriptable Objects

    private void AddCollectorSO(string collectorName, CollectorType collectorType, ResourceSO resource, double baseCollectionRate)
    {
        string path = _SOFolder + "2-Collectors";

        var collector = ScriptableObjectUtility.CreateAssetAtPath<CollectorSO>(path, collectorName);

        collector.CollectorName = collectorName;
        collector.CollectorType = collectorType;
        collector.GeneratedResource = resource;
        collector._baseCollectionRate = baseCollectionRate;

        EditorUtility.SetDirty(collector);

        AddChildToItem(_parentId, new TreeItem(GetUniqueId(), label: collector.CollectorName, reference: collector));
    }

    private void AddColonySO(string colonyName)
    {
        string path = _SOFolder + "3-Colonies";

        var colony = ScriptableObjectUtility.CreateAssetAtPath<ColonySO>(path, colonyName);

        EditorUtility.SetDirty(colony);

        AddChildToItem(_parentId, new TreeItem(GetUniqueId(), label: colony.ColonyType.ToString(), reference: colony));
    }

    private void AddResourceSO(string resourceName)
    {
        string path = _SOFolder + "1-Resources";

        var resource = ScriptableObjectUtility.CreateAssetAtPath<ResourceSO>(path, resourceName);

        EditorUtility.SetDirty(resource);

        AddChildToItem(_parentId, new TreeItem(GetUniqueId(), label: resource.resourceType.ToString(), reference: resource));
    }

    #endregion

    #region UI

    private void AddCollectorSOUI()
    {
        VisualElement collectorSOUI = _collectorSOCard.CloneTree();

        var generatedResourceSO = collectorSOUI.Q<ObjectField>("GeneratedResourceSO");
        generatedResourceSO.objectType = typeof(ResourceSO);
        generatedResourceSO.allowSceneObjects = false;

        var collectorTypeField = collectorSOUI.Q<EnumField>("CollectorType");
        var baseCollectionRateField = collectorSOUI.Q<Slider>("BaseCollectionRate");
        var collectorSOCreateButton = collectorSOUI.Q<Button>("Create");

        collectorSOCreateButton.clicked += () =>
        {
            AddCollectorSO(collectorTypeField.value.ToString(), (CollectorType)collectorTypeField.value, (ResourceSO)generatedResourceSO.value, baseCollectionRateField.value);
        };

        collectorsFoldout.Add(collectorSOUI);
    }

    private void AddResourceSOUI()
    {
        VisualElement resourceSOUI = _resourceSOCard.CloneTree();

        var resourceIcon = resourceSOUI.Q<ObjectField>("ResourceIcon");
        resourceIcon.objectType = typeof(Sprite);
        resourceIcon.allowSceneObjects = false;

        var resourceTypeField = resourceSOUI.Q<EnumField>("ResourceType");
        var resourceUnitField = resourceSOUI.Q<TextField>("ResourceUnit");
        var resourceSOCreateButton = resourceSOUI.Q<Button>("Create");

        resourceSOCreateButton.clicked += () =>
        {
            AddResourceSO(resourceTypeField.value.ToString());
        };

        collectorsFoldout.Add(resourceSOUI);
    }

    private void AddCollectorUI()
    {
        AddCollectorSOUI();

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

    private void AddColonySOUI()
    {
        VisualElement colonySOUI = _colonySOCard.CloneTree();

        var colonyNameField = colonySOUI.Q<TextField>("ColonyName");
        var colonyTypeField = colonySOUI.Q<EnumField>("ColonyType");
        var colonySOCreateButton = colonySOUI.Q<Button>("Create");

        colonySOCreateButton.clicked += () =>
        {
            AddColonySO(colonyTypeField.value.ToString());
        };

        collectorsFoldout.Add(colonySOUI);
    }

    #endregion

    #region Not Implemented

    //private void CreateColonyInScene()
    //{
    //    GameObject colony = new GameObject($"{colonySO.ColonyType}_Colony");
    //    var colonyModel = colony.AddComponent<ColonyModel>();
    //    colonyModel.colonyData = colonySO;
    //    colonyModel.Collectors = new List<CollectorModel>();

    //    foreach (var c in collectors)
    //    {
    //        GameObject collectorObj = new GameObject("Collector");
    //        var cm = collectorObj.AddComponent<CollectorModel>();
    //        cm.Data = new CollectorData();
    //        cm.Data.SetCollectionRate(UnityEngine.Random.Range(1f, 10f)); // placeholder

    //        collectorObj.transform.SetParent(colony.transform);
    //        colonyModel.Collectors.Add(cm);
    //    }

    //    Debug.Log($"Created Colony '{colony.name}' with {collectors.Count} collectors.");
    //}

    #endregion
}

#region Util Classes

[Serializable]
public class TreeItem
{
    public UnityEngine.Object reference; // ScriptableObject, GameObject, anything
    public int id;
    public int parentId;
    public string label;

    public TreeItem(int id, string label = null, UnityEngine.Object reference = null, int parentId = -1)
    {
        this.id = id;
        this.label = label;
        this.reference = reference;
        this.parentId = parentId;
    }

    public override string ToString()
    {
        return label;
    }
}

public static class ScriptableObjectUtility
{
    public static T CreateAssetAtPath<T>(string folderPath, string assetName) where T : ScriptableObject
    {
        // Ensure folder exists
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            Debug.LogError($"Folder not found: {folderPath}");
            return null;
        }

        // Create instance
        T asset = ScriptableObject.CreateInstance<T>();

        // Ensure .asset extension
        string path = System.IO.Path.Combine(folderPath, assetName + ".asset");

        // Make sure it’s unique
        path = AssetDatabase.GenerateUniqueAssetPath(path);

        // Create and save asset
        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        return asset;
    }
}

#endregion

