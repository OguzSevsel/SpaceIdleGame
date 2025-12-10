using UnityEngine;
using UnityEngine.EventSystems;

public class ContextMenuCloser : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            ContextMenuManager.Instance.HideAll();
        }
    }
}
