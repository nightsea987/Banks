using Banks.AccountUtils;
using Banks.Entities;
using Banks.Models;
using Banks.Services;

namespace Banks.Console
{
    public class Console
    {
        private CentralBank _centralBank;
        private Bank bank1;
        private Bank bank2;
        private Dictionary<decimal, Guid> _bankIds;

        public Console()
        {
            _centralBank = CentralBank.GetInstance();
            bank1 = new Bank(new BankConditions(new Dictionary<decimal, decimal> { { 50000, 3 }, { 100000, 4 }, { 1000000, 5 } }, 100000, 1000, 5, 20000));
            bank2 = new Bank(new BankConditions(new Dictionary<decimal, decimal> { { 40000, 2 }, { 90000, 3 }, { 900000, 4 } }, 90000, 1000, 4, 40000));
            _centralBank.RegisterNewBank(bank1);
            _centralBank.RegisterNewBank(bank2);
            _bankIds = new Dictionary<decimal, Guid> { { 1, bank1.BankId }, { 2, bank2.BankId } };
        }

        public void InitialWindow()
        {
            System.Console.WriteLine("Welcome to banks!");
            System.Console.WriteLine("To create a new client, enter 1.");
            System.Console.WriteLine("To create a new bank account, write 2.");
            System.Console.WriteLine("If you have an open bank account and you want to change your personal data, enter 3.");
            System.Console.WriteLine("If you want to top up your account, enter 4.");
            System.Console.WriteLine("If you want to withdraw money, enter 5.");
            System.Console.WriteLine("If you want to transfer money to another client's account or to another bank, write 6.");
        }

        public string InputClientName()
        {
            string? name = System.Console.ReadLine();
            while (string.IsNullOrWhiteSpace(name))
            {
                System.Console.WriteLine("Please, input your name again\n");
                name = System.Console.ReadLine();
            }

            return name;
        }

        public string InputClientSurname()
        {
            string? surname = System.Console.ReadLine();
            while (string.IsNullOrWhiteSpace(surname))
            {
                System.Console.WriteLine("Please, input your surname again\n");
                surname = System.Console.ReadLine();
            }

            return surname;
        }

        public decimal InputPassportNumber()
        {
            decimal number = decimal.Zero;
            while (!PassportNumber.ValidationCheck(number))
            {
                string? passportNumber = System.Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(passportNumber))
                {
                    number = decimal.Parse(passportNumber);
                    if (!PassportNumber.ValidationCheck(number))
                        System.Console.WriteLine("Please, input your passport number again\n");
                }
            }

            return number;
        }

        public string InputAddress()
        {
            string? address = System.Console.ReadLine();
            while (string.IsNullOrWhiteSpace(address))
                System.Console.WriteLine("Please, input your passport number again\n");
            return address;
        }

        public decimal InputCash()
        {
            string? cash = System.Console.ReadLine();
            while (string.IsNullOrWhiteSpace(cash))
            {
                System.Console.WriteLine("Please, try to input cash again\n");
                cash = System.Console.ReadLine();
            }

            return decimal.Parse(cash);
        }

        public Guid InputBankId()
        {
            string? id = System.Console.ReadLine();
            while (string.IsNullOrWhiteSpace(id))
            {
                System.Console.WriteLine("Please, try to input id again\n");
                id = System.Console.ReadLine();
            }

            return _bankIds[decimal.Parse(id)];
        }

        public double InputDaysBeforeAccountClosure()
        {
            System.Console.WriteLine("Enter the number of days you are opening an account for.\n");
            string? days = System.Console.ReadLine();
            while (string.IsNullOrWhiteSpace(days))
            {
                System.Console.WriteLine("Please, try to input days count again\n");
                days = System.Console.ReadLine();
            }

            return double.Parse(days);
        }

        public Client CreateClient()
        {
            var newClientBuilder = new Client.ClientBuilder();

            string name = InputClientName();
            newClientBuilder.AddName(name);

            string surname = InputClientSurname();
            newClientBuilder.AddSurname(surname);

            System.Console.WriteLine("Do you want to input your passport number?\n");
            if (System.Console.ReadLine() == "yes")
            {
                decimal number = InputPassportNumber();
                newClientBuilder.AddPassportNumber(number);
            }

            System.Console.WriteLine("Do you want to input your address?\n");
            if (System.Console.ReadLine() == "yes")
            {
                string address = InputAddress();
                newClientBuilder.AddAddress(address);
            }

            Client newClient = newClientBuilder.Build();
            return newClient;
        }

        public void ChangeClientData(Client client)
        {
            System.Console.WriteLine("Do you want to change your passport number?\n");
            if (System.Console.ReadLine() == "yes")
            {
                decimal number = InputPassportNumber();
                client.ChangePassportNumber(new PassportNumber(number));
            }

            System.Console.WriteLine("Do you want to change your address?\n");
            if (System.Console.ReadLine() == "yes")
            {
                string address = InputAddress();
                client.ChangeAddress(address);
            }
        }

        public Account CreateAccount(Client client)
        {
            System.Console.WriteLine("Which bank do you want to open an account with?\n 1 - bank1; \n 2 - bank2.\n");
            Bank bank = _centralBank.GetBankById(InputBankId());

            System.Console.WriteLine("How much do you want to open an account for?\n");
            decimal cashAmount = InputCash();

            System.Console.WriteLine("What type of account do you want to open?\nPlease, write one of them: Debit, Deposit, Credit\n");

            string? answer = System.Console.ReadLine();
            while (true)
            {
                if (string.IsNullOrWhiteSpace(answer))
                {
                    System.Console.WriteLine("Please, try to input type of account again\n");
                    answer = System.Console.ReadLine();
                    continue;
                }

                if (answer.ToLower() == "debit")
                {
                    return CreateDebitAccount(client, bank, cashAmount);
                }
                else if (answer.ToLower() == "deposit")
                {
                    DateTime closingAccountTime = DateTime.Now.AddDays(InputDaysBeforeAccountClosure());
                    return CreateDepositAccount(client, bank, cashAmount, closingAccountTime);
                }
                else if (answer.ToLower() == "credit")
                {
                    return CreateCreditAccount(client, bank, cashAmount);
                }
            }
        }

        public DebitAccount CreateDebitAccount(Client client, Bank bank, decimal cash)
        {
            return bank.OpenNewDebitAccount(client, cash);
        }

        public DepositAccount CreateDepositAccount(Client client, Bank bank, decimal cash, DateTime closingAccountTime)
        {
            return bank.OpenNewDepositAccount(client, closingAccountTime, cash);
        }

        public CreditAccount CreateCreditAccount(Client client, Bank bank, decimal cash)
        {
            return bank.OpenNewCreditAccount(client, cash);
        }
    }
}