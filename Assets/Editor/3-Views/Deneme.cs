using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class SceneHierarchyTreeView : EditorWindow
{
    private ScrollView _treeContainer;
    private List<TreeNode> _rootNodes = new();
    private TreeNode _pendingReparent;

    // Keep track of expanded states across refreshes
    private HashSet<GameObject> _expandedObjects = new();

    [MenuItem("Tools/Scene TreeView")]
    public static void ShowWindow()
    {
        var wnd = GetWindow<SceneHierarchyTreeView>();
        wnd.titleContent = new GUIContent("Scene TreeView");
        wnd.Show();
    }

    private void CreateGUI()
    {
        _treeContainer = new ScrollView();
        rootVisualElement.Add(_treeContainer);

        RefreshTree();
    }

    private void RefreshTree()
    {
        // Remember expanded states before rebuild
        _expandedObjects.Clear();
        SaveExpandedStates(_rootNodes);

        // Rebuild from scene
        _rootNodes.Clear();
        foreach (var go in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
        {
            _rootNodes.Add(BuildTreeRecursive(go, null));
        }

        // Restore expanded states after rebuild
        RestoreExpandedStates(_rootNodes);

        _treeContainer.Clear();
        foreach (var node in _rootNodes)
        {
            DrawNode(node, _treeContainer, 0);
        }
    }

    private void SaveExpandedStates(IEnumerable<TreeNode> nodes)
    {
        foreach (var n in nodes)
        {
            if (n.IsExpanded && n.GameObject != null)
                _expandedObjects.Add(n.GameObject);
            SaveExpandedStates(n.Children);
        }
    }

    private void RestoreExpandedStates(IEnumerable<TreeNode> nodes)
    {
        foreach (var n in nodes)
        {
            n.IsExpanded = n.GameObject && _expandedObjects.Contains(n.GameObject);
            RestoreExpandedStates(n.Children);
        }
    }

    private TreeNode BuildTreeRecursive(GameObject go, TreeNode parent)
    {
        var node = new TreeNode(go) { Parent = parent };
        foreach (Transform child in go.transform)
        {
            node.Children.Add(BuildTreeRecursive(child.gameObject, node));
        }
        return node;
    }

    private void DrawNode(TreeNode node, VisualElement parent, int depth)
    {
        var row = new VisualElement();
        row.style.flexDirection = FlexDirection.Row;
        row.style.marginLeft = depth * 16;
        row.style.alignItems = Align.Center;
        row.style.marginBottom = 2;

        // Foldout arrow
        var foldoutButton = new Button(() =>
        {
            node.IsExpanded = !node.IsExpanded;
            RefreshTree();
        })
        {
            text = node.Children.Count > 0 ? (node.IsExpanded ? "▼" : "▶") : ""
        };
        foldoutButton.style.width = 18;

        // Label
        var label = new Label(node.Name);
        label.style.flexGrow = 1;
        label.style.unityTextAlign = TextAnchor.MiddleLeft;
        label.style.paddingLeft = 4;

        row.Add(foldoutButton);
        row.Add(label);
        parent.Add(row);

        // Context menu
        row.RegisterCallback<ContextClickEvent>(evt =>
        {
            ShowContextMenu(node);
            evt.StopPropagation();
        });

        // Reparent click
        row.RegisterCallback<ClickEvent>(evt =>
        {
            if (_pendingReparent != null)
            {
                ChangeParent(_pendingReparent, node);
                _pendingReparent = null;
                RefreshTree();
            }
        });

        // Draw children if expanded
        if (node.IsExpanded)
        {
            foreach (var child in node.Children)
                DrawNode(child, parent, depth + 1);
        }
    }

    private void ShowContextMenu(TreeNode node)
    {
        var menu = new GenericMenu();
        menu.AddItem(new GUIContent("Reparent → Select Target"), false, () => _pendingReparent = node);
        menu.AddItem(new GUIContent("Create Child"), false, () => CreateChild(node));
        menu.AddItem(new GUIContent("Delete"), false, () => DeleteNode(node));
        menu.ShowAsContext();
    }

    private void ChangeParent(TreeNode node, TreeNode newParent)
    {
        if (node == newParent || IsChildOf(newParent, node))
        {
            Debug.LogWarning("Invalid parent operation");
            return;
        }

        Undo.SetTransformParent(node.GameObject.transform, newParent.GameObject.transform, "Reparent Object");
        node.GameObject.transform.SetParent(newParent.GameObject.transform);
        Debug.Log($"Moved {node.GameObject.name} under {newParent.GameObject.name}");
    }

    private void CreateChild(TreeNode parent)
    {
        var newObj = new GameObject("New Child");
        Undo.RegisterCreatedObjectUndo(newObj, "Create Child Object");
        newObj.transform.SetParent(parent.GameObject.transform);
        RefreshTree();
    }

    private void DeleteNode(TreeNode node)
    {
        Undo.DestroyObjectImmediate(node.GameObject);
        RefreshTree();
    }

    private bool IsChildOf(TreeNode potentialChild, TreeNode potentialParent)
    {
        var current = potentialChild.Parent;
        while (current != null)
        {
            if (current == potentialParent)
                return true;
            current = current.Parent;
        }
        return false;
    }
}

[System.Serializable]
public class TreeNode
{
    public GameObject GameObject;
    public TreeNode Parent;
    public List<TreeNode> Children = new();
    public bool IsExpanded = true;
    public string Name => GameObject ? GameObject.name : "(Missing)";
    public TreeNode(GameObject go) => GameObject = go;
}
