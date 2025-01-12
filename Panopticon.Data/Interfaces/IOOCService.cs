using Panopticon.Shared.Models;

namespace Panopticon.Data.Interfaces;

public interface IOocService
{
    public OOCItem? GetRandomOocItem();
    public OOCItem? GetOocItem(int id);
    public List<OOCItem> GetAllOocItems();
    public Task CreateOocItem(OOCItem item);
    public Task DeleteOocItem(OOCItem item);
}