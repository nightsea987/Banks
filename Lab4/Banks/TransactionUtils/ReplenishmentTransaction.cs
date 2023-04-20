using Banks.Tools;

namespace Banks.TransactionUtils
{
    public class ReplenishmentTransaction : ITransaction
    {
        private const decimal MinValue = 0;
        public ReplenishmentTransaction(Guid accountId, Guid bankId, decimal transactionAmount, Guid? transactionId = null)
        {
            ArgumentNullException.ThrowIfNull(accountId);
            ArgumentNullException.ThrowIfNull(bankId);
            ValidateTransactionAmount(transactionAmount);
            AccountId = accountId;
            BankId = bankId;
            TransactionAmount = transactionAmount;
            if (transactionId is null)
                TransactionId = Guid.NewGuid();
            else
                TransactionId = transactionId;
        }

        public Guid AccountId { get; }
        public Guid BankId { get; }
        public Guid? TransactionId { get; }
        public decimal TransactionAmount { get; }
        public void ValidateTransactionAmount(decimal amount)
        {
            if (amount < MinValue)
                throw BanksException.ValueHasNotPassedValidation("The amount of money should be greater than zero\n");
        }
    }
}
