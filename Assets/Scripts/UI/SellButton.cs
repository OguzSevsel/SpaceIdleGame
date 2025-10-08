using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SellButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textResource;
    [SerializeField] private TextMeshProUGUI _textMoney;
    [SerializeField] private Image _iconResource;
    private Resource _resource;

    public void Initialize(Sprite icon, string resourceText, string moneyText)
    {
        _textResource.text = resourceText;
        _textMoney.text = moneyText;
        _iconResource.sprite = icon;
    }

    public void SetResource(Resource resource)
    {
        _resource = resource;
    }

    public Resource GetResource()
    {
        return _resource;
    }

    public void SetButtonTexts()
    {
        _textResource.text = $"{_resource.ResourceAmount.ToShortString()} {_resource.ResourceSO.ResourceUnit}";
        _textMoney.text = $"{(_resource.ResourceAmount * _resource.SellRate).ToShortString()} $";
    }
}
