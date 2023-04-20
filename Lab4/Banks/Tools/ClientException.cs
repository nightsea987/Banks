namespace Banks.Tools
{
    public class ClientException : Exception
    {
        private ClientException(string message)
            : base(message) { }

        public static ClientException NecessaryDataIsNotSpecified(string message)
        {
            return new ClientException(message);
        }
    }
}
