using Banks.Interfaces;
using Banks.Models;
using Banks.Tools;

namespace Banks.Entities
{
    public class Client : IObserver
    {
        private Client(string name, string surname, string? address, PassportNumber? passportNumber)
        {
            ClientName = name;
            ClientSurname = surname;
            ClientAddress = address;
            ClientPassportNumber = passportNumber;
        }

        public static ClientBuilder Builder => new ClientBuilder();
        public string ClientName { get; }
        public string ClientSurname { get; }
        public string? ClientAddress { get; private set; }
        public PassportNumber? ClientPassportNumber { get; private set; }
        public IObserverId? Id => ClientPassportNumber;

        public void ChangeAddress(string address)
        {
            string.IsNullOrWhiteSpace(address);
            ClientAddress = address;
        }

        public void ChangePassportNumber(PassportNumber passportNumber)
        {
            ArgumentNullException.ThrowIfNull(passportNumber);
            ClientPassportNumber = passportNumber;
        }

        public void HaveBeenNotified(ISubject subject)
        {
            return;
        }

        public class ClientBuilder
        {
            private string _name;
            private string _surname;
            private string? _address;
            private PassportNumber? _passportNumber;

            public ClientBuilder()
            {
                _name = string.Empty;
                _surname = string.Empty;
            }

            public ClientBuilder AddName(string name)
            {
                string.IsNullOrWhiteSpace(name);
                _name = name;
                return this;
            }

            public ClientBuilder AddSurname(string surname)
            {
                string.IsNullOrWhiteSpace(surname);
                _surname = surname;
                return this;
            }

            public ClientBuilder AddAddress(string address)
            {
                string.IsNullOrWhiteSpace(address);
                _address = address;
                return this;
            }

            public ClientBuilder AddPassportNumber(decimal passportNumber)
            {
                ArgumentNullException.ThrowIfNull(passportNumber);
                _passportNumber = new PassportNumber(passportNumber);
                return this;
            }

            public Client Build()
            {
                if (string.IsNullOrWhiteSpace(_name) || string.IsNullOrWhiteSpace(_surname))
                    throw ClientException.NecessaryDataIsNotSpecified("The client's first or last name is missing");
                return new Client(_name, _surname, _address, _passportNumber);
            }
        }
    }
}