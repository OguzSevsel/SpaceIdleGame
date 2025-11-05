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
    public VisualElement Parent { get; private set; }
    public DialogGraphEditorWindow window { get; set; }
    public ScrollView ScrollView { get; private set; }
    public VisualElement MouseElement { get; private set; }
    public VisualElement Canvas { get; private set; }
    public VisualElement SidePanel { get; private set; }
    public LineElement TempLine { get; set; }

    public ContextMenu ContextMenu { get; set; }

    private Vector2 dragStart;
    private bool isPanning;

    public float zoomFactor = 1f;
    float zoomStep = 0.1f;
    float minZoom = 0.3f;
    float maxZoom = 2f;
    private Vector2 grabOffset;
    private Label infoLabel;
    private Label infoLabel1;

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
        MouseElement.name = "MouseTracker";
        ScrollView.contentContainer.Add(MouseElement);
        Canvas.Add(MouseElement);
        zoomFactor = 1f;
    }

    private void BindUIElements()
    {
        this.ScrollView = Parent.Q<ScrollView>("ScrollView");
        this.Canvas = Parent.Q<VisualElement>("Canvas");
        this.SidePanel = Parent.Q<VisualElement>("SidePanel");

        this.ScrollView.RegisterCallback<PointerDownEvent>(OnPointerDown);
        this.ScrollView.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        this.ScrollView.RegisterCallback<PointerUpEvent>(OnPointerUp);
        this.ScrollView.RegisterCallback<WheelEvent>(OnZoom);
    }
   
    private void OnZoom(WheelEvent evt)
    {
        float delta = evt.delta.y;

        Vector2 mouseWorld = evt.localMousePosition;

        Vector2 beforeZoom = Canvas.WorldToLocal(mouseWorld);

        if (delta < 0)
            zoomFactor = Mathf.Min(zoomFactor + zoomStep, maxZoom);
        else
            zoomFactor = Mathf.Max(zoomFactor - zoomStep, minZoom);

        Canvas.style.scale = new Vector3(zoomFactor, zoomFactor, 1);

        Vector2 afterZoom = Canvas.WorldToLocal(mouseWorld);

        Vector2 diff = afterZoom - beforeZoom;

        float newLeft = Canvas.resolvedStyle.left + diff.x * zoomFactor;
        float newTop = Canvas.resolvedStyle.top + diff.y * zoomFactor;

        Canvas.style.left = newLeft;
        Canvas.style.top = newTop;

        evt.StopPropagation();
    }

    private void OnPointerDown(PointerDownEvent evt)
    {
        dragStart = evt.position;

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

            Vector2 overlayPos = ScrollView.WorldToLocal(evt.position);

            ContextMenu.MenuElement.style.left = overlayPos.x;
            ContextMenu.MenuElement.style.top = overlayPos.y;
            return;
        }

        if (ContextMenu != null)
        {
            ContextMenu.MenuElement.style.display = DisplayStyle.None;
        }

        if (window.isMakingConnection)
        {
            TempLine.Canvas.Remove(TempLine);
            window.isMakingConnection = false;
        }

        Vector2 mouseLocal = MouseElement.parent.WorldToLocal(evt.position);
        Vector2 elementPos = new Vector2(MouseElement.style.left.value.value, MouseElement.style.top.value.value);
        grabOffset = mouseLocal - elementPos;

        evt.StopPropagation();
    }

    private void OnPointerMove(PointerMoveEvent evt)
    {
        if (isPanning)
        {
            Vector2 diff = (Vector2)evt.position - dragStart;

            float newLeft = Canvas.resolvedStyle.left + diff.x;
            float newTop = Canvas.resolvedStyle.top + diff.y;

            Canvas.style.left = newLeft;
            Canvas.style.top = newTop;

            dragStart = evt.position;
            evt.StopPropagation();
        }

        Vector2 mouseWorld = evt.position;
        Vector2 mouseInParent = MouseElement.parent.WorldToLocal(mouseWorld - grabOffset);

        MouseElement.style.left = mouseInParent.x;
        MouseElement.style.top = mouseInParent.y;

        evt.StopPropagation();
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
