using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Rendering;

public class DialogGraphView
{
    private string _treePath = "Assets/DialogSystem/Editor/1-UI Documents/DialogGraphEditorWindow.uxml";
    private string _stylePath = "Assets/DialogSystem/Editor/2-Styles/DialogGraphEditorWindowStyle.uss";

    public VisualTreeAsset TreeAsset { get; private set; }
    public StyleSheet StyleSheet { get; private set; }

    public VisualElement Parent;
    public DialogGraphEditorWindow window { get; set; }
    public ScrollView ScrollView { get; private set; }
    public VisualElement MouseElement { get; private set; }

    public ContextMenu ContextMenu { get; set; }

    private Vector2 dragStart;
    private Vector2 scrollStart;
    private bool isPanning;

    float zoomFactor = 1f;
    float zoomScale = 1f;
    float zoomStep = 0.1f;
    float minZoom = 0.7f;
    float maxZoom = 1.3f;
    private Vector2 grabOffset;
    private Label infoLabel;
    private Label infoLabel1;

    public event Action OnCreateNode;
    public event Action OnContextMenuCreated;

    public DialogGraphView(VisualElement root)
    {
        BindTreeAsset(root);
        BindUIElements();  
        MouseElement = new VisualElement();
        infoLabel = new Label("Deneme");
        infoLabel1 = new Label("Deneme");
        Parent.Add(infoLabel);
        Parent.Add(infoLabel1);
        MouseElement.style.width = 1;
        MouseElement.style.height = 1;
        MouseElement.style.position = Position.Absolute;
        ScrollView.contentContainer.Add(MouseElement);
    }

    private void BindUIElements()
    {
        this.ScrollView = Parent.Q<ScrollView>("deneme");

        ScrollView.contentContainer.style.width = 10000;
        ScrollView.contentContainer.style.height = 10000;
        ScrollView.scrollOffset = new Vector2(5000, 5000);


        this.ScrollView.RegisterCallback<PointerDownEvent>(OnPointerDown);
        this.ScrollView.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        this.ScrollView.RegisterCallback<PointerUpEvent>(OnPointerUp);
        this.ScrollView.RegisterCallback<WheelEvent>(OnZoom);
    }

    private void OnZoom(WheelEvent evt)
    {
        float delta = evt.delta.y;
        Zoom(delta);
        evt.StopPropagation();
    }

    void Zoom(float delta)
    {
        if (delta > 0)
            zoomFactor = Mathf.Min(zoomFactor + zoomStep, maxZoom);
        else
            zoomFactor = Mathf.Max(zoomFactor - zoomStep, minZoom);

        float scaleChange = zoomFactor / zoomScale;

        foreach (var node in ScrollView.contentContainer.Children())
        {
            if (node.name == "Canvas")
                continue;

            if (node.userData == null)
            {
                node.userData = new Vector2(
                    node.resolvedStyle.width,
                    node.resolvedStyle.height
                );
            }

            Vector2 baseSize = (Vector2)node.userData;

            node.style.width = baseSize.x * zoomFactor;
            node.style.height = baseSize.y * zoomFactor;
        }

        zoomScale = zoomFactor;
    }

    public Rect GetVisibleViewportRect(ScrollView scrollView)
    {
        var offset = scrollView.scrollOffset;
        var size = scrollView.contentViewport.layout.size;
        return new Rect(offset, size);
    }

    public void AdjustZoom()
    {
        foreach (var node in ScrollView.contentContainer.Children())
        {
            if (node.name == "Canvas") continue;

            node.userData ??= new Vector2(node.style.width.value.value, node.style.height.value.value);

            Vector2 baseSize = (Vector2)node.userData;

            node.style.width = baseSize.x * (zoomScale);
            node.style.height = baseSize.y * (zoomScale);
        }
    }

    private void OnPointerDown(PointerDownEvent evt)
    {
        dragStart = evt.position;
        scrollStart = new Vector2(this.ScrollView.scrollOffset.x, this.ScrollView.scrollOffset.y);

        if (evt.button == 2)
        {
            isPanning = true;
            this.ScrollView.CapturePointer(evt.pointerId);

            if (ContextMenu != null)
            {
                ContextMenu.MenuElement.style.display = DisplayStyle.None;
            }
            evt.StopPropagation();
            return;
        }

        if (evt.button == 1 && window.ContextMenu.MenuElement.style.display == DisplayStyle.None)
        {
            if (ContextMenu == null)
            {
                OnContextMenuCreated?.Invoke();
            }
            else
            {
                ContextMenu.MenuElement.style.display = DisplayStyle.Flex;
            }

            Vector2 overlayPos = window.overlay.WorldToLocal(evt.position);

            ContextMenu.MenuElement.style.left = overlayPos.x;
            ContextMenu.MenuElement.style.top = overlayPos.y;
            return;
        }

        if (ContextMenu != null)
        {
            ContextMenu.MenuElement.style.display = DisplayStyle.None;
        }

        Vector2 mouseWorld = evt.position; 
        Vector2 mouseElementWorld = ScrollView.contentContainer.LocalToWorld(
            new Vector2(MouseElement.style.left.value.value, MouseElement.style.top.value.value)
        );

        grabOffset = mouseWorld - mouseElementWorld;
        evt.StopPropagation();
    }

    public void CreateNode()
    {
        OnCreateNode?.Invoke();
    }

    private void OnPointerMove(PointerMoveEvent evt)
    {
        if (isPanning)
        {
            Vector2 diff = (Vector2)evt.position - dragStart;
            this.ScrollView.scrollOffset = scrollStart - diff;

            evt.StopPropagation();
        }
        Vector2 mouseWorld = evt.position;
        Vector2 mouseInParent = MouseElement.parent.WorldToLocal(mouseWorld - grabOffset);

        MouseElement.style.left = mouseInParent.x;
        MouseElement.style.top = mouseInParent.y;
    }

    private void OnPointerUp(PointerUpEvent evt)
    {
        if (!isPanning) return;

        isPanning = false;
        this.ScrollView.ReleasePointer(evt.pointerId);
        evt.StopPropagation();
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
}
