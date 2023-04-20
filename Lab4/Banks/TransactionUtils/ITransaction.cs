namespace Banks.TransactionUtils
{
    public interface ITransaction
    {
        Guid? TransactionId { get; }
        Guid AccountId { get; }
        decimal TransactionAmount { get; }
        void ValidateTransactionAmount(decimal amount);
    }
}
