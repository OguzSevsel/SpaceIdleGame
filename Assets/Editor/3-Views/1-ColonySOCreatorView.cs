using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class ColonySOCreatorView
{
    private string _treePath = "Assets/Editor/2-VisualTree/2-ColonySOCard.uxml";
    private string _stylePath = "Assets/Editor/1-Style/ColonySOCard.uss";
    public VisualTreeAsset TreeAsset { get; private set; }
    public StyleSheet StyleSheet { get; private set; }
    public VisualElement Parent { get; private set; }

    public EnumField ColonyTypeEnumField { get; private set; }
    public Button CreateColonySOButton { get; private set; }
    public VisualElement MainPanel { get; private set; }

    public ColonySOCreatorView(VisualElement root)
    {
        BindTreeAsset(root);
        BindUIElements();
    }

    private void BindUIElements()
    {
        ColonyTypeEnumField = Parent.Q<EnumField>("ColonyType");
        CreateColonySOButton = Parent.Q<Button>("Create");
        MainPanel = Parent.Q<VisualElement>("ColonySOCard");
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
