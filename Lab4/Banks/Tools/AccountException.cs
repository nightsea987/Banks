namespace Banks.Tools
{
    public class AccountException : Exception
    {
        private AccountException(string message)
            : base(message) { }

        public static AccountException AccountClosingDateHasNotArrived(string message)
        {
            return new AccountException(message);
        }

        public static AccountException InvalidAccountClosingDate(string message)
        {
            return new AccountException(message);
        }

        public static AccountException AccountNotFound(string message)
        {
            return new AccountException(message);
        }
    }
}
