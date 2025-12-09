using UnityEngine;
using UnityEngine.EventSystems;

public class ContextMenuCloser : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        ContextMenuManager.Instance.HideAll();
    }
}
