using Microsoft.EntityFrameworkCore;
using Panopticon.Data.Contexts;
using Panopticon.Shared.Models;

namespace Panopticon.Data.Services
{
    public class UserRecordService
    {
        public IDbContextFactory<PanopticonContext> _contextFactory { get; set; }
        public UserRecordService(IDbContextFactory<PanopticonContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }
        public UserRecord? GetUserRecord(ulong userId)
        {
            using (PanopticonContext context = _contextFactory.CreateDbContext())
            {
                return context.UserRecords.FirstOrDefault(u => u.UserId == userId);
            }
        }

        public void UpdateOrCreateUserRecords(UserRecord userRecord)
        {
            using(PanopticonContext context = _contextFactory.CreateDbContext())
            {
                UserRecord? existingRecord = context.UserRecords.FirstOrDefault(u => u.UserId == userRecord.UserId);
                if (existingRecord == null)
                {
                    CreateUserRecord(userRecord);
                }
                else
                {
                    context.UserRecords.Remove(existingRecord);
                    context.UserRecords.Add(userRecord);
                    context.SaveChanges();
                }
            }
        }

        private void CreateUserRecord(UserRecord userRecord)
        {
            using (PanopticonContext context = _contextFactory.CreateDbContext())
            {
                context.UserRecords.Add(userRecord);
                context.SaveChanges();
            }
        }
    }
}
