using Microsoft.EntityFrameworkCore;
using Panopticon.Data.Contexts;
using Panopticon.Data.Interfaces;
using Panopticon.Shared.Models;

namespace Panopticon.Data.Services
{
    public class OocService(IDbContextFactory<PanopticonContext> contextFactory) : IOocService
    {
        public IDbContextFactory<PanopticonContext> ContextFactory { get; set; } = contextFactory;

        public OOCItem? GetRandomOocItem()
        {
            using(PanopticonContext context = ContextFactory.CreateDbContext())
            {
                if(context.OutOfContextItems.Count() > 0)
                {
                    return context.OutOfContextItems.ToList().GetRandom();
                }
                return null;
            }
        }

        public OOCItem? GetOocItem(int id)
        {
            using (PanopticonContext context = ContextFactory.CreateDbContext())
            {
                return context.OutOfContextItems.FirstOrDefault(ooc => ooc.ItemID == id);
            }
        }

        public List<OOCItem> GetAllOocItems()
        {
            using (PanopticonContext context = ContextFactory.CreateDbContext())
            {
                return context.OutOfContextItems.ToList();
            }
        }

        public async Task CreateOocItem(OOCItem item)
        {
            using (PanopticonContext context = ContextFactory.CreateDbContext())
            {
                item.DateStored = DateTime.Now;
                context.OutOfContextItems.Add(item);
                _ = await context.SaveChangesAsync();
            }
        }

        public async Task DeleteOocItem(OOCItem item)
        {
            using (PanopticonContext context = ContextFactory.CreateDbContext())
            {
                context.OutOfContextItems.Remove(item);
                _ = await context.SaveChangesAsync();
            }
        }
    }
}
