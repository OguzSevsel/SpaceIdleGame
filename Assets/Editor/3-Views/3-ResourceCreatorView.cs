using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class ResourceCreatorView
{
    private string _treePath = "Assets/Editor/2-VisualTree/4-ResourceCard.uxml";
    private string _stylePath = "Assets/Editor/1-Style/ResourceSOCard.uss";
    public VisualTreeAsset TreeAsset { get; private set; }
    public StyleSheet StyleSheet { get; private set; }
    public VisualElement Parent { get; private set; }

    public ObjectField ResourceSOObjectField { get; private set; }
    public Slider ResourceAmountSlider { get; private set; }
    public Slider SellRateSlider { get; private set; }
    public Slider SellRateMultiplierSlider { get; private set; }
    public Button CreateResourceButton { get; private set; }
    public VisualElement MainPanel { get; private set; }

    public ResourceCreatorView(VisualElement root)
    {
        BindTreeAsset(root);
        BindUIElements();
    }

    private void BindUIElements()
    {
        ResourceSOObjectField = Parent.Q<ObjectField>("ResourceSO");
        ResourceSOObjectField.objectType = typeof(ResourceSO);
        ResourceSOObjectField.allowSceneObjects = false;

        ResourceAmountSlider = Parent.Q<Slider>("ResourceAmount");
        SellRateSlider = Parent.Q<Slider>("SellRate");
        SellRateMultiplierSlider = Parent.Q<Slider>("SellRateMultiplier");
        CreateResourceButton = Parent.Q<Button>("Create");
        MainPanel = Parent.Q<VisualElement>("ResourceSOCard");
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
}
