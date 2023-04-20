using Banks.AccountUtils;
using Banks.Entities;
using Banks.Services;
using Banks.Tools;
using Xunit;

namespace Banks.Test
{
    public class BanksTest
    {
        private CentralBank _centralBank;
        public BanksTest()
        {
            _centralBank = CentralBank.GetInstance();
        }

        [Fact]
        public void RequiredClientDataIsNotSpecified()
        {
            Client newClient;
            Assert.Throws<ClientException>(
                () =>
                newClient = Client.Builder
                .AddAddress("Lomonosova street, 9")
                .AddPassportNumber(8888111111)
                .AddName("Alisa")
                .Build());
        }

        [Fact]
        public void UnverifiedClientTriesToWithdraOutOfLimit()
        {
            Client client = Client.Builder
                .AddPassportNumber(8888111111)
                .AddName("Alisa")
                .AddSurname("Voronina")
                .Build();
            var conditions = new BankConditions(new Dictionary<decimal, decimal> { { 50000, 3 }, { 100000, 4 }, { 1000000, 5 } }, 100000, 1000, 5, 20000);
            Bank bank = _centralBank.RegisterNewBank(conditions);
            DebitAccount account = bank.OpenNewDebitAccount(client, 100000);
            Assert.Throws<ClientAccountsException>(() => bank.MakeWithdrawalOperation(account, 30000, bank.BankId));
        }
    }
}
