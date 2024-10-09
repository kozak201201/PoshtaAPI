using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Poshta.DataAccess.SqlServer.Entities;

namespace Poshta.DataAccess.SqlServer.Configurations
{
    public class ShipmentConfiguration : IEntityTypeConfiguration<ShipmentEntity>
    {
        public void Configure(EntityTypeBuilder<ShipmentEntity> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.TrackingNumber).IsRequired();

            builder.Property(s => s.AppraisedValue).IsRequired();

            builder.Property(s => s.Weight).IsRequired();

            builder.Property(s => s.Price).IsRequired();

            builder.Property(s => s.Status).HasConversion<int>();

            builder.HasOne(s => s.Sender)
                .WithMany()
                .HasForeignKey(s => s.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.Recipient)
                .WithMany()
                .HasForeignKey(s => s.RecipientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.Confidant)
                .WithMany()
                .HasForeignKey(s => s.ConfidantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.OperatorWhoIssued)
                .WithMany()
                .HasForeignKey(s => s.OperatorWhoIssuedId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(s => s.StartPostOffice)
                .WithMany()
                .HasForeignKey(s => s.StartPostOfficeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.EndPostOffice)
                .WithMany()
                .HasForeignKey(s => s.EndPostOfficeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.CurrentPostOffice)
                .WithMany(p => p.Shipments)
                .HasForeignKey(s => s.CurrentPostOfficeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(s => s.ShipmentHistories)
                .WithOne(sh => sh.Shipment)
                .HasForeignKey(sh => sh.ShipmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(s => s.TrackingNumber).IsUnique();
        }
    }
}
