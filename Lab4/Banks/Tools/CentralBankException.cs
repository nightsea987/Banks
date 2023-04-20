namespace Banks.Tools
{
    public class CentralBankException : Exception
    {
        private CentralBankException(string message)
            : base(message) { }

        public static CentralBankException BankWasNotFound(string message)
        {
            return new CentralBankException(message);
        }

        public static CentralBankException BankAlreadyExists(string message)
        {
            return new CentralBankException(message);
        }
    }
}
