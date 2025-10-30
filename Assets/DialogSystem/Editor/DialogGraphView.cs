using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using System.Linq;

public class DialogGraphView
{
    private string _treePath = "Assets/DialogSystem/Editor/1-UI Documents/DialogGraphEditorWindow.uxml";
    private string _stylePath = "Assets/DialogSystem/Editor/2-Styles/DialogGraphEditorWindowStyle.uss";

    public VisualTreeAsset TreeAsset { get; private set; }
    public StyleSheet StyleSheet { get; private set; }

    public VisualElement Parent;
    public ScrollView ScrollView { get; private set; }
    public Button AddNodeButton { get; private set; }

    private Vector2 dragStart;
    private Vector2 scrollStart;
    private bool isPanning;

    float zoomLevel = 1f;
    float zoomFactor = 1f;
    float zoomScale = 1f;
    float zoomStep = 0.1f;
    float minZoom = 0.7f;
    float maxZoom = 1.3f;

    public DialogGraphView(VisualElement root)
    {
        BindTreeAsset(root);
        BindUIElements();   
    }

    private void BindUIElements()
    {
        this.ScrollView = Parent.Q<ScrollView>("deneme");
        this.AddNodeButton = Parent.Q<Button>("AddDialogNode");

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

    public void AdjustZoom()
    {
        foreach (var node in ScrollView.contentContainer.Children())
        {
            if (node.name == "Canvas") continue;

            node.userData ??= new Vector2(node.style.width.value.value, node.style.width.value.value);

            Vector2 baseSize = (Vector2)node.userData;

            node.style.width = baseSize.x * (zoomScale);
            node.style.height = baseSize.y * (zoomScale);
        }
    }

    private void OnPointerDown(PointerDownEvent evt)
    {
        if (evt.button == 2 || evt.button == 1)
        {
            isPanning = true;
            dragStart = evt.position;
            scrollStart = new Vector2(this.ScrollView.scrollOffset.x, this.ScrollView.scrollOffset.y);
            this.ScrollView.CapturePointer(evt.pointerId);
            evt.StopPropagation();
        }
    }

    private void OnPointerMove(PointerMoveEvent evt)
    {
        if (!isPanning) return;

        Vector2 diff = (Vector2)evt.position - dragStart;
        this.ScrollView.scrollOffset = scrollStart - diff;

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
