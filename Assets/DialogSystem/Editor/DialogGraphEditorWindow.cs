using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Android.Gradle;
using UnityEditor;
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
        ContextMenu = new ContextMenu();
        ContextMenu.CreateMenuItemsForNode(ContextMenu.MenuElement);
        ContextMenu.MenuElement.style.display = DisplayStyle.None;

        overlay = new VisualElement();
        overlay.style.position = Position.Absolute;
        overlay.style.left = 0;
        overlay.style.top = 0;
        overlay.style.right = 0;
        overlay.style.bottom = 0;
        overlay.pickingMode = PickingMode.Ignore;
        rootVisualElement.Add(overlay);
        overlay.Add(ContextMenu.MenuElement);

        Connections = new List<LineElement>();

        RegisterCallBacks();
    }

    

    private void RegisterCallBacks()
    {
        main.OnContextMenuCreated += CreateMainContextMenu;
        ContextMenu.OnDeleteNodeButtonClicked += DeleteNode;
    }

    private void CreateMainContextMenu()
    {
        if (main.ContextMenu == null)
        {
            main.ContextMenu = new ContextMenu();
            main.ContextMenu.CreateMenuItemsForScrollView(main.ContextMenu.MenuElement);
            main.ContextMenu.MenuElement.style.display = DisplayStyle.Flex;
            main.ContextMenu.OnCreateNodeButtonClicked += AddNode;
            overlay.Add(main.ContextMenu.MenuElement);
        }
    }


    private void AddNode()
    {
        _node = new NodeElement();

        main.Canvas.Add(_node.Node);

        _node.Node.style.left = main.MouseElement.style.left;
        _node.Node.style.top = main.MouseElement.style.top;

        _node.Parent = this;
        _node.ContextMenu = this.ContextMenu;

        main.ContextMenu.MenuElement.style.display = DisplayStyle.None;

        _nodes.Add(_node);
        //main.AdjustZoom();
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






