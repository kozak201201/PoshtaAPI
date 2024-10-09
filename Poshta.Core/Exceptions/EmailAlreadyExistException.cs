namespace Poshta.Core.Exceptions
{
    public class EmailAlreadyExistException : Exception
    {
        public EmailAlreadyExistException(string email) :
            base($"User with email: {email} already exists.") { }
    }
}
