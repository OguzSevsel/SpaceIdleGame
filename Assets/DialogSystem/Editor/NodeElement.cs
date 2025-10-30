using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class NodeElement
{
    public VisualElement Node { get; private set; }
    public TextField NodeIdField { get; private set; }
    public TextField SpeakerNameField { get; private set; }
    public ObjectField PortraitField { get; private set; }
    public TextField DialogueTextField { get; private set; }
    public Button AddOptionButton { get; private set; }
    public List<NodeElement> OtherNodes = new List<NodeElement>();
    public DialogGraphEditorWindow Parent { get; set; }
    public OptionElement OptionElement { get; private set; }
    public OptionElement EarlierOptionElement { get; private set; }
    public ContextMenu ContextMenu { get; private set; }

    public List<LineElement> Lines = new List<LineElement>();
    public LineElement TempLine { get; set; }

    Vector2 startMouse = Vector2.zero;
    Vector2 startPos = Vector2.zero;

    public NodeElement()
    {
        Node = CreateNode();
        ContextMenu = new ContextMenu();
        Node.Add(ContextMenu.MenuElement);
        ContextMenu.OnCreateConnectionButtonClicked += OnCreateConnection;
        ContextMenu.MenuElement.style.display = DisplayStyle.None;
        OptionElement = new OptionElement();
        EarlierOptionElement = new OptionElement();
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
        AddOptionButtonUI(node);
        AddDialogueTextField(node);

        EnableNodeDragging(node);

        return node;
    }


    private void AddOptionButtonClickHandler()
    {
        OptionElement optionElement = new();
        Node.Insert(3, optionElement.OptionCard);
        optionElement.ParentNode = this;
        optionElement.OnOptionCreated += OptionCreatedHandler;
        optionElement.PopulateNodeId(OtherNodes);
        this.AddOptionButton.visible = false;
        OptionElement = optionElement;
    }

    private void OptionCreatedHandler(OptionElement element)
    {
        if (EarlierOptionElement.NextIdField.value == OptionElement.NextIdField.value)
        {
            EditorUtility.DisplayDialog("You cant do that", "you cant do that", "ok");
        }
        else
        {
            NodeElement nextNode = OtherNodes.Find(node => node.NodeIdField.value == element.NextIdField.value);
            LineElement line = new LineElement(this.Node, nextNode.Node);
            this.Lines.Add(line);

            Parent.main.ScrollView.Add(line);
            Parent.main.ScrollView.schedule.Execute(() => line.MarkDirtyRepaint()).Every(5);
            Node.Remove(element.OptionCard);
            this.AddOptionButton.visible = true;
            EarlierOptionElement = OptionElement;
        }
    }

    private void OnCreateConnection()
    {
        Parent.isMakingConnection = true;
        Parent.ConnectionStartedNode = this;
        ContextMenu.MenuElement.style.display = DisplayStyle.None;
        TempLine = new LineElement(this.Node, Parent.main.MouseElement, nodeFrom: this);
        this.Lines.Add(TempLine);
        Parent.main.ScrollView.contentContainer.Add(TempLine);
        Parent.main.ScrollView.contentContainer.schedule.Execute(() => TempLine.MarkDirtyRepaint()).Every(5);
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
        if (evt.button == 0)
        {
            if (Parent.isMakingConnection && this != Parent.ConnectionStartedNode)
            {
                Parent.ConnectionEndedNode = this;

                if (Parent.ConnectionStartedNode.TempLine != null)
                {
                    if (Parent.main.ScrollView != null)
                    {
                        if (Parent.ConnectionStartedNode.TempLine.parent != null)
                            Parent.ConnectionStartedNode.TempLine.parent.Remove(Parent.ConnectionStartedNode.TempLine);

                        Parent.ConnectionStartedNode.Lines.Remove(Parent.ConnectionStartedNode.TempLine);
                    }
                }

                if (Parent.ConnectionStartedNode != null && Parent.ConnectionEndedNode != null)
                {
                    LineElement line = new LineElement(
                        Parent.ConnectionStartedNode.Node,
                        Parent.ConnectionEndedNode.Node,
                        Parent.ConnectionStartedNode,
                        Parent.ConnectionEndedNode
                    );

                    this.Lines.Add(line);
                    Parent.ConnectionStartedNode.Lines.Add(line);
                    Parent.main.ScrollView.Add(line);
                    Parent.main.ScrollView.schedule.Execute(() => line.MarkDirtyRepaint()).Every(5);
                }

                Parent.isMakingConnection = false;
            }
            else
            {
                startMouse = evt.mousePosition;
                startPos = new Vector2(node.resolvedStyle.left, node.resolvedStyle.top);
                node.CaptureMouse();
                evt.StopPropagation();
                ContextMenu.MenuElement.style.display = DisplayStyle.None;
            }   
        }
        else if (evt.button == 1)
        {
            ContextMenu.MenuElement.style.display = DisplayStyle.Flex;
            ContextMenu.MenuElement.style.left = evt.localMousePosition.x;
            ContextMenu.MenuElement.style.top = evt.localMousePosition.y;
        }
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
        SetTextFieldWidthsPercent(DialogueTextField);
        node.Add(DialogueTextField);
    }

    private void AddSpeakerNameField(VisualElement node)
    {
        SpeakerNameField = new TextField("Speaker Name:");
        SetTextFieldWidthsPercent(SpeakerNameField);
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
                children[0].AddToClassList("objectLabel");
                children[1].AddToClassList("objectField");
            }
        });
    }


    void SetTextFieldWidthsPercent(TextField textField)
    {
        textField.RegisterCallback<AttachToPanelEvent>(evt =>
        {
            var children = textField.Children().ToList();
            if (children.Count >= 2)
            {
                children[0].AddToClassList("objectLabel");
                children[1].AddToClassList("objectField");
            }
        });
    }

    private void AddNodeIdField(VisualElement node)
    {
        NodeIdField = new TextField("Node ID:");
        SetTextFieldWidthsPercent(NodeIdField);

        NodeIdField.RegisterCallback<FocusOutEvent>(evt => OnNodeIdFieldChangedHandler());

        node.Add(NodeIdField);
    }

    private void OnNodeIdFieldChangedHandler()
    {
        foreach (var node in OtherNodes)
        {
            if (node != this && OptionElement != null)
            {
                foreach (var line in node.Lines)
                {
                    Parent.main.ScrollView.Remove(line);
                }
                node.Lines.Clear();
                node.OptionElement.PopulateNodeId(node.OtherNodes);
            }
        }
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