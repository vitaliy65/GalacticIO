using System;

namespace currency
{
    [Serializable]
    public abstract class Currency
    {
        private int balance;

        protected Currency(string id, string displayName, int initialBalance = 0)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Currency id cannot be empty.", nameof(id));
            }

            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new ArgumentException("Currency display name cannot be empty.", nameof(displayName));
            }

            if (initialBalance < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(initialBalance), "Initial balance cannot be negative.");
            }

            CurrencyId = id;
            DisplayName = displayName;
            balance = initialBalance;
        }

        public string CurrencyId { get; }
        public string DisplayName { get; }
        public int Balance => balance;

        public virtual void Add(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount to add cannot be negative.");
            }

            balance += amount;
        }

        public virtual bool TrySpend(int amount)
        {
            if (CanBuy(amount))
            {
                balance -= amount;
                return true;
            }

            return false;
        }

        public virtual bool CanBuy(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount to spend cannot be negative.");
            }

            if (amount > balance)
            {
                return false;
            }

            return true;
        }

        public override string ToString() => $"{DisplayName} ({CurrencyId}): {balance}";
    }
}
