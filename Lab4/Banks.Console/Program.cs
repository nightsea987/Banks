using Banks.Entities;
using Banks.Services;

namespace Banks.Console
{
    public class Program
    {
        public static void Main()
        {
            var centralBank = CentralBank.GetInstance();
            var conditions1 = new BankConditions(new Dictionary<decimal, decimal> { { 50000, 3 }, { 100000, 4 }, { 1000000, 5 } }, 100000, 1000, 5, 20000);
            Bank bank1 = centralBank.RegisterNewBank(conditions1);
            var conditions2 = new BankConditions(new Dictionary<decimal, decimal> { { 40000, 2 }, { 90000, 3 }, { 900000, 4 } }, 90000, 1000, 4, 40000);
            Bank bank2 = centralBank.RegisterNewBank(conditions2);
            centralBank.RegisterNewBank(bank1);
            centralBank.RegisterNewBank(bank2);
        }
    }
}