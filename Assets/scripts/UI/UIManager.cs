
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

    public static UIManager Instance { get; private set; }

    void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Start()
    {
        // Every other manager in the project is a singleton accessed via .Instance;
        // this fell back to Inspector-only references, so leaving either field
        // unassigned by mistake threw a NullReferenceException in Start() and broke
        // the whole HUD with no obvious cause. Fall back to the singletons instead.
        if (!buildingManager)
        {
            buildingManager = BuildingManager.Instance;
        }

        if (!currencyManager)
        {
            currencyManager = CurrencyManager.Instance;
        }

        BindToBuildingManager();
        RefreshCurrencyText();
    }

    private void OnDisable()
    {
        if (buildingManager)
        {
            buildingManager.IncomeUpdated -= RefreshCurrencyText;
        }
    }

    private void BindToBuildingManager()
    {
        if (!buildingManager)
        {
            Debug.LogWarning("UIManager has no BuildingManager reference; currency text will not auto-refresh.", this);
            return;
        }

        buildingManager.IncomeUpdated -= RefreshCurrencyText;
        buildingManager.IncomeUpdated += RefreshCurrencyText;
    }

    public void RefreshCurrencyText()
    {
        if (!currencyManager)
        {
            return;
        }

        coinsText.text = currencyManager.GetCoinsBalance().ToString();
        diamondsText.text = currencyManager.GetDiamondsBalance().ToString();
    }
}
