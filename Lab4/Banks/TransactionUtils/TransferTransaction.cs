using Banks.Tools;

namespace Banks.TransactionUtils
{
    public class TransferTransaction
    {
        private const decimal MinValue = 0;
        public TransferTransaction(Guid accountIdFrom, Guid bankIdFrom, Guid accountIdTo, Guid bankIdTo, decimal transactionAmount)
        {
            ArgumentNullException.ThrowIfNull(accountIdFrom);
            ArgumentNullException.ThrowIfNull(bankIdFrom);
            ArgumentNullException.ThrowIfNull(accountIdTo);
            ArgumentNullException.ThrowIfNull(bankIdTo);
            ValidateTransactionAmount(transactionAmount);
            AccountIdFrom = accountIdFrom;
            BankIdFrom = bankIdFrom;
            AccountIdTo = accountIdTo;
            BankIdTo = bankIdTo;
            TransactionAmount = transactionAmount;
        }

        public Guid AccountIdFrom { get; }
        public Guid BankIdFrom { get; }
        public Guid AccountIdTo { get; }
        public Guid BankIdTo { get; }
        public decimal TransactionAmount { get; }
        public void ValidateTransactionAmount(decimal amount)
        {
            if (amount < MinValue)
                throw BanksException.ValueHasNotPassedValidation("The amount of money should be greater than zero\n");
        }
    }
}