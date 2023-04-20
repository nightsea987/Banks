using Banks.AccountUtils;
using Banks.Tools;

namespace Banks.Entities
{
    public class ClientAccounts
    {
        private const decimal MinValue = 0;
        private Dictionary<Account, decimal> _listOfClientAccounts;

        public ClientAccounts(Dictionary<Account, decimal> listOfClientAccounts, Client bankClient)
        {
            ArgumentNullException.ThrowIfNull(listOfClientAccounts);
            ArgumentNullException.ThrowIfNull(bankClient);
            _listOfClientAccounts = listOfClientAccounts;
            BankClient = bankClient;
            AreAccountsVerified = CheckVerification();
        }

        public Client BankClient { get; }
        public IReadOnlyDictionary<Account, decimal> ListOfClientAccounts => _listOfClientAccounts;
        public bool AreAccountsVerified { get; private set; }

        public void ChargeInterestAndDeductCommission()
        {
            foreach (KeyValuePair<Account, decimal> account in _listOfClientAccounts)
                account.Key.TopUpAccount(account.Value);
        }

        public void ChangeComissionOrLoan(Account account, decimal value)
        {
            ArgumentNullException.ThrowIfNull(account);
            Account? someAccount = _listOfClientAccounts.Keys.FirstOrDefault(a => a.AccountId == account.AccountId);
            if (someAccount is null)
                throw ClientAccountsException.AccountWasNotFound("it is impossible to change the commission or loan because the account is not found\n");
            _listOfClientAccounts[someAccount] = value;
        }

        public bool CheckVerification()
        {
            return BankClient.ClientPassportNumber == null;
        }

        public Account? FindAccount(Guid accountId)
        {
            ArgumentNullException.ThrowIfNull(accountId);
            return _listOfClientAccounts.Keys.FirstOrDefault(a => a.AccountId == accountId);
        }

        public void OpenAccount(Account newAccount)
        {
            ArgumentNullException.ThrowIfNull(newAccount);
            if (FindAccount(newAccount.AccountId) != null)
                throw ClientAccountsException.AccountAlreadyExists("it is impossible to open account because it already exists\n");
            _listOfClientAccounts.Add(newAccount, MinValue);
        }

        public void CloseAccount(Account oldAccount)
        {
            ArgumentNullException.ThrowIfNull(oldAccount);
            if (FindAccount(oldAccount.AccountId) is null)
                throw ClientAccountsException.AccountWasNotFound("it is impossible to close account because the account is not found\n");
            _listOfClientAccounts.Remove(oldAccount);
        }
    }
}
