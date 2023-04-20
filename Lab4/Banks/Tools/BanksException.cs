namespace Banks.Tools
{
    public class BanksException : Exception
    {
        private BanksException(string message)
            : base(message) { }

        public static BanksException ObserverWasNotFound(string message)
        {
            return new BanksException(message);
        }

        public static BanksException ValueHasNotPassedValidation(string message)
        {
            return new BanksException(message);
        }
    }
}
