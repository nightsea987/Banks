using Banks.Interfaces;

namespace Banks.Models
{
    public record PassportNumber : IObserverId
    {
        private const decimal _digitsCount = 10;

        public PassportNumber(decimal numberOfPassport)
        {
            if (!ValidationCheck(numberOfPassport))
                throw new Exception();
            Id = numberOfPassport;
        }

        public decimal Id { get; }

        public static bool ValidationCheck(decimal numberOfPassport)
        {
            return numberOfPassport.ToString().Length == _digitsCount;
        }
    }
}
