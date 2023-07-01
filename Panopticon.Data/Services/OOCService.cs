using Microsoft.EntityFrameworkCore;
using Panopticon.Data.Contexts;
using Panopticon.Shared.Models;

namespace Panopticon.Data.Services
{
    public class OOCService
    {
        public IDbContextFactory<PanopticonContext> _contextFactory { get; set; }
        public OOCService(IDbContextFactory<PanopticonContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }
        public OOCItem? GetRandomOOCItem()
        {
            using(PanopticonContext context = _contextFactory.CreateDbContext())
            {
                if(context.OutOfContextItems.Count() > 0)
                {
                    return context.OutOfContextItems.ToList().GetRandom();
                }
                return null;
            }
        }

        public OOCItem? GetOOCItem(int id)
        {
            using (PanopticonContext context = _contextFactory.CreateDbContext())
            {
                return context.OutOfContextItems.FirstOrDefault(ooc => ooc.ItemID == id);
            }
        }

        public List<OOCItem> GetAllOOCItems()
        {
            using (PanopticonContext context = _contextFactory.CreateDbContext())
            {
                return context.OutOfContextItems.ToList();
            }
        }

        public async Task CreateOOCitem(OOCItem item)
        {
            using (PanopticonContext context = _contextFactory.CreateDbContext())
            {
                item.DateStored = DateTime.Now;
                context.OutOfContextItems.Add(item);
                _ = await context.SaveChangesAsync();
            }
        }

        public async void DeleteOOCItem(OOCItem item)
        {
            using (PanopticonContext context = _contextFactory.CreateDbContext())
            {
                context.OutOfContextItems.Remove(item);
                _ = await context.SaveChangesAsync();
            }
        }
    }
}
