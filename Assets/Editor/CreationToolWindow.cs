using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Drawing;
using TMPro.EditorUtilities;
using UnityEditor;
using UnityEngine;

public class CreationToolWindow : EditorWindow
{
    private int selectedTab = 0;
    private readonly string[] tabs = { "Colony", "Collectors", "Resources" };

    //Game Objects
    private GameObject _colony;

    //Models
    private ColonyModel _colonyModel;
    private CollectorModel _collectorModel;
    private List<CollectorModel> _collectorModels;


    //Views
    private ColonyView _colonyView;
    private CollectorView _collectorView;

    //Scriptable Objects
    private ResourceSO resourceSO;
    private ResourceSO _moneySO;
    private ColonySO _colonySO;

    private List<ResourceSO> availableResourceSOs = new List<ResourceSO>();
    private List<CollectorModel> availableCollectorModels = new List<CollectorModel>();

    //Collector Data Fields
    

    private List<CollectorConfig> collectorConfigs = new List<CollectorConfig>();
    private Vector2 scrollPos;

    [System.Serializable]
    private class CollectorConfig
    {
        public double _collectionRate;
        public double _collectionRateMultiplier;
        public double _speed;
        public double _speedMultiplier;
        public int _level;
        public int _levelIncrement;
        public CollectorSO _collectorSO;
    }


    [MenuItem("Tools/Creation Tool")]
    public static void ShowWindow()
    {
        GetWindow<CreationToolWindow>("Creation Tool");
    }

    private void OnGUI()
    {
        // Draw toolbar tabs
        selectedTab = GUILayout.Toolbar(selectedTab, tabs);

        GUILayout.Space(10);

        switch (selectedTab)
        {
            case 0:
                DrawColonyTab();
                break;
            case 1:
                DrawCollectorsTab();
                break;
            case 2:
                DrawResourcesTab();
                break;
        }
    }


    private void DrawColonyTab()
    {
        GUILayout.Label("Colony Creation", EditorStyles.boldLabel);

        _colonySO = SelectObjectField<ColonySO>("Select Colony SO", _colonySO, false);

        if (_colonySO == null)
        {
            return;
        }

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        for (int i = 0; i < collectorConfigs.Count; i++)
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label($"Collector {i + 1}", EditorStyles.boldLabel);

            collectorConfigs[i]._collectorSO = SelectObjectField<CollectorSO>("Select Collector", collectorConfigs[i]._collectorSO, false);
            collectorConfigs[i]._collectionRate = EditorGUILayout.Slider(0f, 0f, 2f);
            collectorConfigs[i]._collectionRateMultiplier = EditorGUILayout.DoubleField("Collection Rate Multiplier", collectorConfigs[i]._collectionRateMultiplier);
            collectorConfigs[i]._speed = EditorGUILayout.DoubleField("Collect Speed", collectorConfigs[i]._speed);
            collectorConfigs[i]._speedMultiplier = EditorGUILayout.DoubleField("Collect Speed Multiplier", collectorConfigs[i]._speedMultiplier);
            collectorConfigs[i]._level = EditorGUILayout.IntField("Start Level", collectorConfigs[i]._level);
            collectorConfigs[i]._levelIncrement = EditorGUILayout.IntField("Start Level Increment", collectorConfigs[i]._levelIncrement);

            if (GUILayout.Button("Remove Collector"))
            {
                collectorConfigs.RemoveAt(i);
                break;
            }

            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Add Collector"))
        {
            collectorConfigs.Add(new CollectorConfig());
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Create Colony"))
        {
            _colony = new GameObject($"{_colonySO.ColonyType.ToString()}_Colony");
            _colonyModel = _colony.AddComponent<ColonyModel>();
            _colonyModel.Collectors = new List<CollectorModel>();
            _colonyModel.colonyData = _colonySO;
            CreateColonyWithCollectors();
        }
    }


    private void CreateColonyWithCollectors()
    {
        if (_colony == null)
        {
            Debug.LogError("Please assign a Colony Prefab first!");
            return;
        }

        if (_colonyModel == null)
        {
            Debug.LogError("Colony Prefab must have a ColonyModel component!");
            return;
        }

        _colonyModel.Collectors.Clear();

        for (int i = 0; i < collectorConfigs.Count; i++)
        {
            CollectorConfig config = collectorConfigs[i];

            GameObject collectorObj = new GameObject($"Collector_{i + 1}");
            collectorObj.transform.SetParent(_colony.transform);

            CollectorModel collectorModel = collectorObj.AddComponent<CollectorModel>();
            collectorModel.Data = new CollectorData();
            collectorModel.Data.DataSO = config._collectorSO;
            collectorModel.Data.SetCollectionRate(config._collectionRate);
            collectorModel.Data.SetCollectionRateMultiplier(config._collectionRateMultiplier);
            collectorModel.Data.SetSpeed(config._speed);
            collectorModel.Data.SetSpeedMultiplier(config._speedMultiplier);
            collectorModel.Data.SetLevel(config._level);
            collectorModel.Data.SetLevelIncrement(config._levelIncrement);
            _colonyModel.Collectors.Add(collectorModel);
        }

        Selection.activeGameObject = _colony;
        Debug.Log($"Created colony with {collectorConfigs.Count} collectors.");
    }

    private void DrawCollectorsTab()
    {
        GUILayout.Label("Collectors", EditorStyles.boldLabel);
        
    }

    private void DrawResourcesTab()
    {
        GUILayout.Label("Resources", EditorStyles.boldLabel);
    }

    private T SelectObjectField<T>(string label, T currentObject, bool allowSceneObjects = false)
    where T : UnityEngine.Object
    {
        return (T)EditorGUILayout.ObjectField(label, currentObject, typeof(T), allowSceneObjects);
    }
}
