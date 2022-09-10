using Microsoft.EntityFrameworkCore;
using Panopticon.Shared.Models;

namespace Panopticon.Data.Contexts
{
	public class PanopticonContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
#if DEBUG
            optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("TESTDATABASE"));
#else
            optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("PANOPTICONDB"));
#endif
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRecord>()
                .Property(u => u.UserId)
                .ValueGeneratedNever();
        }

        public DbSet<Feedback> Feedback { get; set; }
        public DbSet<UserRecord> UserRecords { get; set; }
        public DbSet<OOCItem> OutOfContextItems { get; set; }
    }
}
