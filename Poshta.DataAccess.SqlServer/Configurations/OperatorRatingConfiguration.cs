using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Poshta.DataAccess.SqlServer.Entities;

namespace Poshta.DataAccess.SqlServer.Configurations
{
    public class OperatorRatingConfiguration : IEntityTypeConfiguration<OperatorRatingEntity>
    {
        public void Configure(EntityTypeBuilder<OperatorRatingEntity> builder)
        {
            builder.HasKey(or => or.Id);

            builder.Property(or => or.Rating).IsRequired();

            builder.Property(or => or.Review);

            builder.Property(or => or.CreatedAt).IsRequired();

            builder.HasOne(or => or.Operator)
                .WithMany(o => o.Ratings)
                .HasForeignKey(or => or.OperatorId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
