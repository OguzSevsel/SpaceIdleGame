using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;


public class ColonyCreatorView
{
    private string _treePath = "Assets/Editor/2-VisualTree/1-ColonyCreatorWindow.uxml";
    private string _stylePath = "Assets/Editor/1-Style/ColonyCreatorWindow.uss";

    public VisualTreeAsset TreeAsset { get; private set; }
    public StyleSheet StyleSheet { get; private set; }
    public VisualElement Parent;
    public VisualElement Panel;
    public TreeView ColonyTreeView;
    public Button AddColonySOButton;
    public Button SelectColonySOButton;
    public Button AddComponentsButton;
    public Button AddColonyButton;
    public ObjectField SelectColonySOField;
    public Tab CreateColonyTab;
    public Tab CreateCollectorTab;
    public TabView TabView;

    private Dictionary<VisualElement, List<VisualElement>> parentViews;

    private TreeItem _rootTreeItem;
    private int _parentId = 0;
    private List<TreeViewItemData<TreeItem>> _rootItems = new List<TreeViewItemData<TreeItem>>();

    public ColonyCreatorView(VisualElement root)
    {
        this.Parent = root;
        TreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(_treePath);
        StyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(_stylePath);
        VisualElement clonedTree = TreeAsset.CloneTree();
        clonedTree.styleSheets.Add(StyleSheet);
        Parent.Add(clonedTree);

        Panel = Parent.Q<VisualElement>("Panel");
        SelectColonySOField = Parent.Q<ObjectField>("selectSOField");
        SelectColonySOField.objectType = typeof(ColonySO);
        SelectColonySOField.allowSceneObjects = false;

        ColonyTreeView = Parent.Q<TreeView>("Hierarchy");
        AddColonySOButton = Parent.Q<Button>("addColonySOButton");
        SelectColonySOButton = Parent.Q<Button>("selectColonySOButton");
        AddComponentsButton = Parent.Q<Button>("addComponentsButton");
        AddColonyButton = Parent.Q<Button>("addColonyButton");
        CreateColonyTab = Parent.Q<Tab>("ColonyCreatorFromZero");
        CreateCollectorTab = Parent.Q<Tab>("CollectorCreator");
        TabView = Parent.Q<TabView>("TabView");
    }

    public void ClearTab(Tab tab)
    {
        tab.Clear();
    }

    public void RemoveFromPanel(VisualElement child)
    {
        if (child.parent == Panel)
        {
            Panel.Remove(child);
        }
    }

    public void AddToPanel(VisualElement child)
    {
        Panel.Add(child);
    }

    private void RegisterTreeItems()
    {
        _rootTreeItem = new TreeItem(_parentId, label: "Colony");

        TreeViewItemData<TreeItem> rootItem = new TreeViewItemData<TreeItem>(_parentId, _rootTreeItem);
        _rootItems.Add(rootItem);

        ColonyTreeView.SetRootItems(_rootItems);
    }

    private void AddChildToItem(int parentId, TreeItem childName)
    {
        int newId = GetUniqueId();
        var childItem = new TreeViewItemData<TreeItem>(newId, childName);

        ColonyTreeView.AddItem(childItem, parentId, -1, true);
    }

    private int GetUniqueId()
    {
        int maxId = 0;
        foreach (var item in _rootItems)
            if (item.id > maxId) maxId = item.id;
        return maxId + 1;
    }

    private void ClearTreeItems()
    {
        _rootItems.Clear();
        TreeViewItemData<TreeItem> rootItem = new TreeViewItemData<TreeItem>(_parentId, _rootTreeItem);
        _rootItems.Add(rootItem);

        ColonyTreeView.SetRootItems(_rootItems);
    }
}

[Serializable]
public class TreeItem
{
    public UnityEngine.Object reference; // ScriptableObject, GameObject, anything
    public int id;
    public int parentId;
    public string label;

    public TreeItem(int id, string label = null, UnityEngine.Object reference = null, int parentId = -1)
    {
        this.id = id;
        this.label = label;
        this.reference = reference;
        this.parentId = parentId;
    }

    public override string ToString()
    {
        return label;
    }
}