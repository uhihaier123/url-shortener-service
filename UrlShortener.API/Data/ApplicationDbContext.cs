using Microsoft.EntityFrameworkCore;
using UrlShortener.API.Models;

namespace UrlShortener.API.Data
{
    // This class manages database operations
    public class ApplicationDbContext : DbContext
    {
        // Constructor - sets up the database connection
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // This represents the Urls table in the database
        public DbSet<Url> Urls { get; set; }

        // Configure the database model
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the Urls table
            modelBuilder.Entity<Url>(entity =>
            {
                // Set the primary key
                entity.HasKey(e => e.Id);

                // Make ShortCode unique (no duplicates allowed)
                entity.HasIndex(e => e.ShortCode)
                      .IsUnique();

                // Set maximum length for OriginalUrl
                entity.Property(e => e.OriginalUrl)
                      .HasMaxLength(2048)
                      .IsRequired();

                // Set maximum length for ShortCode
                entity.Property(e => e.ShortCode)
                      .HasMaxLength(10)
                      .IsRequired();

                // Set default value for CreatedAt
                entity.Property(e => e.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");

                // Set default value for ClickCount
                entity.Property(e => e.ClickCount)
                      .HasDefaultValue(0);
            });
        }
    }
}