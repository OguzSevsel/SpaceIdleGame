using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


public class ColonyCreatorView
{
    private string _treePath = "Assets/Editor/2-VisualTree/1-ColonyCreatorWindow.uxml";
    private string _stylePath = "Assets/Editor/1-Style/ColonyCreatorWindow.uss";
    private TreeItem<UnityEngine.Object> _rootTreeItem;
    private int _parentId = 0;
    private List<TreeViewItemData<TreeItem<UnityEngine.Object>>> _rootItems = new List<TreeViewItemData<TreeItem<UnityEngine.Object>>>();
    public VisualTreeAsset TreeAsset { get; private set; }
    public StyleSheet StyleSheet { get; private set; }
    public VisualElement Parent;
    public Button TreeViewAddRootButton;
    public Button TreeViewAddChildButton;
    private int nextId = 1;

    private TreeViewItemData<TreeItem<UnityEngine.Object>> _draggedTreeItem;

    public UnityEngine.UIElements.TreeView TreeView;


    //Header Panel Elements
    public VisualElement MenuButtonPanel;
    public Button ColonySOMenuBtn;
    public Button ResourceSOMenuBtn;
    public Button ResourceMenuBtn;
    public Button CollectorSOMenuBtn;
    public Button CollectorMenuBtn;
    public Button ColonyMenuBtn;

    //Body Panel Elements
    public VisualElement LeftPanel;
    public VisualElement RightPanel;

    //ColonySO Card Elements
    public VisualElement ColonySoCard;
    public VisualElement ColonySOButtonPanel;
    public ObjectField ColonySOSelectObject;
    public Button ColonySOCloseBtn;
    public EnumField ColonySOTypeEnum;
    public Button ColonySOSelectBtn;
    public Button ColonySOCreateBtn;

    //ResourceSO Card Elements
    public VisualElement ResourceSOCard;
    public ObjectField ResourceSOIcon;
    public EnumField ResourceSOTypeEnum;
    public TextField ResourceSOUnitText;
    public Button ResourceSOCloseBtn;
    public Button ResourceSOCreateBtn;

    //Resource Card Elements
    public VisualElement ResourceCard;
    public ObjectField ResourceObject;
    public Slider ResourceAmountSlider;
    public Slider ResourceSellRateSlider;
    public Slider ResourceSellRateMultSlider;
    public Button ResourceCloseBtn;
    public Button ResourceCreateBtn;

    //CollectorSO Card Elements
    public VisualElement CollectorSOCard;
    public ObjectField CollectorSOResourceObject;
    public EnumField CollectorSOTypeEnum;
    public Slider CollectorSOBaseRate;
    public Button CollectorSOCloseBtn;
    public Button CollectorSOCreateBtn;

    //Collector Card Elements
    public VisualElement CollectorCard;
    public ScrollView CollectorScrollView;
    public Button CollectorCloseBtn;
    public ObjectField CollectorSOObject;
    public ListView CollectorCostResourceListView;
    public Slider CollectorRateSlider;
    public Slider CollectorRateMultSlider;
    public Slider CollectorSpeedSlider;
    public Slider CollectorSpeedMultSlider;
    public SliderInt CollectorLevelSlider;
    public SliderInt CollectorLevelIncrementSlider;
    public Button CollectorCreateBtn;

    //Cost Resource Card Elements
    public VisualElement CostResourceCard;
    public ObjectField CostResourceObject;
    public Slider CostResourceCostAmountSlider;
    public Slider CostResourceBaseCostAmountSlider;
    public Slider CostResourceCostMultSlider;
    public Button CostResourceCreateBtn;

    //Footer Panel Elements
    public VisualElement FooterPanel;
    public Label LeftLabel;
    public Label MiddleLabel;
    public Label RightLabel;


    public ColonyCreatorView(VisualElement root)
    {
        BindTreeAsset(root);
        BindUIElements();
    }

    private void BindUIElements()
    {
        //Menu Panel Elements
        MenuButtonPanel = Parent.Q<VisualElement>("ButtonPanel");
        ColonySOMenuBtn = Parent.Q<Button>("CreateColonySO");
        ResourceSOMenuBtn = Parent.Q<Button>("CreateResourceSO");
        ResourceMenuBtn = Parent.Q<Button>("CreateResource");
        CollectorSOMenuBtn = Parent.Q<Button>("CreateCollectorSO");
        CollectorMenuBtn = Parent.Q<Button>("CreateCollector");

        //Main Panels
        LeftPanel = Parent.Q<VisualElement>("LeftPanel");
        RightPanel = Parent.Q<VisualElement>("RightPanel");

        //ColonySO Card Elements
        ColonySoCard = Parent.Q<VisualElement>("ColonySOCard");
        ColonySOButtonPanel = Parent.Q<VisualElement>("ColonySOButtonPanel");
        ColonySOTypeEnum = Parent.Q<EnumField>("ColonySOTypeEnum");
        ColonySOSelectObject = Parent.Q<ObjectField>("ColonySOSelect");
        ColonySOSelectObject.objectType = typeof(ColonySO);
        ColonySOSelectObject.allowSceneObjects = false;
        ColonySOCloseBtn = Parent.Q<Button>("ColonySOCloseButton");
        ColonySOSelectBtn = Parent.Q<Button>("ColonySOSelectButton");
        ColonySOCreateBtn = Parent.Q<Button>("ColonySOCreateButton");

        //ResourceSO Card Elements
        ResourceSOCard = Parent.Q<VisualElement>("ResourceSOCard");
        ResourceSOIcon = Parent.Q<ObjectField>("ResourceSOIcon");
        ResourceSOIcon.objectType = typeof(Sprite);
        ResourceSOIcon.allowSceneObjects = false;
        ResourceSOTypeEnum = Parent.Q<EnumField>("ResourceSOType");
        ResourceSOUnitText = Parent.Q<TextField>("ResourceSOUnit");
        ResourceSOCreateBtn = Parent.Q<Button>("ResourceSOCreateButton");
        ResourceSOCloseBtn = Parent.Q<Button>("ResourceSOCloseButton");

        //Resource Card Elements
        ResourceCard = Parent.Q<VisualElement>("ResourceCard");
        ResourceObject = Parent.Q<ObjectField>("ResourceSO");
        ResourceObject.objectType = typeof(ResourceSO);
        ResourceObject.allowSceneObjects = false;
        ResourceCloseBtn = Parent.Q<Button>("ResourceCloseButton");
        ResourceAmountSlider = Parent.Q<Slider>("ResourceAmount");
        ResourceSellRateSlider = Parent.Q<Slider>("SellRate");
        ResourceSellRateMultSlider = Parent.Q<Slider>("SellRateMultiplier");
        ResourceCreateBtn = Parent.Q<Button>("ResourceCreateButton");

        //Collector SO Card Elements
        CollectorSOCard = Parent.Q<VisualElement>("CollectorSOCard");
        CollectorSOCloseBtn = Parent.Q<Button>("CollectorSOCloseButton");
        CollectorSOResourceObject = Parent.Q<ObjectField>("GeneratedResourceSO");
        CollectorSOResourceObject.objectType = typeof(ResourceSO);
        CollectorSOResourceObject.allowSceneObjects = false;
        CollectorSOTypeEnum = Parent.Q<EnumField>("CollectorType");
        CollectorSOBaseRate = Parent.Q<Slider>("BaseCollectionRate");
        CollectorSOCreateBtn = Parent.Q<Button>("CollectorSOCreateButton");

        //Collector Card Elements
        CollectorCard = Parent.Q<VisualElement>("CollectorCard");
        CollectorScrollView = Parent.Q<ScrollView>("CollectorCardScrollView");
        CollectorCloseBtn = Parent.Q<Button>("CollectorCloseButton");
        CollectorCostResourceListView = Parent.Q<ListView>("CostResourceListView");
        CollectorSOObject = Parent.Q<ObjectField>("CollectorSO");
        CollectorSOObject.objectType = typeof(CollectorSO);
        CollectorSOObject.allowSceneObjects = false;
        CollectorCreateBtn = Parent.Q<Button>("CollectorCreateButton");
        CollectorRateSlider = Parent.Q<Slider>("CollectorRate");
        CollectorRateMultSlider = Parent.Q<Slider>("CollectorRateMultiplier");
        CollectorSpeedSlider = Parent.Q<Slider>("CollectorSpeed");
        CollectorSpeedMultSlider = Parent.Q<Slider>("CollectorSpeedMultiplier");
        CollectorLevelSlider = Parent.Q<SliderInt>("CollectorLevel");
        CollectorLevelIncrementSlider = Parent.Q<SliderInt>("CollectorLevelIncrement");

        //Cost Resource Card Elements
        CostResourceCard = Parent.Q<VisualElement>("CostResourceCard");
        CostResourceObject = Parent.Q<ObjectField>("CostResourceSO");
        CostResourceObject.objectType = typeof(ResourceSO);
        CostResourceObject.allowSceneObjects = false;
        CostResourceCostAmountSlider = Parent.Q<Slider>("CostAmount");
        CostResourceBaseCostAmountSlider = Parent.Q<Slider>("BaseCostAmount");
        CostResourceCostMultSlider = Parent.Q<Slider>("CostMultiplier");
        CostResourceCreateBtn = Parent.Q<Button>("CostResourceCreateButton");

        //Footer Panel Elements
        LeftLabel = Parent.Q<Label>("LeftLabel");
        MiddleLabel = Parent.Q<Label>("MiddleLabel");
        RightLabel = Parent.Q<Label>("RightLabel");

        //TreeView Elements
        TreeView = Parent.Q<UnityEngine.UIElements.TreeView>("Hierarchy");
        TreeViewAddRootButton = Parent.Q<Button>("AddRoot");
        TreeViewAddChildButton = Parent.Q<Button>("AddChild");
        TreeView.showAlternatingRowBackgrounds = AlternatingRowBackground.None;
        SetupTreeView(TreeView);

        TreeView.selectionChanged += selectedItems =>
        {
            TreeView.Rebuild(); // forces bindItem for all visible elements
        };

        TreeView.setupDragAndDrop += SetupDragAndDropHandler;
        TreeView.canStartDrag += StartDragHandler;
        TreeView.handleDrop += HandleDrop;
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


    private DragVisualMode HandleDrop(HandleDragAndDropArgs args)
    {
        // Retrieve dragged item
        TreeViewItemData<TreeItem<UnityEngine.Object>> draggedItem = new TreeViewItemData<TreeItem<UnityEngine.Object>>();

        draggedItem = (TreeViewItemData<TreeItem<UnityEngine.Object>>)args.dragAndDropData.GetGenericData("DraggedItem");

        TreeViewItemData<TreeItem<UnityEngine.Object>> parent = new TreeViewItemData<TreeItem<UnityEngine.Object>>();

        TreeView.ClearSelection();
        Debug.Log($"{args.insertAtIndex}");

        if (args.dropPosition == DragAndDropPosition.BetweenItems) //sibling
        {
            Debug.Log("SIBLING");

            if (HasParent(draggedItem))
            {
                parent = GetParent(draggedItem);
                RemoveFromParent(parent, draggedItem);
                _rootItems.Insert(args.insertAtIndex, draggedItem);
            }
            else
            {
                _rootItems.Remove(draggedItem);
                _rootItems.Insert(args.insertAtIndex, draggedItem);
            }
        }
        else if (args.dropPosition == DragAndDropPosition.OverItem) //child
        {
            Debug.Log("CHILD");

            if (HasParent(draggedItem))
            {
                parent = GetParent(draggedItem);
                RemoveFromParent(parent, draggedItem);
                int parentIndex = _rootItems.IndexOf(parent);
                TreeViewItemData<TreeItem<UnityEngine.Object>>? newParent = FindTreeItem(args.parentId);

                if (newParent != null)
                {
                    AddToParent((TreeViewItemData<TreeItem<UnityEngine.Object>>)newParent, draggedItem, parentIndex);
                }
            }
            else
            {
                _rootItems.Remove(draggedItem);
                TreeViewItemData<TreeItem<UnityEngine.Object>>? newParent = (TreeViewItemData<TreeItem<UnityEngine.Object>>)FindTreeItem(args.parentId);
                AddToParent((TreeViewItemData<TreeItem<UnityEngine.Object>>)newParent, draggedItem, _rootItems.IndexOf(newParent.Value));
            }
        }
        else if (args.dropPosition == DragAndDropPosition.OutsideItems) //first or last
        {
            Debug.Log("FIRST OR LAST");

            if (HasParent(draggedItem))
            {
                parent = GetParent(draggedItem);
                RemoveFromParent(parent, draggedItem);

                _rootItems.Insert(args.insertAtIndex - 1, draggedItem);
                _rootItems.Insert(args.insertAtIndex - 1, draggedItem);
            }
            else
            {
                _rootItems.Remove(draggedItem);
                _rootItems.Insert(args.insertAtIndex - 1, draggedItem);
            }
        }

        TreeView.SetRootItems(_rootItems);
        TreeView.Rebuild();
        return DragVisualMode.Move;
    }



    private bool StartDragHandler(CanStartDragArgs args)
    {
        return args.selectedIds.Count() == 1;
    }
    private StartDragArgs SetupDragAndDropHandler(SetupDragAndDropArgs args)
    {
        var draggedItem = FindTreeItem(args.selectedIds.First());

        // Store dragged item in generic data
        var startDragArgs = new StartDragArgs(args.startDragArgs.title, DragVisualMode.Move);
        startDragArgs.SetGenericData("DraggedItem", draggedItem);

        return startDragArgs;
    }

    private void AddToParent(TreeViewItemData<TreeItem<UnityEngine.Object>> parent, TreeViewItemData<TreeItem<UnityEngine.Object>> child, int parentIndex)
    {
        List<TreeViewItemData<TreeItem<UnityEngine.Object>>> newChildren = new List<TreeViewItemData<TreeItem<UnityEngine.Object>>>();

        if (parent.children != null)
        {
            newChildren = new List<TreeViewItemData<TreeItem<UnityEngine.Object>>>(parent.children);
        }

        newChildren.Add(child);

        TreeViewItemData<TreeItem<UnityEngine.Object>> newParent = new TreeViewItemData<TreeItem<UnityEngine.Object>>(id: parent.id, data: parent.data, children: newChildren);

        _rootItems.Remove(parent);
        _rootItems.Insert(parentIndex, newParent);
    }

    private void RemoveFromParent(TreeViewItemData<TreeItem<UnityEngine.Object>> parent, TreeViewItemData<TreeItem<UnityEngine.Object>> child)
    {
        List<TreeViewItemData<TreeItem<UnityEngine.Object>>> newChildren = new List<TreeViewItemData<TreeItem<UnityEngine.Object>>>(parent.children);

        newChildren.Remove(child);

        TreeViewItemData < TreeItem < UnityEngine.Object >> newParent = new TreeViewItemData<TreeItem<UnityEngine.Object>>(id: parent.id, data: parent.data, children: newChildren);

        _rootItems.Remove(parent);
        _rootItems.Add(newParent);
    }

    private void AddAsRoot(TreeViewItemData<TreeItem<UnityEngine.Object>> item)
    {
        _rootItems.Add(item);
    }
    
    
    private TreeViewItemData<TreeItem<UnityEngine.Object>> GetParent(TreeViewItemData<TreeItem<UnityEngine.Object>> item)
    {
        TreeViewItemData<TreeItem<UnityEngine.Object>> newParent = new TreeViewItemData<TreeItem<UnityEngine.Object>>();

        foreach (var parent in _rootItems)
        {
            if (!parent.Equals(item))
            {
                if (parent.children.Contains(item))
                {
                    newParent = parent;
                }
            }
        }
        return newParent;
    }

    private bool HasParent(TreeViewItemData<TreeItem<UnityEngine.Object>> item)
    {
        foreach (var parent in _rootItems)
        {
            if (!parent.Equals(item))
            {
                if (parent.children.Contains(item))
                {
                    return true;
                }
            }
        }
        return false;
    }

    

    

    



    public TreeViewItemData<TreeItem<UnityEngine.Object>>? FindTreeItem(int id)
    {
        var item = new TreeViewItemData<TreeItem<UnityEngine.Object>>();

        foreach (var traversedItem in _rootItems)
        {
            if (traversedItem.id == id)
            {
                item = traversedItem;
            }
            else
            {
                foreach (var child in traversedItem.children)
                {
                    if (child.id == id)
                    {
                        item = child;
                    }
                }
            }
        }
        return item;
    }
    public TreeViewItemData<TreeItem<UnityEngine.Object>>? FindTreeItem(UnityEngine.Object treeItem)
    {
        foreach (TreeViewItemData<TreeItem<UnityEngine.Object>> node in _rootItems)
        {
            if (treeItem == node.data.reference)
            {
                return node;
            }
            else
            {
                if (node.hasChildren)
                {
                    foreach (var children in node.children)
                    {
                        if (treeItem == node.data.reference)
                        {
                            return node;
                        }
                    }
                }
            }
        }
        return null;
    }






    public void AddRootItemToTree(UnityEngine.Object item)
    {
        TreeItem<UnityEngine.Object> newTreeItem =
            new TreeItem<UnityEngine.Object>
            (
                nextId++,
                label: item.ToString(),
                reference: item,
                children: new List<UnityEngine.Object>()
            );

        TreeViewItemData<TreeItem<UnityEngine.Object>> newItem = new TreeViewItemData<TreeItem<UnityEngine.Object>>(nextId++, data:newTreeItem);

        _rootItems.Add(newItem);
        TreeView.SetRootItems(_rootItems);
        TreeView.RefreshItems();
    }

    public void AddChildToItemWithObjs(UnityEngine.Object child, UnityEngine.Object parent)
    {
        // 1. Find the parent TreeViewItemData
        var parentNode = FindTreeItem(parent);
        if (parentNode == null) return;

        // 2. Create the child data
        TreeItem<UnityEngine.Object> childData = new TreeItem<UnityEngine.Object>(
            nextId++,
            label: child.ToString(),
            reference: child,
            parentId: parentNode.Value.data.id
        );

        // 3. Create TreeViewItemData for the child
        var childNode = new TreeViewItemData<TreeItem<UnityEngine.Object>>(childData.id, childData);

        // 4. Rebuild the parent's children list
        List<TreeViewItemData<TreeItem<UnityEngine.Object>>> newChildren;
        if (parentNode.Value.children != null)
            newChildren = new List<TreeViewItemData<TreeItem<UnityEngine.Object>>>(parentNode.Value.children);
        else
            newChildren = new List<TreeViewItemData<TreeItem<UnityEngine.Object>>>();

        newChildren.Add(childNode);

        // 5. Create a new parent node with updated children
        var newParentNode = new TreeViewItemData<TreeItem<UnityEngine.Object>>(
            parentNode.Value.id,
            parentNode.Value.data,
            newChildren
        );

        // 6. Replace the old parent in root items
        ReplaceNodeInList(_rootItems, newParentNode);

        // 7. Update TreeView
        TreeView.SetRootItems(_rootItems);
        TreeView.Rebuild();
    }

    private bool ReplaceNodeInList(List<TreeViewItemData<TreeItem<UnityEngine.Object>>> nodes, TreeViewItemData<TreeItem<UnityEngine.Object>> newNode)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].id == newNode.id)
            {
                nodes[i] = newNode;
                return true;
            }

            if (nodes[i].children != null)
            {
                if (ReplaceNodeInList(new List<TreeViewItemData<TreeItem<UnityEngine.Object>>>(nodes[i].children), newNode))
                    return true;
            }
        }
        return false;
    }

    private void SetupTreeView(UnityEngine.UIElements.TreeView treeView)
    {
        treeView.makeItem = () =>
        {
            var container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;
            container.style.alignItems = Align.Center;

            var icon = new Image();
            icon.style.width = 16;
            icon.style.height = 16;
            container.Add(icon);

            var label = new Label();
            label.style.unityTextAlign = TextAnchor.MiddleLeft;
            label.name = "label";
            container.Add(label);

            return container;
        };

        treeView.bindItem = (element, index) =>
        {
            var itemData = treeView.GetItemDataForIndex<TreeItem<UnityEngine.Object>>(index);

            var label = element.Q<Label>("label");
            label.text = itemData.label;

            Color evenRowColor = ColorUtility.TryParseHtmlString("#3E1E68", out var c1) ? c1 : Color.black;
            Color oddRowColor = ColorUtility.TryParseHtmlString("#5D2F77", out var c2) ? c2 : Color.black;
            Color selectedColor = ColorUtility.TryParseHtmlString("#E45A92", out var sc) ? sc : Color.pink;

            if (TreeView.selectedItem == itemData)
                element.style.backgroundColor = selectedColor;       // selected row
            else
                element.style.backgroundColor = (index % 2 == 0) ? evenRowColor : oddRowColor; // alternating

            var icon = element.Q<Image>();
            icon.image = AssetPreview.GetMiniThumbnail(itemData.reference);
        };
    }

}

[Serializable]
public class TreeItem<T> where T : UnityEngine.Object
{
    public T reference;      
    public int id;
    public int parentId;
    public string label;
    public List<T> children;

    public TreeItem(int id,  string label = null, T reference = null, int parentId = -1, List<T> children = null)
    {
        this.id = id;
        this.label = label;
        this.reference = reference;
        this.parentId = parentId;
        this.children = children;
    }

    public override string ToString()
    {
        return label ?? reference?.name ?? $"Item {id}";
    }
}