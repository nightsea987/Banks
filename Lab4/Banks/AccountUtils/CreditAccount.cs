namespace Banks.AccountUtils
{
    public class CreditAccount : Account
    {
        private readonly decimal minMoneyAmount;
        public CreditAccount(decimal cash, decimal limit)
        {
            minMoneyAmount = limit;
            ValidationCheck(cash);
            Cash = cash;
            AccountId = Guid.NewGuid();
        }

        protected override bool ValidateMoney(decimal moneyCount)
        {
            return moneyCount >= minMoneyAmount;
        }
    }
}
