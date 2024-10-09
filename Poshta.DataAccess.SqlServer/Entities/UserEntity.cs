using Microsoft.AspNetCore.Identity;

namespace Poshta.DataAccess.SqlServer.Entities
{
    public class UserEntity : IdentityUser<Guid>
    {
        public string LastName { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string? MiddleName { get; set; }
    }
}
