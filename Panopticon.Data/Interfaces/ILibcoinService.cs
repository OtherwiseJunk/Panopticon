using Panopticon.Models.Libcoin;
using Panopticon.Data.Contexts;

namespace Panopticon.Data.Interfaces;

public interface ILibcoinService
{
    public void CreateLibcoinTransaction(LibcoinTransaction transaction);
    public IQueryable<LibcoinTransaction> GetAllLibcoinTransactionsForUser(PanopticonContext context, string userId);
    public IQueryable<LibcoinTransaction> GetAllLibcoinTransactions(PanopticonContext context);
    public IQueryable<LibcoinTransaction> GetReceivedLibcoinTransactionsForUser(PanopticonContext context, string userId);
    public IQueryable<LibcoinTransaction> GetSentLibcoinTransactionsForUser(PanopticonContext context, string userId);
    public double GetLibcoinBalance(string userId);
    public IQueryable<LibcoinUserBalance> GetAllLibcoinBalances(PanopticonContext context);
    public void SendLibcoin(string senderId, string receiverId, double amount, string message);
    public void GrantLibcoin(string userId, double amount, string authorizingKey, string message);
    public void DeductLibcoin(string userId, double amount, string authorizingKey, string message);
}