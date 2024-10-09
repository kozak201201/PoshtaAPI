using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Poshta.DataAccess.SqlServer.Entities;

namespace Poshta.DataAccess.SqlServer
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<UserEntity, IdentityRole<Guid>, Guid>(options)
    {
        public DbSet<ShipmentHistoryEntity> ShipmentHistories { get; set; }

        public DbSet<ShipmentEntity> Shipments { get; set; }

        public DbSet<PostOfficeTypeEntity> PostOfficeTypes { get; set; }

        public DbSet<PostOfficeEntity> PostOffices { get; set; }

        public DbSet<OperatorRatingEntity> OperatorRatings { get; set; }

        public DbSet<OperatorEntity> Operators { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            SeedRoles(modelBuilder);
        }

        private void SeedRoles(ModelBuilder modelBuilder)
        {
            var roles = new[]
            {
                new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = "Operator", NormalizedName = "OPERATOR" }
            };

            modelBuilder.Entity<IdentityRole<Guid>>().HasData(roles);
        }
    }
}
