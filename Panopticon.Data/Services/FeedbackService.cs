using Microsoft.EntityFrameworkCore;
using Panopticon.Data.Contexts;
using Panopticon.Shared.Models;

namespace Panopticon.Data.Services
{
    public class FeedbackService
    {
        public IDbContextFactory<PanopticonContext> _contextFactory { get; set; }
        public FeedbackService(IDbContextFactory<PanopticonContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }
        public List<Feedback> GetAllFeedback()
        {
            using(PanopticonContext context = _contextFactory.CreateDbContext())
            {
                return context.Feedback.ToList();
            }
        }

        public async Task CreateFeedback(ulong userId, string message)
        {
            Console.WriteLine($"{Environment.GetEnvironmentVariable("DATABASE")}");
            using (PanopticonContext context = _contextFactory.CreateDbContext())
            {
                context.Feedback.Add(new Feedback(userId, message));
                await context.SaveChangesAsync();
            }
        }

        public Feedback? GetFeedback(int id)
        {
            using (PanopticonContext context = _contextFactory.CreateDbContext())
            {
                return context.Feedback.FirstOrDefault(f => f.Id == id);
            }
        }

        public bool FeedbackExists(int id)
        {
            return GetFeedback(id) != null;
        }

        public void DeleteFeedback(Feedback feedback)
        {
            using (PanopticonContext context = _contextFactory.CreateDbContext())
            {
                context.Feedback.Remove(feedback);
                context.SaveChanges();
            }
        }
    }
}
