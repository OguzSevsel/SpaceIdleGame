using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ColonyPanel : MonoBehaviour
{
    public string panelName;

    [Header("UI Elements")]
    public List<CollectorPanel> CollectorPanels;
    public List<TextMeshProUGUI> ColonyResourceTexts;
    public List<Button> CollectorAmountButtons;

    void Start()
    {
        panelName = gameObject.name;

        foreach (Button button in CollectorAmountButtons)
        {
            switch (button.name)
            {
                case "Button_X1":
                     button.onClick.AddListener(() => OnCollectorAmountButtonClicked(1));
                    break;
                case "Button_X5":
                    button.onClick.AddListener(() => OnCollectorAmountButtonClicked(5));
                    break;
                case "Button_X10":
                    button.onClick.AddListener(() => OnCollectorAmountButtonClicked(10));
                    break;
                case "Button_X100":
                    button.onClick.AddListener(() => OnCollectorAmountButtonClicked(100));
                    break;
                default:
                    break;
            }
        }
    }



    private void OnCollectorAmountButtonClicked(int value)
    {
        EventBus.Publish(new CollectorLevelAmountRequestedEvent() { amount = value });
    }
}
