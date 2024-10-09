namespace Poshta.Core.Exceptions
{
    public class PhoneAlreadyExistException : Exception
    {
        public PhoneAlreadyExistException(string phone) :
            base($"User with phone: {phone} already exists.") { }
    }
}
