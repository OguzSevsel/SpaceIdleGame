using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Android.Gradle;
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

    public NodeElement()
    {
        Node = CreateNode();
        ContextMenu.OnCreateConnectionButtonClicked += OnCreateConnection;
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

    private VisualElement CreateNode()
    {
        var node = new VisualElement();
        node.style.width = 200;
        node.style.height = 150;
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
        ParentNodeLabel.text = $"Parent Nodes: {ParentNodes.Count}";
        ChildNodeLabel.text = $"Child Nodes: {ChildNodes.Count}";
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
        if (evt.button == 1)
        {
            if (ContextMenu == null)
            {
                return;
            }

            ContextMenu.Parent = this;
            ContextMenu.MenuElement.style.display = DisplayStyle.Flex;

            Vector2 worldPos = this.Node.LocalToWorld(evt.localMousePosition);
            Vector2 overlayPos = Parent.overlay.WorldToLocal(worldPos);

            ContextMenu.MenuElement.style.left = overlayPos.x;
            ContextMenu.MenuElement.style.top = overlayPos.y;
            
            return;
        }

        if (evt.button == 0)
        {
            if (ContextMenu == null)
            {
                return;
            }

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

            if (this.ChildNodes.Contains(Parent.ConnectionStartedNode))
            {
                EditorUtility.DisplayDialog("Parent node of this node", "Parent Node Conflict", "OK");
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

    private void OnNodeMouseUp(VisualElement node, MouseUpEvent evt)
    {
        node.ReleaseMouse();
    }

    private void OnNodeMouseMove(VisualElement node, MouseMoveEvent evt)
    {
        if (node.HasMouseCapture())
        {
            float zoom = Parent.main.zoomFactor == 0 ? 1f : Parent.main.zoomFactor;
            Vector2 diff = (evt.mousePosition - startMouse) / zoom;

            OnNodeDragged(node, new Vector2(startPos.x + diff.x, startPos.y + diff.y));
        }
    }

    void OnNodeDragged(VisualElement node, Vector2 newPosition)
    {
        float minX = Mathf.Min(0, newPosition.x);
        float minY = Mathf.Min(0, newPosition.y);

        node.style.left = newPosition.x;
        node.style.top = newPosition.y;
    }

    #endregion

    #region UI Creation

    private void AddDialogueTextField(VisualElement node)
    {
        DialogueTextField = new TextField("Dialogue Text:");
        DialogueTextField.multiline = true;
        DialogueTextField.style.marginBottom = 0;
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

    private void AddNodeIdField(VisualElement node)
    {
        NodeIdField = new TextField("Node ID:");
        SetTextFieldWidthsPercent(NodeIdField);
        node.Add(NodeIdField);
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

    #endregion
}