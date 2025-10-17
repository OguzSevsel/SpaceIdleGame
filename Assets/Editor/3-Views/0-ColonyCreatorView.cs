using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


public class ColonyCreatorView
{
    private string _treePath = "Assets/Editor/2-VisualTree/1-ColonyCreatorWindow.uxml";
    private string _stylePath = "Assets/Editor/1-Style/ColonyCreatorWindow.uss";
    private List<TreeViewItemData<TreeItem>> _rootItems = new List<TreeViewItemData<TreeItem>>();
    public VisualTreeAsset TreeAsset { get; private set; }
    public StyleSheet StyleSheet { get; private set; }
    public VisualElement Parent;
    public Button GenerateUIButton;
    public Button TreeViewAddChildButton;
    private int nextId = 1;

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
    public ObjectField ColonySOPrefab;
    public ObjectField ColonySOManager;
    public ObjectField ColonySOWhereToAdd;
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
    public ObjectField CollectorPrefab;
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
        ColonySOPrefab = Parent.Q<ObjectField>("ColonyUI");
        ColonySOPrefab.objectType = typeof(GameObject);
        ColonySOPrefab.allowSceneObjects = false;
        ColonySOManager = Parent.Q<ObjectField>("ColonyManager");
        ColonySOManager.objectType = typeof(GameObject);
        ColonySOManager.allowSceneObjects = true;
        ColonySOWhereToAdd = Parent.Q<ObjectField>("ColonyWhereToAdd");
        ColonySOWhereToAdd.objectType = typeof(GameObject);
        ColonySOWhereToAdd.allowSceneObjects = true;
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
        CollectorPrefab = Parent.Q<ObjectField>("CollectorUI");
        CollectorPrefab.objectType = typeof(GameObject);
        CollectorPrefab.allowSceneObjects = false;
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
        GenerateUIButton = Parent.Q<Button>("GenerateUI");
        TreeViewAddChildButton = Parent.Q<Button>("AddChild");
        TreeView.showAlternatingRowBackgrounds = AlternatingRowBackground.None;
        SetupTreeView(TreeView);

        TreeView.selectionChanged += selectedItems =>
        {
            TreeView.SetRootItems(_rootItems);
            TreeView.Rebuild(); // forces bindItem for all visible elements
        };

        TreeView.itemExpandedChanged += (args) =>
        {
            TreeView.SetSelectionById(args.id);
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
            var itemData = treeView.GetItemDataForIndex<TreeItem>(index);

            var label = element.Q<Label>("label");
            label.text = itemData.label;

            Color evenRowColor = ColorUtility.TryParseHtmlString("#3E1E68", out var c1) ? c1 : Color.black;
            Color oddRowColor = ColorUtility.TryParseHtmlString("#5D2F77", out var c2) ? c2 : Color.black;
            Color selectedColor = ColorUtility.TryParseHtmlString("#E45A92", out var sc) ? sc : Color.pink;

            if (TreeView.selectedItem == itemData)
                element.style.backgroundColor = selectedColor;       // selected row
            else
                element.style.backgroundColor = (index % 2 == 0) ? evenRowColor : oddRowColor; // alternating
        };
    }



    #region TreeView Drag Drop Functionality

    private DragVisualMode HandleDrop(HandleDragAndDropArgs args)
    {
        // Retrieve dragged item
        TreeViewItemData<TreeItem> draggedItem = new TreeViewItemData<TreeItem>();

        draggedItem = (TreeViewItemData<TreeItem>)args.dragAndDropData.GetGenericData("DraggedItem");

        TreeViewItemData<TreeItem>? parent = new TreeViewItemData<TreeItem>();

        TreeView.ClearSelection();
        Debug.Log($"insert {args.insertAtIndex}");
        Debug.Log($"childIndex {args.childIndex}");

        if (args.dropPosition == DragAndDropPosition.BetweenItems) //sibling
        {
            Debug.Log("SIBLING");

            if (HasParent(draggedItem))
            {
                parent = GetParent(draggedItem);

                if (parent != null)
                {
                    if (parent.Value.id == args.parentId)
                    {
                        int parentIndex = _rootItems.IndexOf(parent.Value);

                        RemoveFromParent(parent.Value, draggedItem, parentIndex);
                        ReplaceParent(parent.Value, draggedItem, parentIndex, args.childIndex);
                    }
                    else
                    {
                        RemoveFromParent(parent.Value, draggedItem, _rootItems.IndexOf(parent.Value));
                        int rootInsertIndex = GetRootInsertIndex(args);
                        if (rootInsertIndex == -1)
                        {
                            if (args.insertAtIndex < _rootItems.Count)
                            {
                                _rootItems.Insert(args.insertAtIndex, draggedItem);
                            }
                            else
                            {
                                _rootItems.Insert(0, draggedItem);
                            }
                        }
                        else
                        {
                            _rootItems.Insert(rootInsertIndex, draggedItem);
                        }
                    }
                }
            }
            else
            {
                int rootInsertIndex = GetRootInsertIndex(args);
                if (rootInsertIndex == -1)
                {
                    TreeViewItemData<TreeItem>? newParent = FindTreeItem(args.parentId);
                    int parentIndex = _rootItems.IndexOf(newParent.Value);

                    if (draggedItem.data.reference == newParent.Value.data.reference)
                    {
                        EditorUtility.DisplayDialog("Conflict", "You cant do that", "OK");
                    }
                    else
                    {
                        object draggedItemType = draggedItem.data.reference.GetType();
                        object parentType = newParent.Value.data.reference.GetType();

                        if (draggedItemType is CollectorSO && parentType is CollectorSO)
                        {
                            EditorUtility.DisplayDialog("Conflict", "You cant do that", "OK");
                        }
                        else if (draggedItemType is CollectorModel && parentType is CollectorModel)
                        {
                            EditorUtility.DisplayDialog("Conflict", "You cant do that", "OK");
                        }
                        else if (draggedItemType is Resource && parentType is CollectorModel)
                        {
                            EditorUtility.DisplayDialog("Conflict", "You cant do that", "OK");
                        }
                        else if (draggedItemType is Resource && parentType is CollectorModel)
                        {
                            EditorUtility.DisplayDialog("Conflict", "You cant do that", "OK");
                        }
                        else
                        {
                            if (newParent != null)
                            {
                                ReplaceParent((TreeViewItemData<TreeItem>)newParent, draggedItem, parentIndex, args.childIndex);
                            }
                            _rootItems.Remove(draggedItem);
                        }
                    }
                }
                else
                {
                    _rootItems.Remove(draggedItem);
                    _rootItems.Insert(rootInsertIndex, draggedItem);
                }
            }
        }
        else if (args.dropPosition == DragAndDropPosition.OverItem) //child
        {
            Debug.Log("CHILD");

            if (HasParent(draggedItem))
            {
                parent = GetParent(draggedItem);

                if (parent != null)
                {
                    int parentIndex = _rootItems.IndexOf(parent.Value);
                    TreeViewItemData<TreeItem>? newParent = FindTreeItem(args.parentId);

                    TreeViewItemData<TreeItem>? newSubParent = GetParent((TreeViewItemData<TreeItem>)newParent);

                    if (newSubParent != null)
                    {
                        EditorUtility.DisplayDialog("Invalid Request", "You cant do that", "OK");
                    }
                    else
                    {
                        if (newParent != null)
                        {
                            RemoveFromParent(parent.Value, draggedItem, _rootItems.IndexOf(parent.Value));

                            AddToParent((TreeViewItemData<TreeItem>)newParent, draggedItem, parentIndex, args.childIndex);
                        }
                    }
                }
            }
            else
            {
                TreeViewItemData<TreeItem>? newParent = (TreeViewItemData<TreeItem>)FindTreeItem(args.parentId);

                TreeViewItemData<TreeItem>? newSubParent = GetParent((TreeViewItemData<TreeItem>)newParent);


                if (draggedItem.data.reference == newParent.Value.data.reference)
                {
                    EditorUtility.DisplayDialog("Conflict", "You cant do that", "OK");
                }
                else
                {
                    object draggedItemType = draggedItem.data.reference.GetType();
                    object parentType = newParent.Value.data.reference.GetType();

                    if (draggedItemType is ColonySO && parentType is ColonySO)
                    {
                        EditorUtility.DisplayDialog("Conflict", "You cant do that", "OK");
                    }
                    else if (draggedItemType is CollectorModel && parentType is CollectorModel)
                    {
                        EditorUtility.DisplayDialog("Conflict", "You cant do that", "OK");
                    }
                    else if (draggedItemType is Resource && parentType is CollectorModel)
                    {
                        EditorUtility.DisplayDialog("Conflict", "You cant do that", "OK");
                    }
                    else
                    {
                        if (newSubParent != null)
                        {
                            EditorUtility.DisplayDialog("Invalid Request", "You cant do that", "OK");
                        }
                        else
                        {
                            AddToParent((TreeViewItemData<TreeItem>)newParent, draggedItem, _rootItems.IndexOf(newParent.Value), args.childIndex);
                            _rootItems.Remove(draggedItem);
                        }
                    }
                }
            }
        }
        else if (args.dropPosition == DragAndDropPosition.OutsideItems) //first or last
        {
            Debug.Log("FIRST OR LAST");

            if (HasParent(draggedItem))
            {
                parent = GetParent(draggedItem);

                if (parent != null)
                {
                    RemoveFromParent(parent.Value, draggedItem, _rootItems.IndexOf(parent.Value));

                    if (args.insertAtIndex != 0)
                    {
                        _rootItems.Add(draggedItem);
                    }
                    else
                    {
                        _rootItems.Insert(0, draggedItem);
                    }
                }
            }
            else
            {
                _rootItems.Remove(draggedItem);

                if (args.insertAtIndex != 0)
                {
                    _rootItems.Add(draggedItem);
                }
                else
                {
                    _rootItems.Insert(0, draggedItem);
                }
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


    #region Utils

    private void ReplaceParent(TreeViewItemData<TreeItem> parent, TreeViewItemData<TreeItem> child, int parentIndex, int childIndex)
    {
        // Copy existing children safely
        var newChildren = parent.children != null
            ? new List<TreeViewItemData<TreeItem>>(parent.children)
            : new List<TreeViewItemData<TreeItem>>();

        // Ensure unique (prevent duplicates)
        newChildren.RemoveAll(c => c.id == child.id);

        // Insert safely
        childIndex = Mathf.Clamp(childIndex, 0, newChildren.Count);
        newChildren.Insert(childIndex, child);

        // Create a new parent with updated children
        var newParent = new TreeViewItemData<TreeItem>(
            id: parent.id,
            data: parent.data,
            children: newChildren
        );

        // ✅ Replace in-place instead of remove+insert
        if (parentIndex >= 0 && parentIndex < _rootItems.Count)
            _rootItems[parentIndex] = newParent;
    }
    private void RemoveFromParent(TreeViewItemData<TreeItem> parent, TreeViewItemData<TreeItem> child, int parentIndex)
    {
        List<TreeViewItemData<TreeItem>> newChildren = new List<TreeViewItemData<TreeItem>>(parent.children);

        newChildren.Remove(child);

        TreeViewItemData<TreeItem> newParent = new TreeViewItemData<TreeItem>(id: parent.id, data: parent.data, children: newChildren);

        if (newParent.data.reference is GameObject parentObject)
        {
            ColonyModel colonyModel = parentObject.GetComponent<ColonyModel>();

            if (colonyModel != null)
            {
                if (child.data.reference is GameObject childObject)
                {
                    CollectorModel collectorModel = childObject.GetComponent<CollectorModel>();
                    colonyModel.Collectors.Remove(collectorModel);
                }
                else if (child.data.reference is Resource childResource)
                {
                    colonyModel.Resources.Remove(childResource);
                }
            }
        }

        _rootItems.Remove(parent);
        _rootItems.Insert(parentIndex, newParent);
    }
    private bool HasParent(TreeViewItemData<TreeItem> item)
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

    private int GetRootInsertIndex(HandleDragAndDropArgs args)
    {
        // Get the item being dropped *over* (if any)
        var dropItemId = args.parentId;

        // If dropping outside items, we can use args.insertAtIndex safely as 0 or end
        if (args.dropPosition == DragAndDropPosition.OutsideItems)
            return Mathf.Clamp(args.insertAtIndex, 0, _rootItems.Count);

        // Otherwise, find the visible items up to the drop index
        var visibleItems = _rootItems;

        // Rebuild a mapping of visible row -> root index
        var visibleRootIndexes = new Dictionary<int, int>();
        int visibleIndex = 0;
        int rootIndex = 0;

        foreach (var root in _rootItems)
        {
            visibleRootIndexes[visibleIndex++] = rootIndex; // root row
            if (root.children != null)
                visibleIndex += CountVisibleChildren(root); // skip children
            rootIndex++;
        }

        // Now find the nearest valid root index based on insertAtIndex
        if (visibleRootIndexes.ContainsKey(args.insertAtIndex))
            return visibleRootIndexes[args.insertAtIndex];
        else
        {
            return -1;
        }
    }
    private int CountVisibleChildren(TreeViewItemData<TreeItem> item)
    {
        if (item.children == null) return 0;
        int count = item.children.Count();
        foreach (var child in item.children)
            count += CountVisibleChildren(child);
        return count;
    }

    #endregion




    #endregion

    #region Add Nodes to Tree View

    public void AddRootItemToTree(UnityEngine.Object item, GameObject UIReference, GameObject whereToAdd, string label, TypeEnum type)
    {
        TreeItem newTreeItem =
            new TreeItem
            (
                nextId++,
                type: type,
                label: label,
                reference: item,
                UIReference: UIReference,
                whereToAdd: whereToAdd
            );

        TreeViewItemData<TreeItem> newItem = new TreeViewItemData<TreeItem>(nextId++, data: newTreeItem);

        _rootItems.Add(newItem);
        TreeView.SetRootItems(_rootItems);
        TreeView.RefreshItems();
    }
    public void AddChildToItemWithObjs(object child, GameObject UIReference, GameObject whereToAdd, string label, TypeEnum childType, object parent)
    {
        // 1. Find the parent TreeViewItemData
        var parentNode = FindTreeItem(parent);
        if (parentNode == null) return;

        // 2. Create the child data
        TreeItem childData = new TreeItem(
            nextId++,
            type: childType,
            label: label,
            reference: child,
            UIReference: UIReference,
            whereToAdd: whereToAdd,
            parentId: parentNode.Value.data.id
        );

        // 3. Create TreeViewItemData for the child
        var childNode = new TreeViewItemData<TreeItem>(childData.id, childData);

        // 4. Rebuild the parent's children list
        List<TreeViewItemData<TreeItem>> newChildren;
        if (parentNode.Value.children != null)
            newChildren = new List<TreeViewItemData<TreeItem>>(parentNode.Value.children);
        else
            newChildren = new List<TreeViewItemData<TreeItem>>();

        newChildren.Add(childNode);

        // 5. Create a new parent node with updated children
        var newParentNode = new TreeViewItemData<TreeItem>(
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
    private bool ReplaceNodeInList(List<TreeViewItemData<TreeItem>> nodes, TreeViewItemData<TreeItem> newNode)
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
                if (ReplaceNodeInList(new List<TreeViewItemData<TreeItem>>(nodes[i].children), newNode))
                    return true;
            }
        }
        return false;
    }
    private void AddToParent(TreeViewItemData<TreeItem> parent, TreeViewItemData<TreeItem> child, int parentIndex, int childIndex)
    {
        List<TreeViewItemData<TreeItem>> newChildren = new List<TreeViewItemData<TreeItem>>();

        if (parent.children != null)
        {
            newChildren = new List<TreeViewItemData<TreeItem>>(parent.children);
        }

        if (newChildren.Count == 0)
        {
            newChildren.Add(child);
        }
        else
        {
            if (childIndex == -1)
            {
                newChildren.Add(child);
            }
            else
            {
                newChildren.Insert(childIndex, child);
            }
        }


        TreeViewItemData<TreeItem> newParent = new TreeViewItemData<TreeItem>(id: parent.id, data: parent.data, children: newChildren);

        if (newParent.data.reference is GameObject parentObject)
        {
            ColonyModel colonyModel = parentObject.GetComponent<ColonyModel>();

            if (colonyModel != null)
            {
                if (child.data.reference is GameObject childObject)
                {
                    CollectorModel collectorModel = childObject.GetComponent<CollectorModel>();
                    childObject.transform.SetParent(parentObject.transform);
                    colonyModel.Collectors.Add(collectorModel);
                }
                else if (child.data.reference is Resource childResource)
                {
                    colonyModel.Resources.Add(childResource);
                }
            }
        }

        _rootItems.Remove(parent);
        _rootItems.Insert(parentIndex, newParent);
    }

    #endregion

    #region Find Nodes

    public TreeViewItemData<TreeItem>? FindTreeItem(object treeItem)
    {
        foreach (TreeViewItemData<TreeItem> node in _rootItems)
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
    public TreeViewItemData<TreeItem>? FindTreeItem(int id)
    {
        var item = new TreeViewItemData<TreeItem>();

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
    private TreeViewItemData<TreeItem>? GetParent(TreeViewItemData<TreeItem> item)
    {
        TreeViewItemData<TreeItem> newParent = new TreeViewItemData<TreeItem>();

        foreach (var parent in _rootItems)
        {
            if (!parent.Equals(item))
            {
                if (parent.children.Contains(item))
                {
                    newParent = parent;
                }
            }
            else
            {
                return null;
            }
        }
        return newParent;
    }


    #endregion

    public void GenerateUI(ColonyManager colonyManager)
    {
        foreach (TreeViewItemData<TreeItem> node in _rootItems)
        {
            if (node.data.reference is GameObject Object)
            {
                if (Object.TryGetComponent<ColonyModel>(out ColonyModel colonyModel))
                {
                    GameObject colonyUI = GameObject.Instantiate(node.data.UIReference);
                    colonyUI.transform.SetParent(node.data.WhereToAdd.transform);
                    ColonyView colonyView = colonyUI.GetComponent<ColonyView>();
                    TabGroup colonyTabGroup = GameObject.Find("ColonyTabGroupPanel").GetComponent<TabGroup>();
                    colonyTabGroup.objectsToSwap.Add(colonyUI);

                    RectTransform rect = colonyUI.GetComponent<RectTransform>();
                    rect.anchorMin = Vector2.zero;   
                    rect.anchorMax = Vector2.one;    
                    rect.offsetMin = Vector2.zero;   
                    rect.offsetMax = Vector2.zero;
                    rect.localScale = Vector3.one;
                    rect.position = new Vector3(rect.position.x, rect.position.y, 1);

                    colonyManager.AddToColonyModels(colonyModel);
                    colonyManager.AddToColonyViews(colonyView);

                    colonyUI.name = $"{colonyModel.colonyData.ColonyType.ToString()}_Colony";
                    
                    if (node.hasChildren)
                    {
                        foreach (TreeViewItemData<TreeItem> item in node.children)
                        {
                            if (item.data.reference is GameObject collectorObject)
                            {
                                if (collectorObject.TryGetComponent<CollectorModel>(out CollectorModel collectorModel))
                                {
                                    Transform whereToAddCollector = FindChild(colonyUI.transform, "IndustryContent");
                                    GameObject collectorUI = GameObject.Instantiate(item.data.UIReference);
                                    collectorUI.transform.SetParent(whereToAddCollector.transform);

                                    RectTransform collectorRect = collectorUI.GetComponent<RectTransform>();
                                    collectorRect.anchorMin = Vector2.zero;
                                    collectorRect.anchorMax = Vector2.one;
                                    collectorRect.offsetMin = Vector2.zero;
                                    collectorRect.localScale = Vector3.one;
                                    collectorRect.position = new Vector3(collectorRect.position.x, collectorRect.position.y, 1);

                                    CollectorView collectorView = collectorUI.GetComponent<CollectorView>();
                                    colonyView.CollectorPanels.Add(collectorView);
                                    collectorUI.name = collectorModel.Data.DataSO.CollectorType.ToString();
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private Transform FindChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child;

            var result = FindChild(child, name);
            if (result != null)
                return result;
        }
        return null;
    }
}


public enum TypeEnum
{
    Colony,
    Collector,
    CostResource,
    Resource
}

[Serializable]
public class TreeItem
{
    public object reference;
    public GameObject UIReference;
    public GameObject WhereToAdd;
    public TypeEnum type;
    public int id;
    public int parentId;
    public string label;

    public TreeItem(int id, TypeEnum type, GameObject UIReference = null, GameObject whereToAdd = null, string label = null, object reference = default, int parentId = -1)
    {
        this.id = id;
        this.type = type;
        this.label = label;
        this.reference = reference;
        this.parentId = parentId;
        this.UIReference = UIReference;
        this.WhereToAdd = whereToAdd;
    }

    public override string ToString()
    {
        return label ?? reference?.ToString() ?? $"Item {id}";
    }
}