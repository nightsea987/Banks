using Banks.Tools;

namespace Banks.Entities
{
    public class BankConditions
    {
        private const decimal MinValue = 0;
        private Dictionary<decimal, decimal> interestOnDepositBalance;
        public BankConditions(Dictionary<decimal, decimal> interestOnDeposit, decimal creditLimit, decimal loanComission, decimal interestOnDebitBalance, decimal maxAmountForDoubtfulAccounts)
        {
            ArgumentNullException.ThrowIfNull(interestOnDeposit);
            DictionaryValidation(interestOnDeposit);
            ValidationCheck(creditLimit);
            ValidationCheck(loanComission);
            ValidationCheck(interestOnDebitBalance);
            ValidationCheck(maxAmountForDoubtfulAccounts);
            CreditLimit = creditLimit;
            LoanComission = loanComission;
            InterestOnDebitBalance = interestOnDebitBalance;
            interestOnDepositBalance = interestOnDeposit;
            MaxAmountForDoubtfulAccounts = maxAmountForDoubtfulAccounts;
        }

        public decimal CreditLimit { get; private set; }
        public decimal LoanComission { get; private set; }
        public decimal InterestOnDebitBalance { get; private set; }
        public decimal MaxAmountForDoubtfulAccounts { get; private set; }
        public IReadOnlyDictionary<decimal, decimal> InterestOnDepositBalance => interestOnDepositBalance;

        public void ChangeCreditLimit(decimal newCreditLimit)
        {
            ValidationCheck(newCreditLimit);
            CreditLimit = newCreditLimit;
        }

        public void ChangeLoanComission(decimal newLoanComission)
        {
            ValidationCheck(newLoanComission);
            LoanComission = newLoanComission;
        }

        public void ChangeInterestOnDebitBalance(decimal newInterest)
        {
            ValidationCheck(newInterest);
            InterestOnDebitBalance = newInterest;
        }

        public void ChangeInterestOnDepositBalance(Dictionary<decimal, decimal> newInterest)
        {
            DictionaryValidation(newInterest);
            interestOnDepositBalance = newInterest;
        }

        public decimal GetInterestOnAmountForDeposit(decimal cash)
        {
            return interestOnDepositBalance[interestOnDepositBalance.Keys.First(k => cash < k)];
        }

        public void DictionaryValidation(Dictionary<decimal, decimal> interestOnBalance)
        {
            ArgumentNullException.ThrowIfNull(interestOnBalance);
            if (interestOnBalance.Any(s => !ValidateMoney(s.Value) && !ValidateMoney(s.Key)))
                throw BanksException.ValueHasNotPassedValidation("Current value hasn't passed validation\n");
        }

        public void ValidationCheck(decimal currentValue)
        {
            if (!ValidateMoney(currentValue))
                throw BanksException.ValueHasNotPassedValidation("Current value hasn't passed validation\n");
        }

        public bool ValidateMoney(decimal moneyCount)
        {
            return moneyCount >= MinValue;
        }
    }
}
