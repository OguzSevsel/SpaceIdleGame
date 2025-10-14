using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class ResourceSOCreatorView
{
    private string _treePath = "Assets/Editor/2-VisualTree/3-ResourceSOCard.uxml";
    private string _stylePath = "Assets/Editor/1-Style/ResourceSOCard.uss";
    public VisualTreeAsset TreeAsset { get; private set; }
    public StyleSheet StyleSheet { get; private set; }
    public VisualElement Parent { get; private set; }

    public ObjectField ResourceIconObjectField { get; private set; }
    public EnumField ResourceTypeEnumField { get; private set; }
    public TextField ResourceUnitTextField { get; private set; }
    public Button CreateResourceSOButton { get; private set; }
    public Button SelectResourceSOButton { get; private set; }
    public VisualElement ButtonPanel { get; private set; }
    public VisualElement MainPanel { get; private set; }


    public ResourceSOCreatorView(VisualElement root)
    {
        BindTreeAsset(root);
        BindUIElements();
    }

    private void BindUIElements()
    {
        MainPanel = Parent.Q<VisualElement>("ResourceSOCard");
        ResourceIconObjectField = Parent.Q<ObjectField>("ResourceIcon");
        SetObjectFieldType(isSprite: true);
        ResourceTypeEnumField = Parent.Q<EnumField>("ResourceType");
        ResourceUnitTextField = Parent.Q<TextField>("ResourceUnit");
        CreateResourceSOButton = Parent.Q<Button>("Create");
        SelectResourceSOButton = Parent.Q<Button>("Select");
        ButtonPanel = Parent.Q<VisualElement>("CreateOrSelectPanel");
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

    public void SetObjectFieldType(bool isSprite = true)
    {
        if (isSprite)
        {
            ResourceIconObjectField.objectType = typeof(Sprite);
            ResourceIconObjectField.allowSceneObjects = false;
        }
        else
        {
            ResourceIconObjectField.objectType = typeof(ResourceSO);
            ResourceIconObjectField.allowSceneObjects = false;
        }
    }
}
