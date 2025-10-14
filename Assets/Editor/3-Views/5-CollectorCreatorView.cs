using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class CollectorCreatorView
{
    private string _treePath = "Assets/Editor/2-VisualTree/6-CollectorCard.uxml";
    private string _stylePath = "Assets/Editor/1-Style/CollectorCard.uss";
    public VisualTreeAsset TreeAsset {  get; private set; }
    public StyleSheet StyleSheet { get; private set; }
    public VisualElement Parent { get; private set; }

    public ScrollView CollectorCardScrollView { get; private set; }
    public ObjectField CollectorSO { get; private set; }
    public ListView CostResourceListView { get; private set; }
    public Button CreateCostResourceButton { get; private set; }
    public Slider CollectorRateSlider { get; private set; }
    public Slider CollectorRateMultiplierSlider { get; private set; }
    public Slider CollectorSpeedSlider { get; private set; }
    public Slider CollectorSpeedMultiplierSlider { get; private set; }
    public Slider CollectorLevelSlider { get; private set; }
    public Slider CollectorLevelIncrementSlider { get; private set; }
    public Button CreateCollectorButton { get; private set; }
    public VisualElement MainPanel { get; private set; }

    public CollectorCreatorView(VisualElement root)
    {
        BindTreeAsset(root);
        BindUIElements();
    }

    private void BindTreeAsset(VisualElement root)
    {
        this.Parent = root;
        TreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(_treePath);
        StyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(_stylePath);
        VisualElement clonedTree = TreeAsset.CloneTree();
        clonedTree.styleSheets.Add(StyleSheet);
        Parent.Add(clonedTree);
    }

    private void BindUIElements()
    {
        CollectorCardScrollView = Parent.Q<ScrollView>("CollectorCardScrollView");

        CollectorSO = Parent.Q<ObjectField>("CollectorSO");
        CollectorSO.objectType = typeof(CollectorSO);
        CollectorSO.allowSceneObjects = false;

        CostResourceListView = Parent.Q<ListView>("CostResourceListView");
        CreateCostResourceButton = Parent.Q<Button>("CreateCostResourceButton");

        CollectorRateSlider = Parent.Q<Slider>("CollectorRate");
        CollectorRateMultiplierSlider = Parent.Q<Slider>("CollectorRateMultiplier");
        CollectorSpeedSlider = Parent.Q<Slider>("CollectorSpeed");
        CollectorSpeedMultiplierSlider = Parent.Q<Slider>("CollectorSpeedMultiplier");
        CollectorLevelSlider = Parent.Q<Slider>("CollectorLevel");
        CollectorLevelIncrementSlider = Parent.Q<Slider>("CollectorLevelIncrement");

        CreateCollectorButton = Parent.Q<Button>("Create");

        MainPanel = Parent.Q<VisualElement>("CollectorCard");
    }
}