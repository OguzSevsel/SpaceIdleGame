using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Graphs.Styles;

public enum State
{
    CreateColonyFromZero,

    ColonySOSelection,
    CreateColonySO,
    SelectColonySO,

    ResourceSOSelection,
    CreateResourceSO,
    SelectResourceSO,

    CreateResource,

    CollectorSOSelection,
    CreateCollectorSO,
    SelectCollectorSO,

    OnlyCreateCollector,
}

public class StateController
{
    public State State {  get; private set; }

    public event Action<State> OnStateChanged;

    public StateController()
    {
        this.State = State.CreateColonyFromZero;
    }

    public void ChangeState(State newState)
    {
        this.State = newState;
        OnStateChanged?.Invoke(this.State);
    }
}


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


    private string _SOFolder = "Assets/ScriptableObjects/";


    #region Custom Classes

    private List<CollectorModel> _collectors = new List<CollectorModel>();
    private List<Resource> _resources = new List<Resource>();

    private ColonySO _colonySO;
    private ResourceSO _resourceSO;
    private CollectorSO _collectorSO;

    private ColonyManager _colonyManager;
    private GameObject _mainView;
    private GameObject _colonyPrefab;
    private GameObject _tabGroupObject;
    private StateController _stateController;

    #endregion


    #region Views

    private ColonyCreatorView main;

    #endregion


    #region Panel Indexes

    private int _colonySOIndex = 0;
    private int _resourceSOIndex = 1;
    private int _resourceIndex = 2;
    private int _collectorSOIndex = 0;
    private int _collectorIndex = 1;

    #endregion

    public void CreateGUI()
    {
        main = new ColonyCreatorView(rootVisualElement);

        RegisterSceneObjects();
        RegisterCallBacks();

        main.LeftPanel.Remove(main.ColonySoCard);
        main.LeftPanel.Remove(main.ResourceSOCard);
        main.LeftPanel.Remove(main.ResourceCard);
        main.RightPanel.Remove(main.CollectorSOCard);
        main.RightPanel.Remove(main.CollectorCard);
        main.Parent.style.flexShrink = 0;
        main.Parent.style.flexGrow = 1;
    }

    private void StateChangedHandler(State state)
    {
        switch (state)
        {
            default:
                break;
        }
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
    private void RegisterSceneObjects()
    {
        _colonyManager = FindAnyObjectByType<ColonyManager>();
        _mainView = GameObject.Find("MainView");
        _colonyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/Colony/Panel_Colony.prefab");
        _tabGroupObject = GameObject.Find("BottomLeftPanel");
    }
    private void RegisterCallBacks()
    {
        main.ColonyMenuBtn.clicked += ColonyMenuButtonClickHandler;

        main.ColonySOMenuBtn.clicked += ColonySOMenuButtonClickHandler;
        main.ColonySOCloseBtn.clicked += ColonySOCloseButtonClickHandler;
        main.ColonySOSelectBtn.clicked += ColonySOSelectButtonClickHandler;
        main.ColonySOCreateBtn.clicked += ColonySOCreateButtonClickHandler;

        main.ResourceSOMenuBtn.clicked += ResourceSOMenuButtonClickHandler;
        main.ResourceSOCloseBtn.clicked += ResourceSOCloseButtonClickHandler;
        main.ResourceSOCreateBtn.clicked += ResourceSOCreateButtonClickHandler;

        main.ResourceMenuBtn.clicked += ResourceMenuButtonClickHandler;
        main.ResourceCloseBtn.clicked += ResourceCloseButtonClickHandler;
        main.ResourceCreateBtn.clicked += ResourceCreateButtonClickHandler;

        main.CollectorSOMenuBtn.clicked += CollectorSOMenuButtonClickHandler;
        main.CollectorSOCloseBtn.clicked += CollectorSOCloseButtonClickHandler;
        main.CollectorSOCreateBtn.clicked += CollectorSOCreateButtonClickHandler;

        main.CollectorMenuBtn.clicked += CollectorMenuButtonClickHandler;
        main.CollectorCloseBtn.clicked += CollectorCloseButtonClickHandler;
        main.CollectorCreateBtn.clicked += CollectorCreateButtonClickHandler;
        main.CostResourceCreateBtn.clicked += CostResourceCreateButtonClickHandler;
    }

    #endregion

    #region Colony Button Handlers

    private void ColonyMenuButtonClickHandler()
    {
        throw new NotImplementedException();
    }

    #endregion

    #region ColonySO Button Handlers

    private void ColonySOMenuButtonClickHandler()
    {
        //main.InsertToLeft(_colonySOIndex, main.ColonySoCard);
        main.LeftPanel.Add(main.ColonySoCard);
    }

    private void ColonySOCloseButtonClickHandler()
    {
        throw new NotImplementedException();
    }

    private void ColonySOSelectButtonClickHandler()
    {
        throw new NotImplementedException();
    }

    private void ColonySOCreateButtonClickHandler()
    {
        throw new NotImplementedException();
    }

    #endregion

    #region ResourceSO Button Handlers

    private void ResourceSOMenuButtonClickHandler()
    {
        main.InsertToLeft(_resourceSOIndex, main.ResourceSOCard);
    }

    private void ResourceSOCloseButtonClickHandler()
    {
        throw new NotImplementedException();
    }

    private void ResourceSOCreateButtonClickHandler()
    {
        throw new NotImplementedException();
    }

    #endregion

    #region Resource Button Handlers

    private void ResourceMenuButtonClickHandler()
    {
        main.InsertToLeft(_resourceIndex, main.ResourceCard);
    }

    private void ResourceCloseButtonClickHandler()
    {
        throw new NotImplementedException();
    }

    private void ResourceCreateButtonClickHandler()
    {
        throw new NotImplementedException();
    }

    #endregion

    #region CollectorSO Button Handlers

    private void CollectorSOMenuButtonClickHandler()
    {
        main.InsertToRight(_collectorSOIndex, main.CollectorSOCard);
    }

    private void CollectorSOCloseButtonClickHandler()
    {
        throw new NotImplementedException();
    }

    private void CollectorSOCreateButtonClickHandler()
    {
        throw new NotImplementedException();
    }

    #endregion

    #region Collector Button Handlers

    private void CollectorMenuButtonClickHandler()
    {
        main.InsertToRight(_collectorIndex, main.CollectorCard);
    }

    private void CollectorCloseButtonClickHandler()
    {
        throw new NotImplementedException();
    }

    private void CostResourceCreateButtonClickHandler()
    {
        throw new NotImplementedException();
    }

    private void CollectorCreateButtonClickHandler()
    {
        throw new NotImplementedException();
    }

    #endregion


    private ColonySO AddColonySO(string colonyName, ColonyType colonyType)
    {
        string path = _SOFolder + "3-Colonies";

        bool isAssetExists = CheckAssetNamesAtPath(path, colonyType.ToString());

        if (!isAssetExists)
        {
            var colony = ScriptableObjectUtility.CreateAssetAtPath<ColonySO>(path, colonyName);
            colony.ColonyType = colonyType;
            EditorUtility.SetDirty(colony);
            return colony;
        }
        else
        {
            EditorUtility.DisplayDialog("Asset Conflict!", "There is an asset with same kind", "OK");
        }
        return null;
    }
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


        GameObject colonyPrefab = Instantiate(_colonyPrefab);
        colonyPrefab.transform.SetParent(_mainView.transform);

        if (_colonyManager != null)
        {
            _colonyManager.AddToColonyModels(colonyModel);
            _colonyManager.AddToColonyViews(colonyPrefab.GetComponent<ColonyView>());
        }

        _tabGroupObject.GetComponent<TabGroup>().objectsToSwap.Add(colonyPrefab);

        Debug.Log($"Created Colony '{colony.name}' with {_collectors.Count} collectors.");
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


    private void AddResource(ResourceSO resourceSO, double resourceAmount, double sellRate, double sellRateMultiplier)
    {
        Resource newResource = new Resource();

        newResource.ResourceSO = resourceSO;
        newResource.ChangeResourceAmount(resourceAmount);
        newResource.ChangeSellRateMultiplier(sellRateMultiplier);
        newResource.ChangeSellRate(sellRate);

        _resources.Add(newResource);
    }


    private void AddCollectorSO(string collectorName, CollectorType collectorType, ResourceSO resource, double baseCollectionRate)
    {
        string path = _SOFolder + "2-Collectors";

        var collectorSO = ScriptableObjectUtility.CreateAssetAtPath<CollectorSO>(path, collectorName);

        collectorSO.CollectorName = collectorName;
        collectorSO.CollectorType = collectorType;
        collectorSO.GeneratedResource = resource;
        collectorSO._baseCollectionRate = baseCollectionRate;

        _collectorSO = collectorSO;

        EditorUtility.SetDirty(collectorSO);
    }


    private void AddCostResources(List<CostResource> costResources, VisualElement costResourceUI, int index)
    {
        CostResource newCostResource = new CostResource();
        newCostResource.Resource = (ResourceSO)main.CostResourceObject.value;
        newCostResource.SetCostMultiplier(main.CostResourceCostMultSlider.value);
        newCostResource.SetBaseCostAmount(main.CostResourceBaseCostAmountSlider.value);
        newCostResource.SetCostAmount(main.CostResourceCostAmountSlider.value);

        costResources.Add(newCostResource);
        Label label = new Label();
        label.text = "Cost_1" + newCostResource.Resource.resourceType.ToString();

        main.CollectorCostResourceListView.makeItem = () =>
        {
            var container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;

            var label = new Label();
            label.style.flexGrow = 1;
            container.Add(label);

            return container;
        };

        main.CollectorCostResourceListView.bindItem = (element, i) =>
        {
            var label = element.Q<Label>();
            var item = costResources[i];
            label.text = $"{item.Resource.name}";
        };

        main.CollectorScrollView.Remove(costResourceUI);
        main.CollectorScrollView.Insert(index, main.CostResourceCreateBtn);
        main.CollectorCostResourceListView.itemsSource = costResources;
        main.CollectorCostResourceListView.fixedItemHeight = 20;
    }
    private void AddCollector(List<CostResource> costResources)
    {
        CollectorData collectorData = new CollectorData
            (
                (CollectorSO)main.CollectorSOObject.value,
                costResources, main.CollectorRateSlider.value,
                main.CollectorRateMultSlider.value,
                main.CollectorSpeedMultSlider.value,
                main.CollectorSpeedMultSlider.value,
                main.CollectorLevelSlider.value,
                main.CollectorLevelIncrementSlider.value
            );

        CollectorModel collectorModel = new()
        {
            Data = new CollectorData()
        };
        collectorModel.Data = collectorData;

        _collectors.Add(collectorModel);
    }









    private void AddColonySOUI()
    {
        //_colonySOCreator.ColonyTypeEnumField.RegisterValueChangedCallback((ChangeEvent<System.Enum> evt) =>
        //{
        //    if (_colonySO != null)
        //    {
        //        string earlyPath = _SOFolder + "3-Colonies";
        //        string path = _SOFolder + $"3-Colonies/{_colonySO.ColonyType.ToString()}.asset";

        //        var colonySO = AssetDatabase.LoadAssetAtPath<ColonySO>(path);

        //        ColonyType newColonyType = (ColonyType)evt.newValue;

        //        bool isAssetExists = CheckAssetNamesAtPath(earlyPath, newColonyType.ToString());

        //        if (!isAssetExists)
        //        {
        //            _colonySO.ColonyType = (ColonyType)evt.newValue;
        //            colonySO.ColonyType = (ColonyType)evt.newValue;

        //            string error = AssetDatabase.RenameAsset(path, colonySO.ColonyType.ToString());
        //            AssetDatabase.SaveAssets();
        //            AssetDatabase.Refresh();

        //            _colonySO.name = _colonySO.ColonyType.ToString();
        //        }
        //        else
        //        {
        //            EditorUtility.DisplayDialog("Asset Conflict", "There is an asset with the same kind operation is not completed", "OK");
        //        }
        //    }
        //});
    }

    private void HandleResourceSelect()
    {
        //_resourceSOCreator.ResourceIconObjectField.RegisterValueChangedCallback((evt) => {
        //    _resourceSO = (ResourceSO)evt.newValue;

        //    _resourceSOCreator.ResourceTypeEnumField.value = null;
        //    _resourceSOCreator.ResourceUnitTextField.value = null;

        //    _stateController.ChangeState(State.CreateResource);
        //});
    }
    private bool CheckAssetNamesAtPath(string folderPath, string assetName)
    {
        string[] guids = AssetDatabase.FindAssets(assetName, new[] { folderPath });

        bool isAssetExists = false;

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
            if (asset != null && asset.name == assetName)
                isAssetExists = true;
        }

        return isAssetExists;
    }
}

#region Util Classes

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
