using Microsoft.EntityFrameworkCore;
using Panopticon.Shared.Models;

namespace Panopticon.Contexts
{
	public class FeedbackContext : DbContext
	{
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
#if DEBUG
            optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("TESTDATABASE"));
#else
            optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("DATABASE"));
#endif
        }
        public DbSet<Feedback> Feedback { get; set; }
	}
}
