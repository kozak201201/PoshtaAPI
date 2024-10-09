namespace Poshta.Core.Exceptions
{
    public class UserIsAlreadyOperatorToThePostOfficeException : Exception
    {
        public UserIsAlreadyOperatorToThePostOfficeException(Guid userId, Guid postOfficeId)
        : base($"User with id: {userId} is already operator to the post office with id: {postOfficeId}. " +
              $"Try to remove operator from that post office and try again"){ }
    }
}
