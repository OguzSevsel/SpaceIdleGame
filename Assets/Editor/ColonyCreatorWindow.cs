using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class ColonyCreatorWindow : EditorWindow
{
    #region Menu Item

    [MenuItem("Tools/Colony Creator (UI Toolkit)")]
    public static void ShowWindow()
    {
        var window = GetWindow<ColonyCreatorWindow>();
        window.titleContent = new GUIContent("Colony Creator");
    }

    #endregion

    #region Paths

    private string _mainVisualTreePath = "Assets/Editor/VisualTree/1-ColonyCreatorWindow.uxml";
    private string _colonySOCardVisualTreePath = "Assets/Editor/VisualTree/2-ColonySOCard.uxml";
    private string _resourceSOCardVisualTreePath = "Assets/Editor/VisualTree/3-ResourceSOCard.uxml";
    private string _resourceCardVisualTreePath = "Assets/Editor/VisualTree/4-ResourceCard.uxml";
    private string _collectorSOCardVisualTreePath = "Assets/Editor/VisualTree/5-CollectorSOCard.uxml";
    private string _collectorCardVisualTreePath = "Assets/Editor/VisualTree/6-CollectorCard.uxml";
    private string _costResourceCardVisualTreePath = "Assets/Editor/VisualTree/7-CostResourceCard.uxml";

    private string _mainStylePath = "Assets/Editor/Style/ColonyCreatorWindow.uss";
    private string _collectorCardStylePath = "Assets/Editor/Style/CollectorCard.uss";
    private string _collectorSOCardStylePath = "Assets/Editor/Style/CollectorSOCard.uss";
    private string _resourceSOCardStylePath = "Assets/Editor/Style/ResourceSOCard.uss";
    private string _colonySOCardStylePath = "Assets/Editor/Style/ColonySOCard.uss";

    private string _SOFolder = "Assets/ScriptableObjects/";

    #endregion

    #region UI Elements

    private VisualElement _panel;

    private VisualTreeAsset _collectorCard;
    private VisualTreeAsset _collectorSOCard;
    private VisualTreeAsset _resourceSOCard;
    private VisualTreeAsset _resourceCard;
    private VisualTreeAsset _colonySOCard;
    private VisualTreeAsset _costResourceCard;

    private TreeView _treeView;
    private List<TreeViewItemData<TreeItem>> _rootItems = new List<TreeViewItemData<TreeItem>>();
    private int _parentId = 0;
    private int _parentResourceID;

    private Button _addColonySOButton;
    private Button _selectColonySOButton;
    private Button _addComponentsButton;
    private Button _addColonyButton;
    private ObjectField _selectSOField;

    #endregion

    private List<CollectorModel> _collectors = new List<CollectorModel>();
    private List<Resource> _resources = new List<Resource>();
    private ColonySO _colonySO;
    private ResourceSO _resourceSO;
    private CollectorSO _collectorSO;
    private TreeItem _rootTreeItem;
    public void CreateGUI()
    {
        RegisterVisualTrees();
        RegisterUIElements();
        RegisterTreeItems();
        RegisterCallBacks();

        _panel.Remove(_addColonyButton);
        _panel.Remove(_addComponentsButton);
        _panel.Remove(_selectSOField);
    }

    #region Utils

    private CollectorType? GetCollectorEnumValue(ResourceSO resourceSO)
    {
        foreach (ResourceType value in Enum.GetValues(typeof(ResourceType)))
        {
            if (value == resourceSO.resourceType)
            {
                foreach (CollectorType enumValue in Enum.GetValues(typeof(CollectorType)))
                {
                    string resourceValue = value.ToString().ToLower();
                    string collectorValue = enumValue.ToString().ToLower();

                    if (collectorValue.Contains(resourceValue))
                    {
                        return enumValue;
                    }
                }
            }
        }
        return null;
    }
    private void RegisterVisualTrees()
    {
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(_mainVisualTreePath);
        visualTree.CloneTree(rootVisualElement);

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(_mainStylePath);
        rootVisualElement.styleSheets.Add(styleSheet);

        _collectorCard = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(_collectorCardVisualTreePath);
        var collectorCardStyle = AssetDatabase.LoadAssetAtPath<StyleSheet>(_collectorCardStylePath);

        _collectorSOCard = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(_collectorSOCardVisualTreePath);
        var collectorSOCardStyle = AssetDatabase.LoadAssetAtPath<StyleSheet>(_collectorSOCardStylePath);

        _resourceCard = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(_resourceCardVisualTreePath);
        _costResourceCard = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(_costResourceCardVisualTreePath);

        _resourceSOCard = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(_resourceSOCardVisualTreePath);
        var resourceSOCardStyle = AssetDatabase.LoadAssetAtPath<StyleSheet>(_resourceSOCardStylePath);

        _colonySOCard = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(_colonySOCardVisualTreePath);
        var colonySOCardStyle = AssetDatabase.LoadAssetAtPath<StyleSheet>(_colonySOCardStylePath);
    }

    private void RegisterCallBacks()
    {
        _addColonyButton.clicked += AddColony;
        _addColonySOButton.clicked += AddColonySOUI;
        _addComponentsButton.clicked += AddResourceSOUI;
        _selectColonySOButton.clicked += AddSOField;
    }

    private void AddSOField()
    {
        _panel.Remove(_addColonySOButton);
        _panel.Remove(_selectColonySOButton);
        _panel.Add(_selectSOField);
    }

    private void RegisterUIElements()
    {
        _panel = rootVisualElement.Q<VisualElement>("Panel");
        _treeView = rootVisualElement.Q<TreeView>("Hierarchy");
        _addColonySOButton = rootVisualElement.Q<Button>("addColonySOButton");
        _addColonyButton = rootVisualElement.Q<Button>("addColonyButton");
        _addComponentsButton = rootVisualElement.Q<Button>("addComponentsButton");
        _selectColonySOButton = rootVisualElement.Q<Button>("selectColonySOButton");
        _selectSOField = rootVisualElement.Q<ObjectField>("selectSOField");
        _selectSOField.objectType = typeof(ColonySO);
        _selectSOField.allowSceneObjects = false;
        _selectSOField.RegisterValueChangedCallback(RemoveSOField);
    }

    private void RemoveSOField(ChangeEvent<UnityEngine.Object> evt)
    {
        _colonySO = (ColonySO)evt.newValue;
        _panel.Add(_addComponentsButton);
        _panel.Remove(_selectSOField);
    }

    private void RegisterTreeItems()
    {
        _rootTreeItem = new TreeItem(_parentId, label: "Colony");

        TreeViewItemData<TreeItem> rootItem = new TreeViewItemData<TreeItem>(_parentId, _rootTreeItem);
        _rootItems.Add(rootItem);

        _treeView.SetRootItems(_rootItems);
    }

    private void ClearTreeItems()
    {
        _rootItems.Clear();
        TreeViewItemData<TreeItem> rootItem = new TreeViewItemData<TreeItem>(_parentId, _rootTreeItem);
        _rootItems.Add(rootItem);

        _treeView.SetRootItems(_rootItems);
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

    #region Add Custom Classes

    private void AddResource(ResourceSO resourceSO, double resourceAmount, double sellRate, double sellRateMultiplier)
    {
        Resource newResource = new Resource();

        newResource.ResourceSO = resourceSO;
        newResource.ChangeResourceAmount(resourceAmount);
        newResource.ChangeSellRateMultiplier(sellRateMultiplier);
        newResource.ChangeSellRate(sellRate);

        AddChildToItem(_parentId, new TreeItem(GetUniqueId(), label: $"{newResource.ResourceSO.resourceType.ToString()}", reference: newResource.ResourceSO));
        _resources.Add(newResource);
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

        _collectorSO = collector;

        EditorUtility.SetDirty(collector);

        AddChildToItem(_parentId, new TreeItem(GetUniqueId(), label: collector.CollectorName, reference: collector));
    }

    private ColonySO AddColonySO(string colonyName, ColonyType colonyType)
    {
        string path = _SOFolder + "3-Colonies";

        var colony = ScriptableObjectUtility.CreateAssetAtPath<ColonySO>(path, colonyName);
        colony.ColonyType = colonyType;

        EditorUtility.SetDirty(colony);

        AddChildToItem(_parentId, new TreeItem(GetUniqueId(), label: colony.ColonyType.ToString(), reference: colony));

        return colony;
    }

    private void AddResourceSO(string resourceName, ResourceType resourceType, string resourceUnit, Sprite icon)
    {
        string path = _SOFolder + "1-Resources";

        var resource = ScriptableObjectUtility.CreateAssetAtPath<ResourceSO>(path, resourceName);
        resource.resourceType = resourceType;
        resource.ResourceUnit = resourceUnit;
        resource.ResourceIcon = icon;
        _resourceSO = resource;

        EditorUtility.SetDirty(resource);
    }

    #endregion

    #region Events

    private void AddResourceSOUI()
    {
        bool isSecondStage = false;
        _panel.Remove(_addComponentsButton);
        VisualElement resourceSOUI = _resourceSOCard.CloneTree();
        _panel.Add(resourceSOUI);

        VisualElement resourceSOCard = resourceSOUI.Q<VisualElement>("ResourceSOCard");

        var resourceIcon = resourceSOCard.Q<ObjectField>("ResourceIcon");
        resourceIcon.objectType = typeof(Sprite);
        resourceIcon.allowSceneObjects = false;

        var resourceTypeField = resourceSOCard.Q<EnumField>("ResourceType");
        var resourceUnitField = resourceSOCard.Q<TextField>("ResourceUnit");
        var resourceSOCreateButton = resourceSOCard.Q<Button>("Create");
        var resourceSOSelectButton = resourceSOCard.Q<Button>("Select");
        var resourceSOButtonPanel = resourceSOCard.Q<VisualElement>("CreateOrSelectPanel");

        resourceSOCard.Remove(resourceIcon);
        resourceSOCard.Remove(resourceTypeField);
        resourceSOCard.Remove(resourceUnitField);

        if (resourceSOSelectButton.visible)
        {
            resourceSOSelectButton.clicked += () =>
            {
                if (resourceSOCreateButton.visible)
                {
                    resourceSOButtonPanel.Remove(resourceSOCreateButton);

                    resourceSOCreateButton.visible = false;
                    resourceSOSelectButton.visible = false;

                    resourceSOCard.Insert(1, resourceIcon);
                    resourceIcon.label = "Resource SO";
                    resourceIcon.objectType = typeof(ResourceSO);
                    resourceIcon.allowSceneObjects = false;
                    resourceIcon.RegisterValueChangedCallback( (evt) => {
                        _resourceSO = (ResourceSO)evt.newValue;
                        _panel.Remove(resourceSOUI);

                        resourceIcon.value = null;
                        resourceTypeField.value = null;
                        resourceUnitField.value = null;

                        resourceIcon.label = "Sprite";
                        resourceIcon.objectType = typeof(Sprite);
                        resourceIcon.allowSceneObjects = false;

                        AddResourceUI(_resourceSO);
                    });
                }
            };
        }

        resourceSOCreateButton.clicked += () =>
        {
            if (resourceSOSelectButton.visible)
            {
                resourceSOButtonPanel.Remove(resourceSOSelectButton);
                isSecondStage = true;

                resourceSOCard.Insert(1, resourceIcon);
                resourceSOCard.Insert(2, resourceTypeField);
                resourceSOCard.Insert(3, resourceUnitField);
                return;
            }

            AddResourceSO(resourceTypeField.value.ToString(), (ResourceType)resourceTypeField.value, resourceUnitField.value, (Sprite)resourceIcon.value);

            resourceIcon.value = null;
            resourceTypeField.value = null;
            resourceUnitField.value = null;
            _panel.Remove(resourceSOUI);
            AddResourceUI(_resourceSO);
        };
    }

    private void AddResourceUI(ResourceSO newResourceSO)
    {
        VisualElement resourceUI = _resourceCard.CloneTree();

        var resourceSO = resourceUI.Q<ObjectField>("ResourceSO");
        resourceSO.objectType = typeof(ResourceSO);
        resourceSO.allowSceneObjects = false;
        resourceSO.value = newResourceSO;

        var resourceAmountField = resourceUI.Q<Slider>("ResourceAmount");
        var resourceSellRateField = resourceUI.Q<Slider>("SellRate");
        var resourceSellRateMultField = resourceUI.Q<Slider>("SellRateMultiplier");
        var resourceCreateButton = resourceUI.Q<Button>("Create");

        resourceCreateButton.clicked += () =>
        {
            AddResource((ResourceSO)resourceSO.value, resourceAmountField.value, resourceSellRateField.value, resourceSellRateMultField.value);
            _panel.Remove(resourceUI);
            AddCollectorSOUI();
        };

        _panel.Add(resourceUI);
    }

    private void AddCollectorSOUI()
    {
        VisualElement collectorSOUI = _collectorSOCard.CloneTree();

        var generatedResourceSO = collectorSOUI.Q<ObjectField>("GeneratedResourceSO");
        generatedResourceSO.objectType = typeof(ResourceSO);
        generatedResourceSO.allowSceneObjects = false;
        generatedResourceSO.value = _resourceSO;

        var collectorTypeField = collectorSOUI.Q<EnumField>("CollectorType");
        collectorTypeField.value = GetCollectorEnumValue(_resourceSO);

        var baseCollectionRateField = collectorSOUI.Q<Slider>("BaseCollectionRate");
        var collectorSOCreateButton = collectorSOUI.Q<Button>("Create");

        collectorSOCreateButton.clicked += () =>
        {
            AddCollectorSO(collectorTypeField.value.ToString(), (CollectorType)collectorTypeField.value, (ResourceSO)generatedResourceSO.value, baseCollectionRateField.value);

            AddCollectorUI();
            _panel.Remove(collectorSOUI);
        };
        _panel.Add(collectorSOUI);
    }

    private void AddCollectorUI()
    {
        VisualElement collectorUI = _collectorCard.CloneTree();
        ScrollView scrollView = collectorUI.Q<ScrollView>("CollectorCardScrollView");
        var createCollectorButton = collectorUI.Q<Button>("Create");
        var createCostResourceButton = collectorUI.Q<Button>("CreateCostResourceButton");
        var costResourcesListView = collectorUI.Q<ListView>("CostResourceListView");
        List<CostResource> costResources = new List<CostResource>();


        createCostResourceButton.clicked += () =>
        { 
            VisualElement costResourceUI = _costResourceCard.CloneTree();
            int index = scrollView.IndexOf(createCostResourceButton);
            scrollView.Remove(createCostResourceButton);
            scrollView.Insert(index,costResourceUI);

            var createCostResourceBtn = costResourceUI.Q<Button>("Create");

            var costResourceSO = costResourceUI.Q<UnityEditor.UIElements.ObjectField>("ResourceSO");
            costResourceSO.objectType = typeof(ResourceSO);
            costResourceSO.allowSceneObjects = false;

            var costAmountSlider = costResourceUI.Q<Slider>("CostAmount");
            var baseCostAmountSlider = costResourceUI.Q<Slider>("BaseCostAmount");
            var costMultiplierSlider = costResourceUI.Q<Slider>("CostMultiplier");

            createCostResourceBtn.clicked += () =>
            {
                CostResource newCostResource = new CostResource();
                newCostResource.Resource = (ResourceSO)costResourceSO.value;
                newCostResource.SetCostMultiplier(costMultiplierSlider.value);
                newCostResource.SetBaseCostAmount(baseCostAmountSlider.value);
                newCostResource.SetCostAmount(costAmountSlider.value);

                costResources.Add(newCostResource);
                Label label = new Label();
                label.text = "Cost_1" + newCostResource.Resource.resourceType.ToString();

                costResourcesListView.makeItem = () =>
                {
                    var container = new VisualElement();
                    container.style.flexDirection = FlexDirection.Row;

                    var label = new Label();
                    label.style.flexGrow = 1;
                    container.Add(label);

                    return container;
                };

                costResourcesListView.bindItem = (element, i) =>
                {
                    var label = element.Q<Label>();
                    var item = costResources[i];
                    label.text = $"{item.Resource.name}";
                };
                

                scrollView.Remove(costResourceUI);
                scrollView.Insert(index, createCostResourceButton);

                costResourcesListView.itemsSource = costResources;
                costResourcesListView.fixedItemHeight = 20;
            };
        };



        var SOField = collectorUI.Q<UnityEditor.UIElements.ObjectField>("CollectorSO");
        SOField.objectType = typeof(CollectorSO);
        SOField.allowSceneObjects = false;
        SOField.value = _collectorSO;

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

        createCollectorButton.clicked += () =>
        {
            CollectorData collectorData = new CollectorData((CollectorSO)SOField.value, costResources, rateField.value, rateMultField.value, speedField.value, speedMultField.value, (int)levelField.value, (int)levelIncField.value);
            CollectorModel collector = new();
            collector.Data = new CollectorData();
            collector.Data = collectorData;

            _collectors.Add(collector);

            _panel.Remove(collectorUI);
            _panel.Add(_addColonyButton);
            _panel.Add(_addComponentsButton);
        };

        _panel.Add(collectorUI);
    }

    private void AddColonySOUI()
    {
        _panel.Remove(_addColonySOButton);
        VisualElement colonySOUI = _colonySOCard.CloneTree();
        _panel.Add(colonySOUI);

        var colonyNameField = colonySOUI.Q<TextField>("ColonyName");
        var colonyTypeField = colonySOUI.Q<EnumField>("ColonyType");
        var colonySOCreateButton = colonySOUI.Q<Button>("Create");

        colonySOCreateButton.clicked += () =>
        {
            _colonySO =  AddColonySO(colonyTypeField.value.ToString(), (ColonyType)colonyTypeField.value);
            
            _panel.Add(_addComponentsButton);
            _panel.Remove(colonySOUI);
        };
    }

    #endregion

    #region Not Implemented

    private void AddColony()
    {
        GameObject colony = new GameObject($"{_colonySO.ColonyType}_Colony");
        var colonyModel = colony.AddComponent<ColonyModel>();
        colonyModel.colonyData = _colonySO;
        colonyModel.Collectors = new List<CollectorModel>();
        colonyModel.Resources = new List<Resource>();

        foreach (var collector in _collectors)
        {
            GameObject collectorObject = new GameObject("Collector");
            var collectorModel = collectorObject.AddComponent<CollectorModel>();
            collectorModel.Data = new CollectorData();
            collectorModel.Data = collector.Data;
            collectorObject.name = collectorModel.Data.DataSO.CollectorName;

            collectorObject.transform.SetParent(colony.transform);
            colonyModel.Collectors.Add(collectorModel);
        }

        foreach (var resource in _resources)
        {
            colonyModel.Resources.Add(resource);
        }

        Debug.Log($"Created Colony '{colony.name}' with {_collectors.Count} collectors.");
    }

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

