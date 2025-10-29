using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogGraphEditorWindow : EditorWindow
{
    public DialogGraphView main;
    private NodeElement _node;
    private LineElement _connection;
    private List<LineElement> _connections = new List<LineElement>();
    private List<NodeElement> _nodes = new List<NodeElement>();

    [MenuItem("Tools/Dialog Graph Editor")]
    public static void ShowWindow()
    {
        var window = GetWindow<DialogGraphEditorWindow>();
        window.titleContent = new GUIContent("Dialog Graph");
    }

    private void CreateGUI()
    {
        main = new DialogGraphView(rootVisualElement);
        RegisterCallBacks();
    }

    private void RegisterCallBacks()
    {
        main.AddNodeButton.clicked += AddNode;
    }

    private void AddNode()
    {
        _node = new NodeElement();
        
        main.ScrollView.Add(_node.Node);
        _node.OtherNodes = _nodes;
        _node.Parent = this;
        _nodes.Add(_node);
        main.AdjustZoom();

        //foreach (var conn in _connections)
        //{
        //    main.ScrollView.schedule.Execute(() => conn.MarkDirtyRepaint()).Every(16);
        //}
    }
}

// Editor or runtime: works inside EditorWindow UI Toolkit
public class LineElement : VisualElement
{
    private readonly VisualElement from;
    private readonly VisualElement to;

    public LineElement(VisualElement from, VisualElement to)
    {
        this.from = from;
        this.to = to;
        pickingMode = PickingMode.Ignore;
        style.position = Position.Absolute;

        generateVisualContent += OnGenerateVisualContent;
    }

    private void OnGenerateVisualContent(MeshGenerationContext ctx)
    {
        if (from == null || to == null) return;

        // World-space rects
        Rect fromRectWorld = from.worldBound;
        Rect toRectWorld = to.worldBound;

        // World-space centers
        Vector2 fromCenterWorld = fromRectWorld.center;
        Vector2 toCenterWorld = toRectWorld.center;

        // Direction from A -> B in world space
        Vector2 worldDir = (toCenterWorld - fromCenterWorld);
        float dist = worldDir.magnitude;
        if (dist < 0.001f)
            return;

        Vector2 worldDirNorm = worldDir / dist;

        // compute best edge points in world space
        Vector2 startWorld = GetEdgePointWorld(fromRectWorld, worldDirNorm);
        Vector2 endWorld = GetEdgePointWorld(toRectWorld, -worldDirNorm);

        // convert world-space points into this element's local space
        Vector2 startLocal = this.WorldToLocal(startWorld);
        Vector2 endLocal = this.WorldToLocal(endWorld);

        // if conversion produced NaNs or huge values (defensive), fallback to centers
        if (!IsFiniteVector(startLocal) || !IsFiniteVector(endLocal))
        {
            startLocal = this.WorldToLocal(fromCenterWorld);
            endLocal = this.WorldToLocal(toCenterWorld);
        }

        var painter = ctx.painter2D;
        painter.lineWidth = 3f;
        painter.strokeColor = Color.white;

        // Draw main line
        painter.BeginPath();
        painter.MoveTo(startLocal);
        painter.LineTo(endLocal);
        painter.Stroke();

        // Draw arrowhead at 'endLocal'
        Vector2 dirLocal = (endLocal - startLocal).normalized;
        if (dirLocal.sqrMagnitude < 0.0001f) dirLocal = Vector2.up; // fallback

        Vector2 perp = new Vector2(-dirLocal.y, dirLocal.x);
        Vector2 arrowLeft = endLocal - dirLocal * 12f + perp * 6f;
        Vector2 arrowRight = endLocal - dirLocal * 12f - perp * 6f;

        painter.BeginPath();
        painter.MoveTo(endLocal);
        painter.LineTo(arrowLeft);
        painter.LineTo(arrowRight);
        painter.ClosePath();
        painter.fillColor = Color.black;
        painter.Fill();
    }

    // Choose edge point on rect based on direction vector (world-space)
    private Vector2 GetEdgePointWorld(Rect rect, Vector2 dir)
    {
        Vector2 absDir = new Vector2(Mathf.Abs(dir.x), Mathf.Abs(dir.y));

        // If rect has zero size, return center
        if (rect.width <= 0 || rect.height <= 0)
            return rect.center;

        if (absDir.x > absDir.y)
        {
            // horizontal edge
            if (dir.x > 0)
                return new Vector2(rect.xMax, rect.center.y); // right
            else
                return new Vector2(rect.xMin, rect.center.y); // left
        }
        else
        {
            // vertical edge
            if (dir.y > 0)
                return new Vector2(rect.center.x, rect.yMax); // top
            else
                return new Vector2(rect.center.x, rect.yMin); // bottom
        }
    }

    private bool IsFiniteVector(Vector2 v)
    {
        return float.IsFinite(v.x) && float.IsFinite(v.y) && Mathf.Abs(v.x) < 1e8f && Mathf.Abs(v.y) < 1e8f;
    }
}

public class NodeElement
{
    public VisualElement Node { get; private set; }
    public TextField NodeIdField { get; private set; }
    public TextField SpeakerNameField { get; private set; }
    public ObjectField PortraitField { get; private set; }
    public TextField DialogueTextField { get; private set; }
    public Button AddOptionButton { get; private set; }
    public ListView OptionListView { get; private set; }

    public List<NodeElement> OtherNodes = new List<NodeElement>();
    public DialogGraphEditorWindow Parent { get; set; }

    Vector2 startMouse = Vector2.zero;
    Vector2 startPos = Vector2.zero;

    public NodeElement()
    {
        Node = CreateNode();
    }

    private VisualElement CreateNode()
    {
        var node = new VisualElement();
        node.style.width = 300;
        node.style.height = 300;
        node.style.backgroundColor = Color.blue;
        node.style.borderBottomLeftRadius = 6;
        node.style.borderBottomRightRadius = 6;
        node.style.borderTopLeftRadius = 6;
        node.style.borderTopRightRadius = 6;
        node.style.position = Position.Absolute;
        node.style.flexDirection = FlexDirection.Column;

        AddNodeIdField(node);
        AddPortraitField(node);
        AddSpeakerNameField(node);
        AddOptionListView(node);
        AddOptionButtonUI(node);
        AddDialogueTextField(node);

        EnableNodeDragging(node);
        return node;
    }


    private void AddOptionButtonClickHandler()
    {
        OptionElement optionElement = new();
        Node.Insert(3, optionElement.OptionCard);
        optionElement.PopulateNodeId(OtherNodes);
        optionElement.ParentNode = this;
        optionElement.OnOptionCreated += OptionCreatedHandler;
    }

    private void OptionCreatedHandler(OptionElement element)
    {
        NodeElement nextNode = OtherNodes.Find(node => node.NodeIdField.value == element.NextIdField.value);

        LineElement line = new LineElement(this.Node, nextNode.Node);

        Parent.main.ScrollView.Add(line);

        Parent.main.ScrollView.schedule.Execute(() => line.MarkDirtyRepaint()).Every(16);
    }


    #region Dragging Functionality

    void EnableNodeDragging(VisualElement node)
    {
        node.RegisterCallback<MouseDownEvent>(evt => OnNodeMouseDown(node, evt));
        node.RegisterCallback<MouseMoveEvent>(evt => OnNodeMouseMove(node, evt));
        node.RegisterCallback<MouseUpEvent>(evt => OnNodeMouseUp(node, evt));
    }

    private void OnNodeMouseDown(VisualElement node, MouseDownEvent evt)
    {
        startMouse = evt.mousePosition;
        startPos = new Vector2(node.resolvedStyle.left, node.resolvedStyle.top);
        node.CaptureMouse();
        evt.StopPropagation();
    }

    private void OnNodeMouseMove(VisualElement node, MouseMoveEvent evt)
    {
        if (node.HasMouseCapture())
        {
            Vector2 diff = evt.mousePosition - startMouse;
            OnNodeDragged(node, new Vector2(startPos.x + diff.x, startPos.y + diff.y));
        }
    }

    private void OnNodeMouseUp(VisualElement node, MouseUpEvent evt)
    {
        node.ReleaseMouse();
    }

    void OnNodeDragged(VisualElement node, Vector2 newPosition)
    {
        // Prevent moving left or up
        float clampedX = Mathf.Max(0, newPosition.x);
        float clampedY = Mathf.Max(0, newPosition.y);

        node.style.left = clampedX;
        node.style.top = clampedY;
    }

    #endregion

    #region UI Creation

    private void AddDialogueTextField(VisualElement node)
    {
        DialogueTextField = new TextField("Dialogue Text:");
        DialogueTextField.multiline = true;
        node.Add(DialogueTextField);
    }

    private void AddOptionListView(VisualElement node)
    {
        OptionListView = new ListView();
        node.Add(OptionListView);
    }

    private void AddSpeakerNameField(VisualElement node)
    {
        SpeakerNameField = new TextField("Speaker Name:");
        node.Add(SpeakerNameField);
    }

    private void AddPortraitField(VisualElement node)
    {
        PortraitField = new ObjectField("Portrait:");
        PortraitField.objectType = typeof(Sprite);
        PortraitField.allowSceneObjects = false;

        SetObjectFieldWidthsPercent(PortraitField);

        PortraitField.style.paddingBottom = 0;
        PortraitField.style.paddingLeft = 0;
        PortraitField.style.paddingRight = 0;
        PortraitField.style.paddingTop = 0;

        node.Add(PortraitField);
    }

    void SetObjectFieldWidthsPercent(ObjectField objField)
    {
        objField.RegisterCallback<AttachToPanelEvent>(evt =>
        {
            var children = objField.Children().ToList();
            if (children.Count >= 2)
            {
                children[0].AddToClassList("objectLabel"); // label
                children[1].AddToClassList("objectField"); // field
            }
        });
    }




    private void AddNodeIdField(VisualElement node)
    {
        NodeIdField = new TextField("Node ID:");

        node.Add(NodeIdField);
    }

    private void AddOptionButtonUI(VisualElement node)
    {
        AddOptionButton = new Button();
        AddOptionButton.text = "Add Option";
        AddOptionButton.clicked += AddOptionButtonClickHandler;

        node.Add(AddOptionButton);
    }

    #endregion
}

public class OptionElement
{
    private TextField _optionTextField;
    public DropdownField NextIdField { get; private set; }
    private Button _addOptionButton;
    private VisualElement _optionContainer;
    public VisualElement OptionCard { get; private set; }
    public DialogOption DialogOption { get; private set; }
    public NodeElement ParentNode { get; set; }

    public event Action<OptionElement> OnOptionCreated;

    public OptionElement()
    {
        OptionCard = CreateOptionCard();
    }

    private VisualElement CreateOptionCard()
    {
        _optionTextField = new TextField("Option Text:");
        NextIdField = new DropdownField("Next Id:");
        _addOptionButton = new Button();
        _addOptionButton.text = "Create Option Dialog";

        _optionContainer = new VisualElement();
        _optionContainer.name = "OptionContainer";

        _optionContainer.Add(_optionTextField);
        _optionContainer.Add(NextIdField);
        _optionContainer.Add(_addOptionButton);
        _addOptionButton.clicked += AddOption;

        return _optionContainer;
    }

    private void AddOption()
    {
        DialogOption = new DialogOption
        {
            OptionText = _optionTextField.value,
            NextNodeId = NextIdField.value
        };

        OnOptionCreated?.Invoke(this);
    }

    public void PopulateNodeId(List<NodeElement> nodes)
    {
        foreach (var node in nodes)
        {
            NextIdField.choices.Add(node.NodeIdField.value);
        }
    }
}