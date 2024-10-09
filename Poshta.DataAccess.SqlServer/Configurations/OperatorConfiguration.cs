using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Poshta.DataAccess.SqlServer.Entities;

namespace Poshta.DataAccess.SqlServer.Configurations
{
    public class OperatorConfiguration : IEntityTypeConfiguration<OperatorEntity>
    {
        public void Configure(EntityTypeBuilder<OperatorEntity> builder)
        {
            builder.HasKey(o => o.Id);

            builder.HasOne(o => o.User)
                .WithOne()
                .HasForeignKey<OperatorEntity>(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(o => o.PostOffice)
               .WithMany(p => p.Operators)
               .HasForeignKey(o => o.PostOfficeId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(o => o.UserId).IsUnique();
        }
    }
}
