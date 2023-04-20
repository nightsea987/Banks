using Banks.Tools;

namespace Banks.AccountUtils
{
    public abstract class Account
    {
        public decimal Cash { get; protected set; }
        public Guid AccountId { get; protected set; }

        public void WithdrawMoney(decimal moneyCount)
        {
            AdditionalValidate();
            ValidationCheck(moneyCount);
            ValidationCheck(Cash - moneyCount);
            Cash -= moneyCount;
        }

        public void TopUpAccount(decimal moneyCount)
        {
            ValidationCheck(moneyCount);
            Cash += moneyCount;
        }

        public void TransferMoney(decimal moneyCount)
        {
            WithdrawMoney(moneyCount);
        }

        public virtual void UpdateCurrentDate(DateTime currentTime) { }

        protected abstract bool ValidateMoney(decimal moneyCount);

        protected void ValidationCheck(decimal moneyCount)
        {
            if (!ValidateMoney(moneyCount))
                throw BanksException.ValueHasNotPassedValidation("Count of money hasn't passed validation\n");
        }

        protected virtual void AdditionalValidate() { }
    }
}
