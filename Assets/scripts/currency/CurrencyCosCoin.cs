namespace currency
{
    public sealed class CurrencyCosCoin : Currency
    {
        public CurrencyCosCoin(int initialBalance = 0)
            : base("cos_coin", "Cos Coin", initialBalance)
        {
        }

        public override string ToString() => $"{Balance}";
    }
}
