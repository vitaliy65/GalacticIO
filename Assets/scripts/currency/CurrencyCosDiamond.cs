namespace currency
{
    public sealed class CurrencyCosDiamond : Currency
    {
        public CurrencyCosDiamond(int initialBalance = 0)
            : base("cos_diamond", "Cos Diamond", initialBalance)
        {
        }

        public override string ToString() => $"{Balance}";
    }
}
