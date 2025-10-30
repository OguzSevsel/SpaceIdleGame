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
    private List<LineElement> _connections = new List<LineElement>();
    private List<NodeElement> _nodes = new List<NodeElement>();
    public bool isMakingConnection = false;
    public NodeElement ConnectionStartedNode;
    public NodeElement ConnectionEndedNode;

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
        ConnectionStartedNode = new NodeElement();
        ConnectionEndedNode = new NodeElement();
    }

    private void RegisterCallBacks()
    {
        main.AddNodeButton.clicked += AddNode;
    }

    private void AddNode()
    {
        _node = new NodeElement();
        
        main.ScrollView.Add(_node.Node);
        _node.Parent = this;

        foreach (var item in _nodes)
        {
            _node.OtherNodes.Add(item);
            item.OtherNodes.Add(_node);
        }

        _nodes.Add(_node);
        main.AdjustZoom();
    }
}






