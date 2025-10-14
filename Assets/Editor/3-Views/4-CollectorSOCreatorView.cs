using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class CollectorSOCreatorView
{
    private string _treePath = "Assets/Editor/2-VisualTree/5-CollectorSOCard.uxml";
    private string _stylePath = "Assets/Editor/1-Style/CollectorSOCard.uss";
    public VisualTreeAsset TreeAsset { get; private set; }
    public StyleSheet StyleSheet { get; private set; }
    public VisualElement Parent { get; private set; }

    public ObjectField GeneratedResourceSOField { get; private set; }
    public EnumField CollectorTypeEnumField { get; private set; }
    public Slider BaseCollectionRateSlider { get; private set; }
    public VisualElement ButtonPanel { get; private set; }
    public Button CreateCollectorSOButton { get; private set; }
    public Button SelectCollectorSOButton { get; private set; }
    public VisualElement MainPanel { get; private set; }


    public CollectorSOCreatorView(VisualElement root)
    {
        BindTreeAsset(root);
        BindUIElements();
    }

    private void BindUIElements()
    {
        GeneratedResourceSOField = Parent.Q<ObjectField>("GeneratedResourceSO");
        GeneratedResourceSOField.objectType = typeof(ResourceSO);
        GeneratedResourceSOField.allowSceneObjects = false;

        CollectorTypeEnumField = Parent.Q<EnumField>("CollectorType");
        BaseCollectionRateSlider = Parent.Q<Slider>("BaseCollectionRate");
        ButtonPanel = Parent.Q<VisualElement>("CreateOrSelectPanel");
        CreateCollectorSOButton = Parent.Q<Button>("Create");
        SelectCollectorSOButton = Parent.Q<Button>("Select");
        MainPanel = Parent.Q<VisualElement>("CollectorSOCard");
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
