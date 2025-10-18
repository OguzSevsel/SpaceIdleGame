using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour
{
    private List<TabButtonn> TabButtons;
    [SerializeField] private Sprite tabIdle;
    [SerializeField] private Sprite tabHover;
    [SerializeField] private Sprite tabSelected;
    private TabButtonn selectedTab;
    public List<GameObject> objectsToSwap;

    public static event Action OnSellResourceHide;
    public static event Action OnSellResourceShow;
    public static event Action OnUpgradeInfoShow;
    public static event Action OnUpgradeInfoHide;

    public void Subscribe(TabButtonn button)
    {
        if (TabButtons == null)
        {
            TabButtons = new List<TabButtonn>();
        }

        TabButtons.Add(button);
    }

    public void OnTabEnter(TabButtonn button)
    {
        ResetTabs();
        if (selectedTab == null || button != selectedTab)
        {
            button.backgroundImage.sprite = tabHover;
        }
    }

    public void OnTabExit(TabButtonn button)
    {
        ResetTabs();
    }

    public void OnTabSelected(TabButtonn button)
    {
        selectedTab = button;
        ResetTabs();
        button.backgroundImage.sprite = tabSelected;

        if (button.gameObject.name == "Button_Convert")
        {
            OnSellResourceShow?.Invoke();
        }
        else if (button.gameObject.name == "Button_Upgrade")
        {
            OnUpgradeInfoShow?.Invoke();
        }
        else
        {
            OnSellResourceHide?.Invoke();
            OnUpgradeInfoHide?.Invoke();
        }

        int index = button.transform.GetSiblingIndex();

        for (int i = 0; i < objectsToSwap.Count; i++)
        {
            if (i == index)
            {
                objectsToSwap[i].SetActive(true);
            }
            else
            {
                objectsToSwap[i].SetActive(false);
            }
        }
    }

    public void ResetTabs()
    {
        foreach (var button in TabButtons)
        {
            if (selectedTab != null && button == selectedTab)
            {
                continue;
            }
            button.backgroundImage.sprite = tabIdle;
        }
    }
}
