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

    private ColonyCreatorView _main;
    private ColonySOCreatorView _colonySOCreator;
    private ResourceSOCreatorView _resourceSOCreator;
    private ResourceCreatorView _resourceCreator;
    private CollectorSOCreatorView _collectorSOCreator;
    private CollectorCreatorView _collectorCreator;
    private CostResourceCreatorView _costResourceCreator;

    #endregion


    public void CreateGUI()
    {
        _stateController = new StateController();
        _stateController.OnStateChanged += StateChangedHandler;

        if (_stateController.State == State.CreateColonyFromZero)
        {
            _main = new ColonyCreatorView(rootVisualElement);
            RegisterCallBacks();
            RegisterSceneObjects();
            _main.RemoveFromPanel(_main.AddComponentsButton);
            _main.RemoveFromPanel(_main.AddColonyButton);
            _main.RemoveFromPanel(_main.SelectColonySOField);
            _stateController.ChangeState(State.ColonySOSelection);
        }
    }

    private void StateChangedHandler(State state)
    {
        switch (state)
        {
            case State.CreateColonyFromZero:
                break;

            case State.ColonySOSelection:
                break;

            case State.CreateColonySO:
                AddColonySOUI();
                break;

            case State.SelectColonySO:
                _main.RemoveFromPanel(_main.SelectColonySOButton);
                _main.RemoveFromPanel(_main.AddColonySOButton);
                _main.AddToPanel(_main.SelectColonySOField);
                break;

            case State.ResourceSOSelection:
                AddResourceSOUI();
                break;

            case State.CreateResourceSO:
                break;

            case State.SelectResourceSO:
                break;

            case State.OnlyCreateCollector:
                _main.ClearTab(_main.CreateCollectorTab);
                AddCollectorUI();
                break;

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

    private void RegisterCallBacks()
    {
        _main.AddColonyButton.clicked += () => AddColony();
        _main.AddColonySOButton.clicked += () => StateChanged(State.CreateColonySO);
        _main.AddComponentsButton.clicked += () => StateChanged(State.ResourceSOSelection);
        _main.SelectColonySOButton.clicked += () => StateChanged(State.SelectColonySO);
        _main.TabView.activeTabChanged += ActiveTabChangedHandler;
        _main.SelectColonySOField.RegisterValueChangedCallback(RemoveSOField);
    }

    private void StateChanged(State newState)
    {
        _stateController.ChangeState(newState);
    }

    private void ActiveTabChangedHandler(Tab tab1, Tab tab2)
    {
        string tabName = _main.TabView.activeTab.name;

        if (tabName == _main.CreateColonyTab.name)
        {
            _stateController.ChangeState(State.CreateColonyFromZero);
        }
        else if (tabName == _main.CreateCollectorTab.name)
        {
            _stateController.ChangeState(State.OnlyCreateCollector);
        }
    }

    private void RegisterSceneObjects()
    {
        _colonyManager = FindAnyObjectByType<ColonyManager>();
        _mainView = GameObject.Find("MainView");
        _colonyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/Colony/Panel_Colony.prefab");
        _tabGroupObject = GameObject.Find("BottomLeftPanel");
    }

    private void RemoveSOField(ChangeEvent<UnityEngine.Object> evt)
    {
        _colonySO = (ColonySO)evt.newValue;
        _main.AddToPanel(_main.AddComponentsButton);
        _main.RemoveFromPanel(_main.SelectColonySOField);
    }

    #endregion


    #region ColonyCreation

    private ColonySO AddColonySO(string colonyName, ColonyType colonyType)
    {
        string path = _SOFolder + "3-Colonies";

        var colony = ScriptableObjectUtility.CreateAssetAtPath<ColonySO>(path, colonyName);
        colony.ColonyType = colonyType;

        EditorUtility.SetDirty(colony);

        return colony;
    }

    private void AddColonySOUI()
    {
        VisualElement colonySOUI = new VisualElement();
        _colonySOCreator = new ColonySOCreatorView(colonySOUI);

        _main.AddToPanel(_colonySOCreator.Parent);

        _colonySOCreator.CreateColonySOButton.clicked += () =>
        {
            _colonySO = AddColonySO(_colonySOCreator.ColonyTypeEnumField.value.ToString(), (ColonyType)_colonySOCreator.ColonyTypeEnumField.value);
            _colonySOCreator.MainPanel.Remove(_colonySOCreator.CreateColonySOButton);
            _stateController.ChangeState(State.ResourceSOSelection);
        };

        _colonySOCreator.ColonyTypeEnumField.RegisterValueChangedCallback((ChangeEvent<System.Enum> evt) =>
        {
            if (_colonySO != null)
            {
                string path = _SOFolder + $"3-Colonies/{_colonySO.ColonyType.ToString()}.asset";

                var colonySO = AssetDatabase.LoadAssetAtPath<ColonySO>(path);
                _colonySO.ColonyType = (ColonyType)evt.newValue;
                colonySO.ColonyType = (ColonyType)evt.newValue;

                string error = AssetDatabase.RenameAsset(path, colonySO.ColonyType.ToString());
                Debug.LogError(error);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                
                _colonySO.name = _colonySO.ColonyType.ToString();
            }
        });
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

    #endregion


    #region CollectorCreation

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

    private void AddCollectorSOUI()
    {
        VisualElement collectorSOUI = new VisualElement();
        _collectorSOCreator = new CollectorSOCreatorView(collectorSOUI);

        _collectorSOCreator.GeneratedResourceSOField.value = _resourceSO;

        _collectorSOCreator.CollectorTypeEnumField.value = GetCollectorEnumValue(_resourceSO);

        _collectorSOCreator.SelectCollectorSOButton.clicked += () =>
        {

        };

        _collectorSOCreator.CreateCollectorSOButton.clicked += () =>
        {
            AddCollectorSO
            (
                _collectorSOCreator.CollectorTypeEnumField.value.ToString(),
                (CollectorType)_collectorSOCreator.CollectorTypeEnumField.value,
                (ResourceSO)_collectorSOCreator.GeneratedResourceSOField.value,
                _collectorSOCreator.BaseCollectionRateSlider.value);

            AddCollectorUI();
        };
    }

    private void AddCollectorUI()
    {
        VisualElement collectorUI = new VisualElement();
        _collectorCreator = new CollectorCreatorView(collectorUI);
        List<CostResource> costResources = new List<CostResource>();
        _main.CreateCollectorTab.Add(collectorUI);

        _collectorCreator.CreateCostResourceButton.clicked += () =>
        {
            VisualElement costResourceUI = new VisualElement();
            _costResourceCreator = new CostResourceCreatorView(costResourceUI);
            int index = _collectorCreator.CollectorCardScrollView.IndexOf(_collectorCreator.CreateCostResourceButton);
            _collectorCreator.CollectorCardScrollView.Remove(_collectorCreator.CreateCostResourceButton);
            _collectorCreator.CollectorCardScrollView.Insert(index, costResourceUI);

            _costResourceCreator.CreateCostResourceButton.clicked += () =>
            {
                AddCostResources(costResources, costResourceUI, index);
            };
        };

        _collectorCreator.CollectorSO.value = _collectorSO;

        _collectorCreator.CreateCollectorButton.clicked += () =>
        {
            AddCollector(costResources);
        };
    }

    private void AddCostResources(List<CostResource> costResources, VisualElement costResourceUI, int index)
    {
        CostResource newCostResource = new CostResource();
        newCostResource.Resource = (ResourceSO)_costResourceCreator.ResourceSO.value;
        newCostResource.SetCostMultiplier(_costResourceCreator.CostMultiplierSlider.value);
        newCostResource.SetBaseCostAmount(_costResourceCreator.BaseCostAmountSlider.value);
        newCostResource.SetCostAmount(_costResourceCreator.CostAmountSlider.value);

        costResources.Add(newCostResource);
        Label label = new Label();
        label.text = "Cost_1" + newCostResource.Resource.resourceType.ToString();

        _collectorCreator.CostResourceListView.makeItem = () =>
        {
            var container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;

            var label = new Label();
            label.style.flexGrow = 1;
            container.Add(label);

            return container;
        };

        _collectorCreator.CostResourceListView.bindItem = (element, i) =>
        {
            var label = element.Q<Label>();
            var item = costResources[i];
            label.text = $"{item.Resource.name}";
        };

        _collectorCreator.CollectorCardScrollView.Remove(costResourceUI);
        _collectorCreator.CollectorCardScrollView.Insert(index, _collectorCreator.CreateCostResourceButton);
        _collectorCreator.CostResourceListView.itemsSource = costResources;
        _collectorCreator.CostResourceListView.fixedItemHeight = 20;
    }

    private void AddCollector(List<CostResource> costResources)
    {
        CollectorData collectorData = new CollectorData
            (
                (CollectorSO)_collectorCreator.CollectorSO.value,
                costResources, _collectorCreator.CollectorRateSlider.value,
                _collectorCreator.CollectorRateMultiplierSlider.value,
                _collectorCreator.CollectorSpeedSlider.value,
                _collectorCreator.CollectorSpeedMultiplierSlider.value,
                (int)_collectorCreator.CollectorLevelSlider.value,
                (int)_collectorCreator.CollectorLevelIncrementSlider.value
            );

        CollectorModel collectorModel = new()
        {
            Data = new CollectorData()
        };
        collectorModel.Data = collectorData;

        _collectors.Add(collectorModel);
    }

    #endregion


    #region ResourceCreation

    private void AddResource(ResourceSO resourceSO, double resourceAmount, double sellRate, double sellRateMultiplier)
    {
        Resource newResource = new Resource();

        newResource.ResourceSO = resourceSO;
        newResource.ChangeResourceAmount(resourceAmount);
        newResource.ChangeSellRateMultiplier(sellRateMultiplier);
        newResource.ChangeSellRate(sellRate);

        _resources.Add(newResource);
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

    private void AddResourceSOUI()
    {
        VisualElement resourceSOUI = new VisualElement();
        _resourceSOCreator = new ResourceSOCreatorView(resourceSOUI);

        _resourceSOCreator.MainPanel.Remove(_resourceSOCreator.ResourceIconObjectField);
        _resourceSOCreator.MainPanel.Remove(_resourceSOCreator.ResourceTypeEnumField);
        _resourceSOCreator.MainPanel.Remove(_resourceSOCreator.ResourceUnitTextField);
        _main.CreateColonyTab.Add(_resourceSOCreator.Parent);

        if (_resourceSOCreator.SelectResourceSOButton.visible)
        {
            _resourceSOCreator.SelectResourceSOButton.clicked += () =>
            {
                if (_resourceSOCreator.CreateResourceSOButton.visible)
                {
                    _resourceSOCreator.ButtonPanel.Remove(_resourceSOCreator.CreateResourceSOButton);

                    _resourceSOCreator.CreateResourceSOButton.visible = false;
                    _resourceSOCreator.SelectResourceSOButton.visible = false;

                    _resourceSOCreator.MainPanel.Insert(1, _resourceSOCreator.ResourceIconObjectField);

                    _resourceSOCreator.ResourceIconObjectField.label = "Resource SO";
                    _resourceSOCreator.SetObjectFieldType(isSprite: false);

                    _resourceSOCreator.ResourceIconObjectField.RegisterValueChangedCallback((evt) => {
                        _resourceSO = (ResourceSO)evt.newValue;

                        _resourceSOCreator.ResourceIconObjectField.value = null;
                        _resourceSOCreator.ResourceTypeEnumField.value = null;
                        _resourceSOCreator.ResourceUnitTextField.value = null;

                        _resourceSOCreator.ResourceIconObjectField.label = "Sprite";
                        _resourceSOCreator.SetObjectFieldType(isSprite: true);

                        AddResourceUI(_resourceSO);
                    });
                }
            };
        }

        _resourceSOCreator.CreateResourceSOButton.clicked += () =>
        {
            if (_resourceSOCreator.SelectResourceSOButton.visible)
            {
                _resourceSOCreator.ButtonPanel.Remove(_resourceSOCreator.SelectResourceSOButton);

                _resourceSOCreator.MainPanel.Insert(1, _resourceSOCreator.ResourceIconObjectField);
                _resourceSOCreator.MainPanel.Insert(2, _resourceSOCreator.ResourceTypeEnumField);
                _resourceSOCreator.MainPanel.Insert(3, _resourceSOCreator.ResourceUnitTextField);
                return;
            }

            AddResourceSO(_resourceSOCreator.ResourceTypeEnumField.value.ToString(), (ResourceType)_resourceSOCreator.ResourceTypeEnumField.value, _resourceSOCreator.ResourceUnitTextField.value, (Sprite)_resourceSOCreator.ResourceIconObjectField.value);

            _resourceSOCreator.ResourceIconObjectField.value = null;
            _resourceSOCreator.ResourceTypeEnumField.value = null;
            _resourceSOCreator.ResourceUnitTextField.value = null;
            AddResourceUI(_resourceSO);
        };
    }

    private void AddResourceUI(ResourceSO newResourceSO)
    {
        VisualElement resourceUI = new VisualElement();
        _resourceCreator = new ResourceCreatorView(resourceUI);

        _resourceCreator.ResourceSOObjectField.value = newResourceSO;

        _resourceCreator.CreateResourceButton.clicked += () =>
        {
            AddResource
            (
                (ResourceSO)_resourceCreator.ResourceSOObjectField.value,
                _resourceCreator.ResourceAmountSlider.value, _resourceCreator.SellRateSlider.value,
                _resourceCreator.SellRateMultiplierSlider.value
            );
            AddCollectorSOUI();
        };
    }

    #endregion
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
