using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Poshta.DataAccess.SqlServer.Entities;

namespace Poshta.DataAccess.SqlServer.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
    {
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            builder.HasKey(u => u.Id);

            builder.HasIndex(u => u.Email).IsUnique();

            builder.HasIndex(u => u.PhoneNumber).IsUnique();

            builder.Property(u => u.LastName).IsRequired();

            builder.Property(u => u.FirstName).IsRequired();

            builder.Property(u => u.PasswordHash).IsRequired();
        }
    }
}
