using Banks.Tools;

namespace Banks.AccountUtils
{
    public class DepositAccount : Account
    {
        private const decimal MinMoneyAmount = 0;
        public DepositAccount(DateTime currentTime, DateTime timeOfClosingAccount, decimal cash)
        {
            ArgumentNullException.ThrowIfNull(currentTime);
            ArgumentNullException.ThrowIfNull(timeOfClosingAccount);
            ValidationCheck(cash);
            if (currentTime > timeOfClosingAccount)
                throw AccountException.InvalidAccountClosingDate("Current time can't be greater than timeOfClosingAccount\n");
            AccountId = Guid.NewGuid();
            Cash = cash;
            CurrentTime = currentTime;
            AccountClosingDate = timeOfClosingAccount;
        }

        public DateTime CurrentTime { get; private set; }
        public DateTime AccountClosingDate { get; }

        public override void UpdateCurrentDate(DateTime currentTime)
        {
            ArgumentNullException.ThrowIfNull(currentTime);
            CurrentTime = currentTime;
        }

        protected override bool ValidateMoney(decimal moneyCount)
        {
            return moneyCount >= MinMoneyAmount;
        }

        protected override void AdditionalValidate()
        {
            if (CurrentTime < AccountClosingDate)
                throw AccountException.AccountClosingDateHasNotArrived("The current time is less than the closing date of the account\n");
        }
    }
}
