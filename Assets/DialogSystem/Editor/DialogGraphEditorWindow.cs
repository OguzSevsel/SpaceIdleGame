using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Android.Gradle;
using UnityEditor;
using UnityEditor.iOS.Extensions.Common;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogGraphEditorWindow : EditorWindow
{
    public DialogGraphView main;
    private NodeElement _node;
    private LineElement _connection;
    public List<LineElement> Connections { get; set; }
    private List<NodeElement> _nodes = new List<NodeElement>();
    public bool isMakingConnection = false;
    public NodeElement ConnectionStartedNode;
    public NodeElement ConnectionEndedNode;
    public ContextMenu ContextMenu { get; set; }

    public VisualElement overlay;
    public DialogGraph selectedGraph;
    public ScriptableObject selectGraphObject;

    private bool handledObjectPickerEvent = false;

    [MenuItem("Tools/Dialog Graph Editor")]
    public static void ShowWindow()
    {
        var window = GetWindow<DialogGraphEditorWindow>();
        window.titleContent = new GUIContent("Dialog Graph");
    }

    private void CreateGUI()
    {
        main = new DialogGraphView(rootVisualElement);
        main.window = this;
        ConnectionStartedNode = new NodeElement();
        ConnectionEndedNode = new NodeElement();
        ContextMenu = new ContextMenu(120, 50);
        ContextMenu.CreateMenuItemsForNode(ContextMenu.MenuElement);
        ContextMenu.MenuElement.style.display = DisplayStyle.None;

        overlay = new VisualElement();
        overlay.style.position = Position.Absolute;
        overlay.style.left = 0;
        overlay.style.top = 0;
        overlay.style.right = 0;
        overlay.style.bottom = 0;
        overlay.pickingMode = PickingMode.Ignore;
        main.Parent.Add(overlay);
        main.ScrollView.Add(ContextMenu.MenuElement);

        Connections = new List<LineElement>();

        RegisterCallBacks();
    }

    private void RegisterCallBacks()
    {
        main.CreateNewDialogGraphButton.clicked += CreateNewDialogGraphButtonClicked;
        main.SelectDialogGraphButton.clicked += SelectDialogGraphButtonClicked;
        main.GenerateGraphButton.clicked += GenerateGraphButtonClicked;
        main.GenerateGraphInSceneButton.clicked += GenerateGraphInSceneButtonClicked;
        main.SyncDialogGraphButton.clicked += SyncDialogGraphButtonClicked;
        main.BackButton.clicked += BackButtonClicked;
        //main.GraphTitle.RegisterValueChangedCallback(ChangeGraphName);
        main.OnContextMenuCreated += CreateMainContextMenu;
        ContextMenu.OnDeleteNodeButtonClicked += DeleteNode;
    }

    private void SyncDialogGraphButtonClicked()
    {

    }

    private void BackButtonClicked()
    {
        ResetRightPanel();

        if (main.ScrollView.enabledInHierarchy)
        {
            main.ScrollView.SetEnabled(false);
            main.Canvas.Clear();
            main.Canvas.ClearBindings();
            main.ResetCanvas();
            main.zoomFactor = 1f;
            Connections.Clear();
            _nodes.Clear();
            main.SetTitleText("");
        }
    }

    private void ChangeGraphName(ChangeEvent<string> evt)
    {
        if (selectedGraph != null)
        {
            selectedGraph.name = evt.newValue;
            selectGraphObject.name = evt.newValue;
        }
    }

    private void CreateMainContextMenu()
    {
        if (main.ContextMenu == null)
        {
            main.ContextMenu = new ContextMenu(120, 50);
            main.ContextMenu.CreateMenuItemsForScrollView(main.ContextMenu.MenuElement);
            main.ContextMenu.MenuElement.style.display = DisplayStyle.Flex;
            main.ContextMenu.OnCreateNodeButtonClicked += AddNode;
            main.ScrollView.Add(main.ContextMenu.MenuElement);
        }
    }

    private void GenerateGraphInSceneButtonClicked()
    {

    }

    private void GenerateGraphButtonClicked()
    {

    }

    private void SelectDialogGraphButtonClicked()
    {
        Helpers.Show(main.BackButton);
        Helpers.Show(main.SyncDialogGraphButton);
        Helpers.Hide(main.CreateNewDialogGraphButton);
        Helpers.Hide(main.SelectDialogGraphButton);
        EditorGUIUtility.ShowObjectPicker<DialogGraph>(selectedGraph, false, "", 0);
    }

    private void OnGUI()
    {
        if (Event.current.commandName == "ObjectSelectorClosed" && !handledObjectPickerEvent)
        {
            handledObjectPickerEvent = true; // prevent double trigger

            selectedGraph = EditorGUIUtility.GetObjectPickerObject() as DialogGraph;
            selectGraphObject = EditorGUIUtility.GetObjectPickerObject() as ScriptableObject;

            if (selectedGraph != null)
            {
                main.ScrollView.SetEnabled(true);
                main.SetTitleText(selectedGraph.name);

                var viewport = main.ScrollView.contentContainer.worldBound;
                Rect localVisibleRect = main.ScrollView.contentContainer.WorldToLocal(viewport);

                Vector2 center = new Vector2(
                    localVisibleRect.x + localVisibleRect.width / 2f,
                    localVisibleRect.y + localVisibleRect.height / 2f
                );

                main.Canvas.style.left = center.x + main.Canvas.style.width.value.value / 2;
                main.Canvas.style.top = center.y + main.Canvas.style.height.value.value / 2;

                DialogNode node = selectedGraph.DialogNodes.First().DialogNode;
                AddNode();
                _node.PopulateNodeFields(node.NodeId, node.SpeakerName, node.Portrait, node.DialogText);

                CreateGraphInCanvas(selectedGraph, node);
                CreateConnectionsFromGraph(selectedGraph);
            }
            else
            {
                ResetRightPanel();
            }

            // optional: repaint UI
            Repaint();

            // Reset after a short delay or next frame
            EditorApplication.delayCall += () => handledObjectPickerEvent = false;
        }
    }

    private void CreateConnectionsFromGraph(DialogGraph graph)
    {
        List<VisualElement> childNodes = new List<VisualElement>();

        foreach (var nodeSO in graph.DialogNodes)
        {
            DialogNode node = nodeSO.DialogNode;

            ConnectionStartedNode = _nodes.Find(n => n.NodeIdField.value == node.NodeId);

            for (int i = 0; i < node.DialogOptions.Count; i++)
            {
                DialogOption option = node.DialogOptions[i];

                if (option.NextNodeId != string.Empty)
                {
                    DialogNode optionNode = graph.DialogNodes.Find(node => node.DialogNode.NodeId == option.NextNodeId).DialogNode;

                    ConnectionEndedNode = _nodes.Find(n => n.NodeIdField.value == optionNode.NodeId);

                    childNodes.Add(ConnectionEndedNode.Node);

                    LineElement line = new LineElement(
                        ConnectionStartedNode.Node,
                        ConnectionEndedNode.Node,
                        main.Canvas,
                        ConnectionStartedNode,
                        ConnectionEndedNode
                    );

                    Connections.Add(line);
                    line.Canvas.Add(line);
                    line.CreateTextBox();
                    line.OptionText.value = option.OptionText;
                    line.Canvas.schedule.Execute(() =>
                    {
                        line.MarkDirtyRepaint();
                        line.UpdateTextboxPosition();
                    }).Every(5);

                    ConnectionEndedNode.ParentNodes.Add(ConnectionStartedNode);
                    ConnectionStartedNode.ChildNodes.Add(ConnectionEndedNode);
                    isMakingConnection = false;
                    ConnectionStartedNode.UpdateInfoFields();
                    ConnectionEndedNode.UpdateInfoFields();
                }
            }
            
            AlignNodes(ConnectionStartedNode.Node, childNodes);
            childNodes.Clear();
        }
    }

    private void AlignNodes(VisualElement parentNode, List<VisualElement> childNodes)
    {
        int count = childNodes.Count;
        

        float parentLeft = parentNode.style.left.value.value;
        float parentTop = parentNode.style.top.value.value;
        float parentWidth = parentNode.style.width.value.value;
        float parentHeight = parentNode.style.height.value.value;

        float maxWidth = -(count * parentWidth);

        foreach (var node in childNodes)
        {
            int index = childNodes.IndexOf(node);

            float childLeft = node.style.left.value.value;
            float childTop = node.style.top.value.value;
            float childWidth = node.style.width.value.value;
            float childHeight = node.style.height.value.value;

            float newTop = parentTop + parentHeight + 300;

            if (count == 1)
            {
                node.style.left = parentLeft - (index * childWidth);
                node.style.top = newTop;
                return;
            }

            if (count % 2 == 0)
            {
                node.style.left = parentLeft + parentWidth / 2 + (maxWidth / 2);
                node.style.top = newTop;
                maxWidth += parentWidth + parentWidth;
            }
            else
            {
                node.style.left = parentLeft + parentWidth / 2 + (maxWidth / 2);
                node.style.top = newTop;
                maxWidth += parentWidth + parentWidth;
            }    
        }
    }

    private void CreateGraphInCanvas(DialogGraph graph, DialogNode node)
    {
        if (node != null)
        {
            foreach (var option in node.DialogOptions)
            {
                if (option.NextNodeId != string.Empty)
                {
                    DialogNode optionNode = graph.DialogNodes.Find(node => node.DialogNode.NodeId == option.NextNodeId).DialogNode;

                    AddNode();

                    _node.PopulateNodeFields(optionNode.NodeId, optionNode.SpeakerName, optionNode.Portrait, optionNode.DialogText);

                    CreateGraphInCanvas(graph, optionNode);
                }
            }
        }
    }

    private void CreateNewDialogGraphButtonClicked()
    {
        main.ScrollView.SetEnabled(true);
        main.SetTitleText("New Dialog Graph");
        Helpers.Show(main.GenerateGraphButton);
        Helpers.Show(main.GenerateGraphInSceneButton);
        Helpers.Show(main.BackButton);
        Helpers.Hide(main.SelectDialogGraphButton);
        Helpers.Hide(main.CreateNewDialogGraphButton);
    }

    private void ResetRightPanel()
    {
        Helpers.Show(main.CreateNewDialogGraphButton);
        Helpers.Show(main.SelectDialogGraphButton);
        Helpers.Hide(main.BackButton);
        Helpers.Hide(main.GenerateGraphButton);
        Helpers.Hide(main.GenerateGraphInSceneButton);
        Helpers.Hide(main.SyncDialogGraphButton);
    }

    private void AddNode()
    {
        _node = new NodeElement();

        main.Canvas.Add(_node.Node);

        _node.Node.style.left = main.MouseElement.style.left;
        _node.Node.style.top = main.MouseElement.style.top;

        _node.Parent = this;
        _node.ContextMenu = this.ContextMenu;

        if (main.ContextMenu != null)
        {
            main.ContextMenu.MenuElement.style.display = DisplayStyle.None;
        }

        _nodes.Add(_node);
    }

    private void DeleteNode(NodeElement element)
    {
        List<LineElement> deleteLines = new List<LineElement>();

        foreach (var node in _nodes)
        {
            if (node.ChildNodes.Contains(element))
            {
                node.ChildNodes.Remove(element);
                node.UpdateInfoFields();
            }

            if (node.ParentNodes.Contains(element))
            {
                node.ParentNodes.Remove(element);
                node.UpdateInfoFields();
            }
        }

        element.ChildNodes.Clear();
        element.ParentNodes.Clear();

        foreach (var line in Connections)
        {
            if (line.NodeFrom == element || line.NodeTo == element)
            {
                deleteLines.Add(line);
            }
        }

        foreach (var line in deleteLines)
        {
            Connections.Remove(line);
            line.Canvas.Remove(line);
            line.Canvas.Remove(line.OptionText);
        }

        if (main.Canvas.Contains(element.Node))
        {
            main.Canvas.Remove(element.Node);
        }

        ContextMenu.MenuElement.style.display = DisplayStyle.None;
    }
}






