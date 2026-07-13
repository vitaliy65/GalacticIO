using UnityEngine;

namespace currency
{
    public class CurrencyManager : MonoBehaviour
    {
        [Header("Starting balances")]
        [SerializeField] private int startingCoinBalance;
        [SerializeField] private int startingDiamondBalance;

        public int StartingCoinBalance => startingCoinBalance;
        public int StartingDiamondBalance => startingDiamondBalance;

        private CurrencyCosCoin cosCoin;
        private CurrencyCosDiamond cosDiamond;

        public static CurrencyManager Instance { get; private set; }


        public CurrencyCosCoin CosCoin => cosCoin ??= new CurrencyCosCoin(startingCoinBalance);
        public CurrencyCosDiamond CosDiamond => cosDiamond ??= new CurrencyCosDiamond(startingDiamondBalance);

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeCurrencies();
        }

        private void InitializeCurrencies()
        {
            cosCoin ??= new CurrencyCosCoin(startingCoinBalance);
            cosDiamond ??= new CurrencyCosDiamond(startingDiamondBalance);
        }

        public void AddCoins(int amount) => CosCoin.Add(amount);
        public void AddDiamonds(int amount) => CosDiamond.Add(amount);

        public bool TrySpendCoins(int amount) => CosCoin.TrySpend(amount);
        public bool TrySpendDiamonds(int amount) => CosDiamond.TrySpend(amount);

        public int GetCoinsBalance() => CosCoin.Balance;
        public int GetDiamondsBalance() => CosDiamond.Balance;

        [ContextMenu("Reset currencies")]
        public void ResetCurrencies()
        {
            cosCoin = new CurrencyCosCoin(startingCoinBalance);
            cosDiamond = new CurrencyCosDiamond(startingDiamondBalance);
        }

        [ContextMenu("Log currency balances")]
        public void LogCurrencyBalances()
        {
            Debug.Log($"Coins: {GetCoinsBalance()} | Diamonds: {GetDiamondsBalance()}");
        }
    }
}
