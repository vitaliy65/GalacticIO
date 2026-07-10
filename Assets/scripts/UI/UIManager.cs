
using builds;
using currency;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI diamondsText;
    [SerializeField] private BuildingManager buildingManager;
    [SerializeField] private CurrencyManager currencyManager;

    private void Start()
    {
        BindToBuildingManager();
        RefreshCurrencyText();
    }

    private void OnDisable()
    {
        buildingManager.IncomeUpdated -= RefreshCurrencyText;
    }

    private void BindToBuildingManager()
    {
        buildingManager.IncomeUpdated -= RefreshCurrencyText;
        buildingManager.IncomeUpdated += RefreshCurrencyText;
    }

    private void RefreshCurrencyText()
    {
        coinsText.text = currencyManager.GetCoinsBalance().ToString();
        diamondsText.text = currencyManager.GetDiamondsBalance().ToString();
    }
}
