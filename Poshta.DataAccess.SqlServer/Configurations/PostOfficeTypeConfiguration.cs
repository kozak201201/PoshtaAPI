using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Poshta.DataAccess.SqlServer.Entities;

namespace Poshta.DataAccess.SqlServer.Configurations
{
    public class PostOfficeTypeConfiguration : IEntityTypeConfiguration<PostOfficeTypeEntity>
    {
        public void Configure(EntityTypeBuilder<PostOfficeTypeEntity> builder)
        {
            builder.HasKey(pt => pt.Id);

            builder.Property(pt => pt.Name).IsRequired();

            builder.Property(pt => pt.MaxShipmentWeight).IsRequired();

            builder.Property(pt => pt.MaxShipmentLength).IsRequired();

            builder.Property(pt => pt.MaxShipmentWidth).IsRequired();

            builder.Property(pt => pt.MaxShipmentHeight).IsRequired();

            builder.HasIndex(pt => pt.Name).IsUnique();
        }
    }
}
