using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Poshta.DataAccess.SqlServer.Entities;

namespace Poshta.DataAccess.SqlServer.Configurations
{
    public class ShipmentHistoryConfiguration : IEntityTypeConfiguration<ShipmentHistoryEntity>
    {
        public void Configure(EntityTypeBuilder<ShipmentHistoryEntity> builder)
        {
            builder.HasKey(sh => sh.Id);

            builder.Property(sh => sh.StatusDate).IsRequired();

            builder.Property(sh => sh.Description).IsRequired();

            builder.Property(sh => sh.Status).HasConversion<int>();

            builder.HasOne(sh => sh.Shipment)
                .WithMany(s => s.ShipmentHistories)
                .HasForeignKey(sh => sh.ShipmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(sh => sh.PostOffice)
                .WithMany()
                .HasForeignKey(sh => sh.PostOfficeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
