using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Poshta.DataAccess.SqlServer.Entities;

namespace Poshta.DataAccess.SqlServer.Configurations
{
    public class PostOfficeConfiguration : IEntityTypeConfiguration<PostOfficeEntity>
    {
        public void Configure(EntityTypeBuilder<PostOfficeEntity> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Number).IsRequired();

            builder.Property(p => p.Address).IsRequired();

            builder.Property(p => p.City).IsRequired();

            builder.Property(p => p.Latitude).IsRequired();

            builder.Property(p => p.Longitude).IsRequired();

            builder.HasOne(p => p.Type)
                .WithMany()
                .HasForeignKey(p => p.TypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(p => new { p.Number, p.Address, p.City }).IsUnique();

            builder.HasIndex(p => new { p.Latitude, p.Longitude }).IsUnique();
        }
    }
}
