using TMPro;
using UnityEngine;

public class GlobalResourceManager : MonoBehaviour
{
    public static GlobalResourceManager Instance;
    public double MoneyAmount;
    [SerializeField] private TextMeshProUGUI _moneyText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ChangeMoneyText(MoneyAmount);
    }

    public void SpendMoney(double amount)
    {
        MoneyAmount -= amount;
        ChangeMoneyText(MoneyAmount);
    }

    public void AddMoney(double amount)
    {
        MoneyAmount += amount;
        ChangeMoneyText(MoneyAmount);
    }

    private void ChangeMoneyText(double moneyAmount)
    {
        _moneyText.text = $"{moneyAmount.ToShortString()} $";
    }
}
