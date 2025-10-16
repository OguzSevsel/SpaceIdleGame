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
    private List<CostResource> _costResources = new List<CostResource>();

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


    public void CreateGUI()
    {
        main = new ColonyCreatorView(rootVisualElement);

        RegisterSceneObjects();
        RegisterCallBacks();

        main.ColonySoCard.style.display = DisplayStyle.None;
        main.ResourceSOCard.style.display = DisplayStyle.None;
        main.ResourceCard.style.display = DisplayStyle.None;
        main.CollectorSOCard.style.display = DisplayStyle.None;
        main.CollectorCard.style.display = DisplayStyle.None;
        main.ColonySOSelectObject.style.display = DisplayStyle.None;

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

        main.TreeViewAddChildButton.clicked += TreeViewAddChildClickHandler;
        main.TreeViewAddRootButton.clicked += TreeViewAddRootClickHandler;
    }

    private void TreeViewAddChildClickHandler()
    {
        ColonySO newColony = AddColonySO("deneme", ColonyType.Ceres);
        main.AddChildToItemWithObjs(newColony, _colonySO);
    }

    private void TreeViewAddRootClickHandler()
    {
        main.AddRootItemToTree(_colonySO);
    }

    private void ShowControl(VisualElement control)
    {
        control.style.display = DisplayStyle.Flex;
    }
    private void HideControl(VisualElement control)
    {
        control.style.display = DisplayStyle.None;
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

    #endregion

    #region Menu Button Handlers

    private void ColonySOMenuButtonClickHandler()
    {
        ShowControl(main.ColonySoCard);
    }

    private void ResourceSOMenuButtonClickHandler()
    {
        ShowControl(main.ResourceSOCard);
    }

    private void ResourceMenuButtonClickHandler()
    {
        ShowControl(main.ResourceCard);
    }

    private void CollectorSOMenuButtonClickHandler()
    {
        ShowControl(main.CollectorSOCard);
    }

    private void CollectorMenuButtonClickHandler()
    {
        ShowControl(main.CollectorCard);
    }

    #endregion

    #region Close Button Handlers

    private void ColonySOCloseButtonClickHandler()
    {
        HideControl(main.ColonySOSelectObject);
        HideControl(main.ColonySoCard);
        ShowControl(main.ColonySOTypeEnum);
        ShowControl(main.ColonySOCreateBtn);
        ShowControl(main.ColonySOSelectBtn);
    }

    private void ResourceSOCloseButtonClickHandler()
    {
        HideControl(main.ResourceSOCard);
    }

    private void ResourceCloseButtonClickHandler()
    {
        HideControl(main.ResourceCard);
    }

    private void CollectorSOCloseButtonClickHandler()
    {
        HideControl(main.CollectorSOCard);
    }

    private void CollectorCloseButtonClickHandler()
    {
        HideControl(main.CollectorCard);
    }

    #endregion


    

    #region ColonySO Handlers

    private void ColonySOSelectButtonClickHandler()
    {
        ShowControl(main.ColonySOSelectObject);
        HideControl(main.ColonySOTypeEnum);
        HideControl(main.ColonySOCreateBtn);
        HideControl(main.ColonySOSelectBtn);
        main.ColonySOSelectObject.RegisterValueChangedCallback(ColonySOObjectValueChangedHandler);
    }

    private void ColonySOObjectValueChangedHandler(ChangeEvent<UnityEngine.Object> evt)
    {
        HideControl(main.ColonySoCard);
        HideControl(main.ColonySOSelectObject);
        ShowControl(main.ColonySOTypeEnum);
        ShowControl(main.ColonySOCreateBtn);
        ShowControl(main.ColonySOSelectBtn);
        _colonySO = evt.newValue as ColonySO;
    }

    private void ColonySOCreateButtonClickHandler()
    {
        _colonySO = AddColonySO(main.ColonySOTypeEnum.value.ToString(), (ColonyType)main.ColonySOTypeEnum.value);
    }

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
            EditorUtility.DisplayDialog("Asset Exists!", "Operation is not completed.", "OK");
        }
        return null;
    }

    #endregion

    #region ResourceSO Button Handlers

    private void ResourceSOCreateButtonClickHandler()
    {
        AddResourceSO(main.ResourceSOTypeEnum.value.ToString(), (ResourceType)main.ResourceSOTypeEnum.value, main.ResourceSOUnitText.value, (Sprite)main.ResourceSOIcon.value);
    }

    private void AddResourceSO(string resourceName, ResourceType resourceType, string resourceUnit, Sprite icon)
    {
        string path = _SOFolder + "1-Resources";

        bool isAssetExists = CheckAssetNamesAtPath(path, resourceType.ToString());

        if (!isAssetExists)
        {
            var resource = ScriptableObjectUtility.CreateAssetAtPath<ResourceSO>(path, resourceName);
            resource.resourceType = resourceType;
            resource.ResourceUnit = resourceUnit;
            resource.ResourceIcon = icon;
            _resourceSO = resource;
            EditorUtility.SetDirty(resource);
        }
        else
        {
            EditorUtility.DisplayDialog("Asset Exists!", "Operation is not completed.", "OK");
        }
    }

    #endregion

    #region Resource Button Handlers

    private void ResourceCreateButtonClickHandler()
    {
        AddResource((ResourceSO)main.ResourceObject.value, main.ResourceAmountSlider.value, main.ResourceSellRateSlider.value, main.ResourceSellRateMultSlider.value);
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


    #endregion

    #region CollectorSO Button Handlers

    private void CollectorSOCreateButtonClickHandler()
    {
        AddCollectorSO(main.CollectorSOTypeEnum.value.ToString(), (CollectorType)main.CollectorSOTypeEnum.value, (ResourceSO)main.CollectorSOResourceObject.value, main.CollectorSOBaseRate.value);
    }

    private void AddCollectorSO(string collectorName, CollectorType collectorType, ResourceSO resource, double baseCollectionRate)
    {
        string path = _SOFolder + "2-Collectors";

        bool isAssetExists = CheckAssetNamesAtPath(path, collectorName);

        if (!isAssetExists)
        {
            var collectorSO = ScriptableObjectUtility.CreateAssetAtPath<CollectorSO>(path, collectorName);
            collectorSO.CollectorName = collectorName;
            collectorSO.CollectorType = collectorType;
            collectorSO.GeneratedResource = resource;
            collectorSO._baseCollectionRate = baseCollectionRate;

            _collectorSO = collectorSO;

            EditorUtility.SetDirty(collectorSO);
        }
        else
        {
            EditorUtility.DisplayDialog("Asset Exists!", "Operation is not completed.", "OK");
        }
    }

    #endregion

    #region Collector Button Handlers

    private void CostResourceCreateButtonClickHandler()
    {
        AddCostResources(_costResources);
        main.CostResourceObject.value = null;
        main.CostResourceCostAmountSlider.value = 0;
        main.CostResourceBaseCostAmountSlider.value = 0;
        main.CostResourceCostMultSlider.value = 1;
    }

    private void CollectorCreateButtonClickHandler()
    {
        AddCollector(_costResources);
        _costResources.Clear();

        main.CollectorSOObject.value = null;
        main.CollectorRateSlider.value = 1;
        main.CollectorRateMultSlider.value = 1;
        main.CollectorSpeedSlider.value = 0.95f;
        main.CollectorSpeedMultSlider.value = 1;
        main.CollectorLevelSlider.value = 1;
        main.CollectorLevelIncrementSlider.value = 1;
        main.CollectorScrollView.ScrollTo(main.CollectorCloseBtn);
    }

    private void AddCostResources(List<CostResource> costResources)
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

    #endregion







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
