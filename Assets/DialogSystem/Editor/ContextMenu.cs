using System;
using UnityEngine;
using UnityEngine.UIElements;

public class ContextMenu
{
    public VisualElement MenuElement { get; private set; }
    public event Action OnCreateConnectionButtonClicked;

    public ContextMenu()
    {
        MenuElement = CreateMenu();
    }

    private VisualElement CreateMenu()
    {
        var menu = new VisualElement();
        menu.style.width = 150;
        menu.style.height = 200;
        menu.style.backgroundColor = Color.white;
        menu.style.borderBottomLeftRadius = 6;
        menu.style.borderBottomRightRadius = 6;
        menu.style.borderTopLeftRadius = 6;
        menu.style.borderTopRightRadius = 6;
        menu.style.position = Position.Absolute;
        menu.style.flexDirection = FlexDirection.Column;

        CreateMenuItems(menu);

        return menu;
    }

    private void CreateMenuItems(VisualElement menu)
    {
        Button button = new Button();
        button.text = "Create Connection";
        button.clicked += OnCreateConnectionMenuButtonClicked;
        menu.Add(button);
    }

    private void OnCreateConnectionMenuButtonClicked()
    {
        OnCreateConnectionButtonClicked?.Invoke();
    }
}
