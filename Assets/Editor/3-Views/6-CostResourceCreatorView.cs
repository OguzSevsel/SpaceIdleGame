using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class CostResourceCreatorView
{
    private string _treePath = "Assets/Editor/2-VisualTree/7-CostResourceCard.uxml";
    private string _stylePath = "Assets/Editor/1-Style/ResourceSOCard.uss";
    public VisualTreeAsset TreeAsset { get; private set; }
    public StyleSheet StyleSheet { get; private set; }
    public VisualElement Parent { get; private set; }

    public ObjectField ResourceSO { get; private set; }
    public Slider CostAmountSlider { get; private set; }
    public Slider BaseCostAmountSlider { get; private set; }
    public Slider CostMultiplierSlider { get; private set; }
    public Button CreateCostResourceButton { get; private set; }
    public VisualElement MainPanel { get; private set; }

    public CostResourceCreatorView(VisualElement root)
    {
        BindTreeAsset(root);
        BindUIElements();
    }

    private void BindUIElements()
    {
        ResourceSO = Parent.Q<ObjectField>("ResourceSO");
        ResourceSO.objectType = typeof(ResourceSO);
        ResourceSO.allowSceneObjects = false;

        CostAmountSlider = Parent.Q<Slider>("CostAmount");
        BaseCostAmountSlider = Parent.Q<Slider>("BaseCostAmount");
        CostMultiplierSlider = Parent.Q<Slider>("CostMultiplier");
        CreateCostResourceButton = Parent.Q<Button>("Create");

        MainPanel = Parent.Q<VisualElement>("CostResourceCard");
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