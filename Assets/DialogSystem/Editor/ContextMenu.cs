using System;
using UnityEngine.UIElements;

public class ContextMenu
{
    public NodeElement Parent { get; set; }
    public VisualElement MenuElement { get; private set; }

    public static event Action<NodeElement> OnCreateConnectionButtonClicked;
    public static event Action<NodeElement> OnDeleteNodeButtonClicked;

    public event Action OnCreateNodeButtonClicked;

    public ContextMenu(int width, int height)
    {
        MenuElement = CreateMenu(width, height);
    }

    private VisualElement CreateMenu(int width, int height)
    {
        var menu = new VisualElement();
        menu.style.width = width;
        menu.style.height = height;
        Helpers.SetBorderRadius(menu, 6);
        menu.style.alignContent = Align.FlexStart;
        menu.style.position = Position.Absolute;
        menu.style.flexDirection = FlexDirection.Column;
        return menu;
    }
    private void SetButtonUI(Button button, string buttonText)
    {
        button.text = buttonText;
        button.style.color = Helpers.HexToColor(Helpers.ColorText);
        button.style.backgroundColor = Helpers.HexToColor(Helpers.ColorBackground);
        Helpers.SetBorderColor(button, Helpers.HexToColor(Helpers.ColorBorder));
        Helpers.SetPadding(button, 1);
        Helpers.OnMouseEnter(button, "Highlight");
        Helpers.OnMouseLeave(button, "Highlight");
    }
    public void CreateMenuItemsForNode(VisualElement menu)
    {
        Button buttonCreateConnection = new();
        SetButtonUI(buttonCreateConnection, "Create Connection");
        buttonCreateConnection.clicked += OnCreateConnectionMenuButtonClicked;
        menu.Add(buttonCreateConnection);

        Button buttonDelete = new();
        SetButtonUI(buttonDelete, "Delete Node");
        buttonDelete.clicked += OnDeleteNodeMenuButtonClicked;
        menu.Add(buttonDelete);
    }

    private void OnDeleteNodeMenuButtonClicked()
    {
        OnDeleteNodeButtonClicked?.Invoke(Parent);
    }
    private void OnCreateConnectionMenuButtonClicked()
    {
        OnCreateConnectionButtonClicked?.Invoke(Parent);
    }

    public void CreateMenuItemsForScrollView(VisualElement menu)
    {
        Button button = new();
        SetButtonUI(button, "Create Node");
        button.clicked += OnCreateNodeMenuButtonClicked;
        menu.Add(button);
    }
    private void OnCreateNodeMenuButtonClicked()
    {
        OnCreateNodeButtonClicked?.Invoke();
    }
}
