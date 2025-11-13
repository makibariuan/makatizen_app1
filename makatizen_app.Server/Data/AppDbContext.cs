using Microsoft.EntityFrameworkCore;
using makatizen_app.Server.Models; // Ensure this matches your models' namespace

namespace makatizen_app.Server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // --- Database Sets (Tables) ---
        public DbSet<UsersSystem> UsersSystems { get; set; }
        public DbSet<UsersKit> UsersKits { get; set; }
        public DbSet<Citizen> Citizens { get; set; }
        public DbSet<BiometricDataEnrollment> BiometricEnrollments { get; set; }
        public DbSet<BiometricDataEnrollment> BiometricDataEnrollments { get; set; }
        public DbSet<BypassLog> BypassLogs { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure composite or specific key/relationship settings here if needed.
            // For now, EF Core's conventions will handle the primary keys and foreign keys (like PersonId).

            // Configure the relationship between Citizen and BiometricDataEnrollment
            modelBuilder.Entity<BiometricDataEnrollment>()
                .HasOne(b => b.Citizen) // <-- THIS MUST MATCH THE PROPERTY NAME IN THE MODEL
                .WithMany(c => c.BiometricEnrollments)
                .HasForeignKey(b => b.PersonId);

            base.OnModelCreating(modelBuilder);
        }
    }
}