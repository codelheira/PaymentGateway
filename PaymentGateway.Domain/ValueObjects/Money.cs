namespace PaymentGateway.Domain.ValueObjects
{
    public sealed class Money
    {
        public decimal Amount { get; }
        public string Currency { get; }

        public Money(decimal amount, string currency)
        {
            if (amount < 0) throw new ArgumentException("amount must be >= 0", nameof(amount));
            if (string.IsNullOrWhiteSpace(currency)) throw new ArgumentException("currency required", nameof(currency));
            Amount = decimal.Round(amount, 2);
            Currency = currency;
        }

        public Money Subtract(decimal value) => new Money(Amount - decimal.Round(value, 2), Currency);

        public override string ToString() => $"{Amount:0.00} {Currency}";


    }

}
