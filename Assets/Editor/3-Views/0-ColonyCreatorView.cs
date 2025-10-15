using System;
using System.CodeDom;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;


public class ColonyCreatorView
{
    private string _treePath = "Assets/Editor/2-VisualTree/1-ColonyCreatorWindow.uxml";
    private string _stylePath = "Assets/Editor/1-Style/ColonyCreatorWindow.uss";
    private TreeItem _rootTreeItem;
    private int _parentId = 0;
    private List<TreeViewItemData<TreeItem>> _rootItems = new List<TreeViewItemData<TreeItem>>();
    public VisualTreeAsset TreeAsset { get; private set; }
    public StyleSheet StyleSheet { get; private set; }
    public VisualElement Parent;



    public TreeView TreeView;


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
        TreeView = Parent.Q<TreeView>("Hierarchy");

        //Menu Panel Elements
        MenuButtonPanel = Parent.Q<VisualElement>("ButtonPanel");
        ColonySOMenuBtn = Parent.Q<Button>("CreateColonySO");
        ResourceSOMenuBtn = Parent.Q<Button>("CreateResourceSO");
        ResourceMenuBtn = Parent.Q<Button>("CreateResource");
        CollectorSOMenuBtn = Parent.Q<Button>("CreateCollectorSO");
        CollectorMenuBtn = Parent.Q<Button>("CreateCollector");
        ColonyMenuBtn = Parent.Q<Button>("CreateColony");

        //Main Panels
        LeftPanel = Parent.Q<VisualElement>("LeftPanel");
        RightPanel = Parent.Q<VisualElement>("RightPanel");

        //ColonySO Card Elements
        ColonySoCard = Parent.Q<VisualElement>("ColonySOCard");
        ColonySOButtonPanel = Parent.Q<VisualElement>("ColonySOButtonPanel");
        ColonySOCloseBtn = Parent.Q<Button>("ColonySOCloseButton");
        ColonySOSelectBtn = Parent.Q<Button>("ColonySOCreateButton");
        ColonySOCreateBtn = Parent.Q<Button>("ColonySOSelectButton");

        //ResourceSO Card Elements
        ResourceSOCard = Parent.Q<VisualElement>("ResourceSOCard");
        ResourceSOIcon = Parent.Q<ObjectField>("ResourceSOIcon");
        ResourceSOTypeEnum = Parent.Q<EnumField>("ResourceSOType");
        ResourceSOUnitText = Parent.Q<TextField>("ResourceSOUnit");
        ResourceSOCreateBtn = Parent.Q<Button>("ResourceSOCreateButton");
        ResourceSOCloseBtn = Parent.Q<Button>("ResourceSOCloseButton");

        //Resource Card Elements
        ResourceCard = Parent.Q<VisualElement>("ResourceCard");
        ResourceObject = Parent.Q<ObjectField>("ResourceSO");
        ResourceCloseBtn = Parent.Q<Button>("ResourceCloseButton");
        ResourceAmountSlider = Parent.Q<Slider>("ResourceAmount");
        ResourceSellRateSlider = Parent.Q<Slider>("SellRate");
        ResourceSellRateMultSlider = Parent.Q<Slider>("SellRateMultiplier");
        ResourceCreateBtn = Parent.Q<Button>("ResourceCreateButton");

        //Collector SO Card Elements
        CollectorSOCard = Parent.Q<VisualElement>("CollectorSOCard");
        CollectorSOCloseBtn = Parent.Q<Button>("CollectorSOCloseButton");
        CollectorSOObject = Parent.Q<ObjectField>("GeneratedResourceSO");
        CollectorSOTypeEnum = Parent.Q<EnumField>("CollectorType");
        CollectorSOBaseRate = Parent.Q<Slider>("BaseCollectionRate");
        CollectorSOCreateBtn = Parent.Q<Button>("CollectorSOCreateButton");

        //Collector Card Elements
        CollectorCard = Parent.Q<VisualElement>("CollectorCard");
        CollectorScrollView = Parent.Q<ScrollView>("CollectorCardScrollView");
        CollectorCloseBtn = Parent.Q<Button>("CollectorCloseButton");
        CollectorCostResourceListView = Parent.Q<ListView>("CostResourceListView");
        CollectorSOObject = Parent.Q<ObjectField>("CollectorSO");
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
        CostResourceCostAmountSlider = Parent.Q<Slider>("CostAmount");
        CostResourceBaseCostAmountSlider = Parent.Q<Slider>("BaseCostAmount");
        CostResourceCostMultSlider = Parent.Q<Slider>("CostMultiplier");
        CostResourceCreateBtn = Parent.Q<Button>("CostResourceCreateButton");

        //Footer Panel Elements
        LeftLabel = Parent.Q<Label>("LeftLabel");
        MiddleLabel = Parent.Q<Label>("MiddleLabel");
        RightLabel = Parent.Q<Label>("RightLabel");
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

    public void ClearPanels()
    {
        LeftPanel.Clear(); 
        RightPanel.Clear();
    }

    public void InsertToLeft(int index, VisualElement child)
    {
        LeftPanel.Insert(index, child);
    }

    public void RemoveFromLeft(VisualElement child)
    {
        LeftPanel.Remove(child);
    }

    public void InsertToRight(int index, VisualElement child)
    {
        RightPanel.Insert(index, child);
    }

    public void RemoveFromRight(VisualElement child)
    {
        RightPanel.Remove(child);
    }


























    private void RegisterTreeItems()
    {
        _rootTreeItem = new TreeItem(_parentId, label: "Colony");

        TreeViewItemData<TreeItem> rootItem = new TreeViewItemData<TreeItem>(_parentId, _rootTreeItem);
        _rootItems.Add(rootItem);

        TreeView.SetRootItems(_rootItems);
    }

    private void AddChildToItem(int parentId, TreeItem childName)
    {
        int newId = GetUniqueId();
        var childItem = new TreeViewItemData<TreeItem>(newId, childName);

        TreeView.AddItem(childItem, parentId, -1, true);
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

        TreeView.SetRootItems(_rootItems);
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