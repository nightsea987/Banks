using Banks.AccountUtils;
using Banks.Interfaces;
using Banks.Tools;
using Banks.TransactionUtils;

namespace Banks.Entities
{
    public class Bank : ISubject
    {
        public const decimal HundredPercent = 100;
        public const decimal DaysInYear = 365;
        private const decimal MinMoneyAmount = 0;
        private List<ClientAccounts> _accounts;
        private List<IObserver> _observers;
        private List<ITransaction> _transactions;
        public Bank(BankConditions conditions)
        {
            ArgumentNullException.ThrowIfNull(conditions);
            Conditions = conditions;
            BankId = Guid.NewGuid();
            _accounts = new List<ClientAccounts>();
            _observers = new List<IObserver>();
            _transactions = new List<ITransaction>();
        }

        public IReadOnlyList<ClientAccounts> Accounts => _accounts.AsReadOnly();
        public IReadOnlyList<IObserver> Observers => _observers.AsReadOnly();
        public IReadOnlyList<ITransaction> Transactions => _transactions.AsReadOnly();
        public BankConditions Conditions { get; }
        public DateTime CurrentTime { get; private set; }
        public Guid BankId { get; }

        public void MakeReplenishmentOperation(Account abstractAccount, decimal moneyAmount, Guid bankId, Guid? transactionId = null)
        {
            ArgumentNullException.ThrowIfNull(abstractAccount);
            ArgumentNullException.ThrowIfNull(transactionId);
            ValidationCheck(moneyAmount);
            IsAccountExists(abstractAccount);
            abstractAccount.TopUpAccount(moneyAmount);
            var newTransaction = new ReplenishmentTransaction(abstractAccount.AccountId, bankId, moneyAmount, transactionId);
            _transactions.Add(newTransaction);
        }

        public void MakeWithdrawalOperation(Account abstractAccount, decimal moneyAmount, Guid bankId, Guid? transactionId = null)
        {
            ArgumentNullException.ThrowIfNull(abstractAccount);
            ValidationCheck(moneyAmount);
            IsAccountExists(abstractAccount);
            ClientAccounts? clientAccounts = FindClientAccounts(abstractAccount);
            if (clientAccounts is null)
                throw ClientAccountsException.AccountWasNotFound("It's impossible to make withdrawal operation to this account because it doesn't exist\n");
            if (!clientAccounts.AreAccountsVerified && Conditions.MaxAmountForDoubtfulAccounts < moneyAmount)
                throw ClientAccountsException.UnverifiedClientTriesToWithdraOutOfLimit("It's impossible to withdraw money because the client is not verified\n");
            abstractAccount.WithdrawMoney(moneyAmount);
            var newTransaction = new WithdrawalTransaction(abstractAccount.AccountId, bankId, moneyAmount, transactionId);
            _transactions.Add(newTransaction);
        }

        public void MakeTransferOperation(Guid bankIdFrom, Guid bankIdTo, Account accountFrom, Account accountTo, decimal moneyAmount)
        {
            ArgumentNullException.ThrowIfNull(accountFrom);
            ArgumentNullException.ThrowIfNull(accountTo);
            var transactionID = Guid.NewGuid();
            MakeWithdrawalOperation(accountFrom, moneyAmount, bankIdFrom, transactionID);
            MakeReplenishmentOperation(accountTo, moneyAmount, bankIdTo, transactionID);
        }

        public void CancellationOfOperation(Guid transactionId)
        {
            ArgumentNullException.ThrowIfNull(transactionId);
            foreach (ITransaction? transaction in _transactions.Where(transaction => transaction.TransactionId == transactionId))
            {
                Account? account = FindAccountById(transaction.AccountId);
                if (account is null)
                    throw AccountException.AccountNotFound("There is no such account\n");
                if (transaction.GetType() == typeof(ReplenishmentTransaction))
                    account.WithdrawMoney(transaction.TransactionAmount);
                else if (transaction.GetType() == typeof(WithdrawalTransaction))
                    account.TopUpAccount(transaction.TransactionAmount);
                _transactions.Remove(transaction);
            }
        }

        public decimal CalculateThePercentageOnDebitAccount(Account account)
        {
            ArgumentNullException.ThrowIfNull(account);
            return account.Cash * (Conditions.InterestOnDebitBalance / DaysInYear);
        }

        public decimal CalculateThePercentageOnDepositAccount(Account account)
        {
            ArgumentNullException.ThrowIfNull(account);
            return account.Cash * (Conditions.GetInterestOnAmountForDeposit(account.Cash) / DaysInYear);
        }

        public decimal CalculateTheLoanOnCreditAccount(Account account)
        {
            ArgumentNullException.ThrowIfNull(account);
            if (account.Cash > 0)
                return 0;
            return (account.Cash - Conditions.CreditLimit) - (Conditions.LoanComission / DaysInYear);
        }

        public void UpdateDaysInDepositAccounts()
        {
            foreach (Account? depositAccount in _accounts.SelectMany(account => account.ListOfClientAccounts.Keys.Where(depositAccount => depositAccount.GetType() == typeof(DepositAccount))))
                depositAccount.UpdateCurrentDate(CurrentTime);
        }

        public void UpdateAllOnOneDay()
        {
            CurrentTime = CurrentTime.AddDays(1);
            UpdateDaysInDepositAccounts();
            foreach (ClientAccounts clientAccount in Accounts)
            {
                ArgumentNullException.ThrowIfNull(clientAccount);
                foreach (KeyValuePair<Account, decimal> account in clientAccount.ListOfClientAccounts)
                {
                    if (((object)account.Key).GetType() == typeof(CreditAccount))
                        clientAccount.ChangeComissionOrLoan(account.Key, account.Value - CalculateTheLoanOnCreditAccount(account.Key));
                    else if (((object)account.Key).GetType() == typeof(DebitAccount))
                        clientAccount.ChangeComissionOrLoan(account.Key, account.Value + CalculateThePercentageOnDebitAccount(account.Key));
                    else if (((object)account.Key).GetType() == typeof(DepositAccount))
                        clientAccount.ChangeComissionOrLoan(account.Key, account.Value + CalculateThePercentageOnDepositAccount(account.Key));
                }
            }
        }

        public void ChargeInterestAndDeductCommission()
        {
            _accounts.ForEach(account => account.ChargeInterestAndDeductCommission());
        }

        public void Attach(IObserver observer)
        {
            ArgumentNullException.ThrowIfNull(observer);
            if (FindObserver(observer.Id) != null)
                return;
            _observers.Add(observer);
        }

        public void Detach(IObserver observer)
        {
            ArgumentNullException.ThrowIfNull(observer);
            if (FindObserver(observer.Id) == null)
                throw BanksException.ObserverWasNotFound("It's imposiible to detach observer because it's not found\n");
            _observers.Remove(observer);
        }

        public void ChangeCreditLimit(decimal newCreditLimit)
        {
            Conditions.ChangeCreditLimit(newCreditLimit);
            NotifyObserversWithCreditAccounts();
        }

        public void ChangeLoanComission(decimal newLoanComission)
        {
            Conditions.ChangeLoanComission(newLoanComission);
            NotifyObserversWithCreditAccounts();
        }

        public void ChangeInterestOnDebitBalance(decimal newInterest)
        {
            Conditions.ChangeInterestOnDebitBalance(newInterest);
            NotifyObserversWithDebitAccounts();
        }

        public void ChangeInterestOnDepositBalance(Dictionary<decimal, decimal> newInterest)
        {
            Conditions.ChangeInterestOnDepositBalance(newInterest);
            NotifyObserversWithDepositAccounts();
        }

        public void NotifyAll()
        {
            _observers.ForEach(observer => observer.HaveBeenNotified(this));
        }

        public bool AreAccountsOfThisTypeInClientAccounts(ClientAccounts clientAccounts, Type typeOfAccount)
        {
            ArgumentNullException.ThrowIfNull(clientAccounts);
            ArgumentNullException.ThrowIfNull(typeOfAccount);
            return clientAccounts.ListOfClientAccounts.Keys.Any(a => a.GetType() == typeOfAccount);
        }

        public void NotifySpecifiedObservers(Type typeOfAccount)
        {
            ArgumentNullException.ThrowIfNull(typeOfAccount);
            IEnumerable<IObserver>? creditAccountsList = _accounts.SelectMany(account => _observers.Where(observer => account.BankClient.Id == observer.Id && AreAccountsOfThisTypeInClientAccounts(account, typeOfAccount)));
            foreach (IObserver observerWithCreditAccounts in creditAccountsList)
                observerWithCreditAccounts.HaveBeenNotified(this);
        }

        public void NotifyObserversWithCreditAccounts()
        {
            NotifySpecifiedObservers(typeof(CreditAccount));
        }

        public void NotifyObserversWithDebitAccounts()
        {
            NotifySpecifiedObservers(typeof(DebitAccount));
        }

        public void NotifyObserversWithDepositAccounts()
        {
            NotifySpecifiedObservers(typeof(DepositAccount));
        }

        public ClientAccounts? FindClientAccounts(Account abstractAccount)
        {
            return _accounts.FirstOrDefault(a => a.ListOfClientAccounts.Keys.Any(b => b.AccountId == abstractAccount.AccountId));
        }

        public Client? FindClientByAccount(Account account)
        {
            ArgumentNullException.ThrowIfNull(account);
            ClientAccounts? neededClientAccounts = _accounts.FirstOrDefault(c => c.ListOfClientAccounts.Keys.Any(a => a.AccountId == account.AccountId));
            if (neededClientAccounts == null)
                return null;
            return neededClientAccounts.BankClient;
        }

        public void IsAccountExists(Account account)
        {
            ArgumentNullException.ThrowIfNull(account);
            if (!_accounts.SelectMany(a => a.ListOfClientAccounts).Any(a => a.Key.AccountId == account.AccountId))
                throw AccountException.AccountNotFound("Account could not be found\n");
        }

        public IObserver? FindObserver(IObserverId? id)
        {
            ArgumentNullException.ThrowIfNull(id);
            return _observers.FirstOrDefault(observer => observer.Id != null && observer.Id.Id == id.Id);
        }

        public ClientAccounts? FindClientAccounts(Client client)
        {
            ArgumentNullException.ThrowIfNull(client);
            return _accounts.FirstOrDefault(c => c.BankClient.ClientPassportNumber == client.ClientPassportNumber);
        }

        public Account? FindAccountById(Guid accountId)
        {
            ArgumentNullException.ThrowIfNull(accountId);
            return _accounts.SelectMany(a => a.ListOfClientAccounts.Keys).FirstOrDefault(a => a.AccountId == accountId);
        }

        public void OpenNewAccount(Client client, Account newAccount)
        {
            ArgumentNullException.ThrowIfNull(client);
            ArgumentNullException.ThrowIfNull(client);
            ClientAccounts? neededClientAccounts = FindClientAccounts(client);
            if (neededClientAccounts == null)
            {
                neededClientAccounts = new ClientAccounts(new Dictionary<Account, decimal> { { newAccount, MinMoneyAmount } }, client);
                _accounts.Add(neededClientAccounts);
            }
            else
            {
                neededClientAccounts.OpenAccount(newAccount);
            }
        }

        public CreditAccount OpenNewCreditAccount(Client client, decimal cash)
        {
            ArgumentNullException.ThrowIfNull(client);
            ValidationCheck(cash);
            var newAccount = new CreditAccount(cash, Conditions.CreditLimit);
            OpenNewAccount(client, newAccount);
            return newAccount;
        }

        public DebitAccount OpenNewDebitAccount(Client client, decimal cash)
        {
            ArgumentNullException.ThrowIfNull(client);
            ValidationCheck(cash);
            var newAccount = new DebitAccount(cash);
            OpenNewAccount(client, newAccount);
            return newAccount;
        }

        public DepositAccount OpenNewDepositAccount(Client client, DateTime timeOfClosingAccount, decimal cash)
        {
            ArgumentNullException.ThrowIfNull(client);
            ArgumentNullException.ThrowIfNull(timeOfClosingAccount);
            ValidationCheck(cash);
            var newAccount = new DepositAccount(CurrentTime, timeOfClosingAccount, cash);
            OpenNewAccount(client, newAccount);
            return newAccount;
        }

        public void ValidationCheck(decimal moneyCount)
        {
            if (moneyCount < MinMoneyAmount)
                throw BanksException.ValueHasNotPassedValidation("Current value hasn't passed validation\n");
        }
    }
}
