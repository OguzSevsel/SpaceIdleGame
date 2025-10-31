using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ContextMenu
{
    public VisualElement MenuElement { get; private set; }
    public static event Action<NodeElement> OnCreateConnectionButtonClicked;
    public static event Action<NodeElement> OnDeleteNodeButtonClicked;



    public event Action OnCreateNodeButtonClicked;
    public NodeElement Parent { get; set; }

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
        menu.pickingMode = PickingMode.Ignore;

        return menu;
    }

    public void CreateMenuItemsForNode(VisualElement menu)
    {
        Button buttonCreateConnection = new Button();
        buttonCreateConnection.text = "Create Connection";
        buttonCreateConnection.clicked += OnCreateConnectionMenuButtonClicked;
        menu.Add(buttonCreateConnection);

        Button buttonDelete = new Button();
        buttonDelete.text = "Delete Node";
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
        Button button = new Button();
        button.text = "Create Node";
        button.clicked += OnCreateNodeMenuButtonClicked;
        menu.Add(button);
    }

    private void OnCreateNodeMenuButtonClicked()
    {
        OnCreateNodeButtonClicked?.Invoke();
    }
}
