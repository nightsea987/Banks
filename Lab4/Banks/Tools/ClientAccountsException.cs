namespace Banks.Tools
{
    public class ClientAccountsException : Exception
    {
        private ClientAccountsException(string message)
            : base(message) { }

        public static ClientAccountsException AccountWasNotFound(string message)
        {
            return new ClientAccountsException(message);
        }

        public static ClientAccountsException AccountAlreadyExists(string message)
        {
            return new ClientAccountsException(message);
        }

        public static ClientAccountsException UnverifiedClientTriesToWithdraOutOfLimit(string message)
        {
            return new ClientAccountsException(message);
        }
    }
}
