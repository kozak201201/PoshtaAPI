namespace Poshta.Core.Exceptions
{
    public class NumberAddressCityAlreadyExistException : Exception
    {
        public NumberAddressCityAlreadyExistException(int number, string address, string city) :
            base($"Post office with (Number: {number}, Address: {address}, City: {city}) already exists.") { }
    }
}
