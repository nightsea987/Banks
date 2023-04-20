namespace Banks.AccountUtils
{
    public class DebitAccount : Account
    {
        private const decimal MinMoneyAmount = 0;

        public DebitAccount(decimal cash)
        {
            ValidationCheck(cash);
            Cash = cash;
            AccountId = Guid.NewGuid();
        }

        protected override bool ValidateMoney(decimal moneyCount)
        {
            return moneyCount >= MinMoneyAmount;
        }
    }
}
