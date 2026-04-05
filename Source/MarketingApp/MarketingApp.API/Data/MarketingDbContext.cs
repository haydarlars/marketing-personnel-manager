using Microsoft.EntityFrameworkCore;
using MarketingApp.API.Models;

namespace MarketingApp.API.Data
{
    /// <summary>
    /// The main Entity Framework database context for the Marketing application.
    /// Manages the connection to SQL Server and exposes DbSets for each table.
    /// </summary>
    public class MarketingDbContext : DbContext
    {
        // Constructor receives options (connection string etc.) injected by DI
        public MarketingDbContext(DbContextOptions<MarketingDbContext> options)
            : base(options)
        {
        }

        // --- DbSets (one per database table) ---
        public DbSet<Personnel> Personnel { get; set; } = null!;
        public DbSet<Sale> Sales { get; set; } = null!;

        /// <summary>
        /// Configure table names, relationships, and constraints using Fluent API.
        /// This mirrors the SQL script so EF and the DB stay in sync.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- Personnel table ---
            modelBuilder.Entity<Personnel>(entity =>
            {
                entity.ToTable("Personnel");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Age).IsRequired();
                entity.Property(e => e.Phone).IsRequired().HasMaxLength(20);

                // DB-level check constraint: Age > 18
                entity.HasCheckConstraint("CK_Personnel_Age", "[Age] > 18");
            });

            // --- Sales table ---
            modelBuilder.Entity<Sale>(entity =>
            {
                entity.ToTable("Sales");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ReportDate).IsRequired();
                entity.Property(e => e.SalesAmount).IsRequired().HasColumnType("decimal(10,2)");

                // Foreign key: Sales → Personnel, with CASCADE DELETE
                // When a Personnel is deleted, their Sales rows are automatically removed.
                entity.HasOne(s => s.Personnel)
                      .WithMany(p => p.Sales)
                      .HasForeignKey(s => s.PersonnelId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Index for fast lookup by personnel
                entity.HasIndex(s => s.PersonnelId).HasDatabaseName("IX_Sales_PersonnelId");
            });
        }
    }
}
