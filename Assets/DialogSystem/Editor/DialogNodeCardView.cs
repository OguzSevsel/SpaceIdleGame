
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;

public class DialogNodeCardView
{
    private string _treePath = "Assets/DialogSystem/Editor/1-UI Documents/DialogNodeCard.uxml";
    private string _stylePath = "Assets/DialogSystem/Editor/2-Styles/DialogNodeCardStyle.uss";

    public VisualTreeAsset TreeAsset { get; private set; }
    public StyleSheet StyleSheet { get; private set; }

    public VisualElement Parent;
    public VisualElement CardBG;

    private bool isDragging;
    private Vector2 offset;

    public DialogNodeCardView(VisualElement root)
    {
        BindTreeAsset(root);
        BindUIElements();

        CardBG.RegisterCallback<PointerDownEvent>(OnPointerDown);
        CardBG.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        CardBG.RegisterCallback<PointerUpEvent>(OnPointerUp);
    }

    private void BindUIElements()
    {
        CardBG = Parent.Q<VisualElement>("CardBG");
        CardBG.style.position = Position.Absolute;
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

    void OnPointerDown(PointerDownEvent evt)
    {
        isDragging = true;
        offset = evt.localPosition;
        CardBG.CapturePointer(evt.pointerId);
    }

    void OnPointerMove(PointerMoveEvent evt)
    {
        if (!isDragging) return;

        Vector2 localMousePosition = CardBG.parent.WorldToLocal(evt.position);
        Vector2 newPos = localMousePosition - offset;
        CardBG.style.left = newPos.x;
        CardBG.style.top = newPos.y;
        CardBG.MarkDirtyRepaint();
    }

    void OnPointerUp(PointerUpEvent evt)
    {
        isDragging = false;
        CardBG.ReleasePointer(evt.pointerId);
        Parent.MarkDirtyRepaint();
    }
}
