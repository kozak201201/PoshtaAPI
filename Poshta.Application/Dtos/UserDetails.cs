namespace Poshta.Application.Dtos
{
    public class UserDetails
    {
        public Guid Id { get; set; }

        public string LastName { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string? MiddleName { get; set; }

        public string? Email { get; set; }

    }
}
