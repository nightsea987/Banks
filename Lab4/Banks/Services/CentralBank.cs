using Banks.Entities;
using Banks.Tools;

namespace Banks.Services
{
    public class CentralBank
    {
        private const decimal MinValue = 0;
        private const double DaysInMonth = 30;
        private const double OneMonth = 1;
        private static CentralBank? instance;
        private List<Bank> _banksList;
        protected CentralBank()
        {
            StartDate = DateTime.Now;
            CurrentDate = DateTime.Now;
            _banksList = new List<Bank>();
        }

        public DateTime StartDate { get; }
        public DateTime CurrentDate { get; private set; }
        public IReadOnlyList<Bank> BanksList => _banksList.AsReadOnly();

        public static CentralBank GetInstance()
        {
            if (instance == null)
                instance = new CentralBank();
            return instance;
        }

        public Bank? FindBankById(Guid bankId)
        {
            ArgumentNullException.ThrowIfNull(bankId);
            return _banksList.FirstOrDefault(b => b.BankId == bankId);
        }

        public Bank GetBankById(Guid bankId)
        {
            ArgumentNullException.ThrowIfNull(bankId);
            return _banksList.First(b => b.BankId == bankId);
        }

        public void RegisterNewBank(Bank newBank)
        {
            ArgumentNullException.ThrowIfNull(newBank);
            if (FindBankById(newBank.BankId) != null)
                throw CentralBankException.BankAlreadyExists("It's impossible to register bank because it already exists\n");
            _banksList.Add(newBank);
        }

        public Bank RegisterNewBank(BankConditions conditions)
        {
            ArgumentNullException.ThrowIfNull(conditions);
            var newBank = new Bank(conditions);
            _banksList.Add(newBank);
            return newBank;
        }

        public void DisbandBank(Bank bank)
        {
            ArgumentNullException.ThrowIfNull(bank);
            if (FindBankById(bank.BankId) is null)
                throw CentralBankException.BankWasNotFound("It's impossible to disband bank because it's not found\n");
            _banksList.Remove(bank);
        }

        public void UpdateDate(decimal days)
        {
            ValidationCheck(days);
            CurrentDate = CurrentDate.AddDays((double)days);
            for (decimal i = 0; i < days; i++)
                _banksList.ForEach(b => b.UpdateAllOnOneDay());
            if (CurrentDate.Subtract(StartDate).TotalDays % DaysInMonth >= OneMonth)
                _banksList.ForEach(b => b.ChargeInterestAndDeductCommission());
        }

        public void ValidationCheck(decimal value)
        {
            if (value < MinValue)
                throw BanksException.ValueHasNotPassedValidation("Current value hasn't passed validation\n");
        }
    }
}
