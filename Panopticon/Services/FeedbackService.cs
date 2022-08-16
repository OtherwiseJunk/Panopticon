using Panopticon.Contexts;
using Panopticon.Shared.Models;

namespace Panopticon.Services
{
    public class FeedbackService
    {
        public List<Feedback> GetAllFeedback()
        {
            using(FeedbackContext context = new FeedbackContext())
            {
                return context.Feedback.ToList();
            }
        }

        public async Task CreateFeedback(ulong userId, string message)
        {
            Console.WriteLine($"{Environment.GetEnvironmentVariable("DATABASE")}");
            using (FeedbackContext context = new FeedbackContext())
            {
                context.Feedback.Add(new Feedback(userId, message));
                await context.SaveChangesAsync();
            }
        }

        public Feedback? GetFeedback(int id)
        {
            using (FeedbackContext context = new FeedbackContext())
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
            using (FeedbackContext context = new FeedbackContext())
            {
                context.Feedback.Remove(feedback);
                context.SaveChanges();
            }
        }
    }
}
