using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ContextMenuManager : MonoBehaviour
{
    public static ContextMenuManager Instance;

    [SerializeField] private GameObject menuPrefab;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Canvas canvas;
    private List<GameObject> openSubmenus = new();

    private GameObject currentMenu;

    void Awake()
    {
        Instance = this;
    }

    public void Show(Vector2 screenPosition, List<ContextMenuOption> options)
    {
        HideAll();

        currentMenu = Instantiate(menuPrefab, canvas.transform);

        RectTransform rt = currentMenu.GetComponent<RectTransform>();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
           canvas.transform as RectTransform,
           screenPosition,
           canvas.worldCamera,
           out Vector2 localPoint
        );

        rt.anchoredPosition = localPoint + new Vector2(rt.rect.width / 2, -(rt.rect.height / 2));

        foreach (var option in options)
        {
            var btnObj = Instantiate(buttonPrefab, rt);
            var text = btnObj.GetComponentInChildren<TMP_Text>();
            text.text = option.Label;

            var arrow = btnObj.transform.Find("Arrow");
            if (arrow != null) arrow.gameObject.SetActive(option.HasSubmenu);

            btnObj.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (option.HasSubmenu)
                {
                    OpenSubmenu(btnObj.GetComponent<RectTransform>(), option.SubOptions);
                }
                else
                {
                    option.Callback?.Invoke();
                    HideAll();
                }
            });
        }
    }


    private void OpenSubmenu(RectTransform buttonRect, List<ContextMenuOption> suboptions)
    {
        // Create submenu next to button
        GameObject submenu = Instantiate(menuPrefab, canvas.transform);

        RectTransform rt = submenu.GetComponent<RectTransform>();

        // Position submenu to the right of button
        Vector3 pos = buttonRect.position;
        pos.x += buttonRect.rect.width;
        rt.position = pos;

        // Clear previous submenus
        CloseSubmenus();

        // Track submenu
        openSubmenus.Add(submenu);

        // Populate submenu
        foreach (var option in suboptions)
        {
            var btnObj = Instantiate(buttonPrefab, rt);
            var text = btnObj.GetComponentInChildren<TMP_Text>();
            text.text = option.Label;

            var arrow = btnObj.transform.Find("Arrow");
            if (arrow != null) arrow.gameObject.SetActive(option.HasSubmenu);

            btnObj.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (!option.HasSubmenu)
                {
                    option.Callback?.Invoke();
                    HideAll();
                }
                else
                {
                    // open nested submenu
                    OpenSubmenu(btnObj.GetComponent<RectTransform>(), option.SubOptions);
                }
            });
        }
    }


    public void HideAll()
    {
        if (currentMenu != null)
            Destroy(currentMenu);

        CloseSubmenus();
    }

    private void CloseSubmenus()
    {
        foreach (var sm in openSubmenus)
            Destroy(sm);

        openSubmenus.Clear();
    }

}
