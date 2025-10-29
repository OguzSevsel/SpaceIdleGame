using UnityEngine;
using UnityEngine.EventSystems;

public class DialogOptionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _indicatorObject;

    private void Awake()
    {
        if (_indicatorObject != null)
            _indicatorObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_indicatorObject != null)
            _indicatorObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_indicatorObject != null)
            _indicatorObject.SetActive(false);
    }
}
