using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class NodeElement
{
    public ScrollView Node { get; private set; }
    public TextField NodeIdField { get; private set; }
    public TextField SpeakerNameField { get; private set; }
    public UnityEditor.UIElements.ObjectField PortraitField { get; private set; }
    public TextField DialogueTextField { get; private set; }
    public Button AddOptionButton { get; private set; }
    public DialogGraphEditorWindow Parent { get; set; }
    public ContextMenu ContextMenu { get; set; }
    public LineElement TempLine { get; set; }
    public List<NodeElement> ChildNodes { get; set; } = new List<NodeElement>();
    public List<NodeElement> ParentNodes { get; set; } = new List<NodeElement>();

    public Label ParentNodeLabel { get; private set; }
    public Label ChildNodeLabel { get; private set; }

    Vector2 startMouse = Vector2.zero;
    Vector2 startPos = Vector2.zero;

    private IVisualElementScheduledItem tempLineSchedule;
    private string leftClassName = "objectLabel";
    private string rightClassName = "objectField";

    public NodeElement()
    {
        Node = CreateNode();
        ContextMenu.OnCreateConnectionButtonClicked += OnCreateConnection;
    }

    public void PopulateNodeFields(string nodeId, string speakerName, Sprite portrait, string dialogueText)
    {
        NodeIdField.value = nodeId;
        SpeakerNameField.value = speakerName;
        PortraitField.value = portrait;
        DialogueTextField.value = dialogueText;
    }

    private void OnCreateConnection(NodeElement element)
    {
        if (element == this)
        {
            Parent.isMakingConnection = true;
            Parent.ConnectionStartedNode = this;
            ContextMenu.MenuElement.style.display = DisplayStyle.None;
            TempLine = new LineElement(this.Node, Parent.main.MouseElement, Parent.main.ScrollView, nodeFrom: this);
            TempLine.Canvas.Add(TempLine);
            Parent.main.TempLine = this.TempLine;
            tempLineSchedule = TempLine.Canvas.schedule.Execute(() => TempLine.MarkDirtyRepaint()).Every(5);
        }
    }

    private ScrollView CreateNode()
    {
        var node = new ScrollView();
        node.style.width = 300;
        node.style.height = 200;
        node.mouseWheelScrollSize = 0;

        Utility.SetBorderWidth(node, 1);
        Utility.SetBorderColor(node, Utility.HexToColor(Utility.ColorBorder));
        Utility.SetBorderRadius(node, 6);
        Utility.SetPadding(node, 6);
        node.style.position = Position.Absolute;
        node.style.flexDirection = FlexDirection.Column;

        AddNodeIdField(node);
        AddPortraitField(node);
        AddSpeakerNameField(node);
        AddDialogueTextField(node);
        EnableNodeDragging(node);
        AddInfoFields(node);

        return node;
    }

    private void AddInfoFields(VisualElement node)
    {
        ParentNodeLabel = new Label($"Parent Nodes: {ParentNodes.Count}");
        ChildNodeLabel = new Label($"Child Nodes: {ChildNodes.Count}");
        node.Add(ParentNodeLabel);
        node.Add(ChildNodeLabel);
    }

    public void UpdateInfoFields()
    {
        if (ParentNodeLabel != null && ChildNodeLabel != null)
        {
            ParentNodeLabel.text = $"Parent Nodes: {ParentNodes.Count}";
            ChildNodeLabel.text = $"Child Nodes: {ChildNodes.Count}";
        }
    }
    
    #region Dragging Functionality

    void EnableNodeDragging(ScrollView node)
    {
        Utility.OnMouseEnter(node, "Highlight");
        Utility.OnMouseLeave(node, "Highlight");
        node.RegisterCallback<MouseDownEvent>(evt => OnNodeMouseDown(node, evt));
        node.RegisterCallback<MouseMoveEvent>(evt => OnNodeMouseMove(node, evt));
        node.RegisterCallback<MouseUpEvent>(evt => OnNodeMouseUp(node, evt));
    }

    private void OnNodeMouseDown(ScrollView node, MouseDownEvent evt)
    {
        if (evt.button == (int)MouseButton.Right)
        {
            if (ContextMenu == null)
            {
                return;
            }

            ContextMenu.Parent = this;
            ContextMenu.MenuElement.style.display = DisplayStyle.Flex;

            Vector2 worldPos = this.Node.LocalToWorld(evt.localMousePosition);
            Vector2 overlayPos = Parent.main.ScrollView.WorldToLocal(worldPos);

            ContextMenu.MenuElement.style.left = overlayPos.x;
            ContextMenu.MenuElement.style.top = overlayPos.y;
            node.AddToClassList("Highlight");

            return;
        }

        if (evt.button == (int)MouseButton.Left)
        {

            if (!Parent.isMakingConnection)
            {
                startMouse = evt.mousePosition;
                startPos = new Vector2(node.resolvedStyle.left, node.resolvedStyle.top);

                node.CaptureMouse();
                evt.StopPropagation();

                ContextMenu.MenuElement.style.display = DisplayStyle.None;
                return;
            }

            if (Parent.ConnectionStartedNode.TempLine == null)
            {
                return;
            }

            Parent.ConnectionStartedNode.TempLine.Canvas.Remove(Parent.ConnectionStartedNode.TempLine);
            Parent.ConnectionStartedNode.tempLineSchedule.Pause();
            Parent.ConnectionStartedNode.TempLine = null;
            ContextMenu.Parent = null;

            Parent.ConnectionEndedNode = this;

            if (Parent.ConnectionStartedNode == Parent.ConnectionEndedNode)
            {
                EditorUtility.DisplayDialog("You cant bind the same node", "Cant bind the same node", "OK");
                Parent.isMakingConnection = false;
                return;
            }

            if (this.ChildNodes.Contains(Parent.ConnectionStartedNode))
            {
                EditorUtility.DisplayDialog("Parent node of this node", "Parent Node Conflict", "OK");
                Parent.isMakingConnection = false;
                return;
            }

            if (Parent.ConnectionStartedNode.ChildNodes.Contains(Parent.ConnectionEndedNode))
            {
                EditorUtility.DisplayDialog("There is already a connection", "Connection already established", "OK");
                Parent.isMakingConnection = false;
                return;
            }

            LineElement line = new LineElement(
                Parent.ConnectionStartedNode.Node,
                Parent.ConnectionEndedNode.Node,
                Parent.main.Canvas,
                Parent.ConnectionStartedNode,
                Parent.ConnectionEndedNode
            );

            Parent.Connections.Add(line);
            line.Canvas.Add(line);
            line.CreateTextBox();
            line.Canvas.schedule.Execute(() =>
            {
                line.MarkDirtyRepaint();
                line.UpdateTextboxPosition();
            }).Every(5);

            this.ParentNodes.Add(Parent.ConnectionStartedNode);
            Parent.ConnectionStartedNode.ChildNodes.Add(this);
            Parent.isMakingConnection = false;
            UpdateInfoFields();
            Parent.ConnectionStartedNode.UpdateInfoFields();
        }
    }

    private void OnNodeMouseUp(ScrollView node, MouseUpEvent evt)
    {
        node.ReleaseMouse();
    }

    private void OnNodeMouseMove(ScrollView node, MouseMoveEvent evt)
    {
        if (node.HasMouseCapture())
        {
            float zoom = Parent.main.zoomFactor == 0 ? 1f : Parent.main.zoomFactor;
            Vector2 diff = (evt.mousePosition - startMouse) / zoom;

            OnNodeDragged(node, new Vector2(startPos.x + diff.x, startPos.y + diff.y));
        }
    }

    void OnNodeDragged(ScrollView node, Vector2 newPosition)
    {
        float minX = Mathf.Min(0, newPosition.x);
        float minY = Mathf.Min(0, newPosition.y);

        node.style.left = newPosition.x;
        node.style.top = newPosition.y;
    }

    #endregion

    #region UI Creation

    private void AddDialogueTextField(ScrollView node)
    {
        DialogueTextField = new TextField("Dialogue Text:");
        DialogueTextField.multiline = true;
        DialogueTextField.style.marginBottom = 0;
        Set(DialogueTextField);
        node.Add(DialogueTextField);
    }

    private void AddSpeakerNameField(ScrollView node)
    {
        SpeakerNameField = new TextField("Speaker Name:");
        Set(SpeakerNameField);
        node.Add(SpeakerNameField);
    }

    private void AddPortraitField(ScrollView node)
    {
        PortraitField = new ObjectField("Portrait:");
        PortraitField.objectType = typeof(Sprite);
        PortraitField.allowSceneObjects = false;

        Utility.SetPadding(PortraitField, 0);
        Set(PortraitField);
        node.Add(PortraitField);
    }

    private void AddNodeIdField(ScrollView node)
    {
        NodeIdField = new TextField("Node ID:");
        Set(NodeIdField);
        node.Add(NodeIdField);
    }
    private void Set(VisualElement element)
    {
        Utility.SetFieldWidthPercentages(element, leftClassName, rightClassName);
    }

    #endregion
}


public enum MouseButton
{
    Left = 0,
    Right = 1,
    Middle = 2
}