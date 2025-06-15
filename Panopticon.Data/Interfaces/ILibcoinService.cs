using Panopticon.Models.Libcoin;

namespace Panopticon.Data.Interfaces;

public interface ILibcoinService
{
    public void CreateLibcoinTransaction(LibcoinTransaction transaction);
    public IQueryable<LibcoinTransaction> GetAllLibcoinTransactionsForUser(string userId);
    public IQueryable<LibcoinTransaction> GetAllLibcoinTransactions();
    public IQueryable<LibcoinTransaction> GetReceivedLibcoinTransactionsForUser(string userId);
    public IQueryable<LibcoinTransaction> GetSentLibcoinTransactionsForUser(string userId);
    public double GetLibcoinBalance(string userId);
    public IQueryable<LibcoinUserBalance> GetAllLibcoinBalances();
    public void SendLibcoin(string senderId, string receiverId, double amount, string message);
    public void GrantLibcoin(string userId, double amount, string authorizingKey, string message);
    public void DeductLibcoin(string userId, double amount, string authorizingKey, string message);
}