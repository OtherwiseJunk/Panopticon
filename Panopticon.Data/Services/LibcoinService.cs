using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Panopticon.Data.Contexts;
using Panopticon.Data.Interfaces;
using Panopticon.Enums;
using Panopticon.Models.Core;
using Panopticon.Models.Libcoin;
using System;

namespace Panopticon.Data.Services;

public class LibcoinService(IDbContextFactory<PanopticonContext> contextFactory, ILogger<LibcoinService> logger) : ILibcoinService
{
    public void CreateLibcoinTransaction(LibcoinTransaction transaction)
    {
        using PanopticonContext context = contextFactory.CreateDbContext();
        context.LibcoinTransactions.Add(transaction);
        context.SaveChanges();
    }

    public List<LibcoinTransaction> GetAllLibcoinTransactionsForUser(string userId)
    {
        using PanopticonContext context = contextFactory.CreateDbContext();
        return context.LibcoinTransactions
            .Where(t => t.SendingUser == userId || t.ReceivingUser == userId)
            .ToList();
    }

    public List<LibcoinTransaction> GetAllLibcoinTransactions()
    {
        using PanopticonContext context = contextFactory.CreateDbContext();
        return context.LibcoinTransactions.ToList();
    }

    public List<LibcoinTransaction> GetReceivedLibcoinTransactionsForUser(string userId)
    {
        using PanopticonContext context = contextFactory.CreateDbContext();
        return context.LibcoinTransactions
            .Where(t => t.ReceivingUser == userId)
            .ToList();
    }

    public List<LibcoinTransaction> GetSentLibcoinTransactionsForUser(string userId)
    {
        using PanopticonContext context = contextFactory.CreateDbContext();
        return context.LibcoinTransactions
            .Where(t => t.SendingUser == userId)
            .ToList();
    }

    public double GetLibcoinBalance(string userId)
    {
        using PanopticonContext context = contextFactory.CreateDbContext();
        var balance = context.LibcoinUserBalances
            .FirstOrDefault(b => b.UserId == userId);
        return balance?.Balance ?? 0;
    }

    public List<LibcoinUserBalance> GetAllLibcoinBalances()
    {
        using PanopticonContext context = contextFactory.CreateDbContext();
        return context.LibcoinUserBalances.ToList();
    }

    public void SendLibcoin(string senderId, string receiverId, double amount, string? message)
    {
        using PanopticonContext context = contextFactory.CreateDbContext();
        using var transaction = context.Database.BeginTransaction();
        try
        {
            var senderBalance = context.LibcoinUserBalances.FirstOrDefault(b => b.UserId == senderId);
            var receiverBalance = context.LibcoinUserBalances.FirstOrDefault(b => b.UserId == receiverId);

            if (senderBalance == null)
            {
                throw new Exception("Sender balance not found");
            }

            if (receiverBalance == null)
            {
                receiverBalance = CreateNewLibcoinUserBalance(receiverId);
            }

            if (senderBalance.Balance < amount)
            {
                throw new Exception("Insufficient funds");
            }

            senderBalance.Balance -= amount;
            receiverBalance.Balance += amount;

            context.LibcoinUserBalances.Update(senderBalance);
            context.LibcoinUserBalances.Update(receiverBalance);

            var transactionType = LibcoinTransactionType.UserTransaction;
            ulong senderUlong = 0;
            ulong receiverUlong = 0;
            try
            {
                // API Transactions will have one sender or receiver with a GUID value.
                senderUlong = ulong.Parse(senderId);
                receiverUlong = ulong.Parse(receiverId);
            }
            catch
            {
                transactionType = LibcoinTransactionType.ApiTransaction;
            }

            CreateLibcoinTransaction(new LibcoinTransaction
            {
                SendingUser = senderId,
                ReceivingUser = receiverId,
                Amount = amount,
                TransactionMessage = message,
                TransactionType = transactionType,
                TransactionDate = DateTime.Now
            });
            context.SaveChanges();
            transaction.Commit();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing Libcoin transaction");
            transaction.Rollback();
            throw;
        }
    }
    
    private LibcoinUserBalance CreateNewLibcoinUserBalance(string userId)
    {
        var context = contextFactory.CreateDbContext();
        context.LibcoinUserBalances.Add(new LibcoinUserBalance
        {
            UserId = userId,
            Balance = 0
        });
        context.SaveChanges();
        
        return context.LibcoinUserBalances.First(b => b.UserId == userId);
    }

    public void GrantLibcoin(string userId, double amount, string authorizingKey, string message)
    {
        using PanopticonContext context = contextFactory.CreateDbContext();
        using var transaction = context.Database.BeginTransaction();
        try
        {
            var receiverBalance = context.LibcoinUserBalances.FirstOrDefault(b => b.UserId == userId);

            if (receiverBalance == null)
            {
                receiverBalance = new LibcoinUserBalance
                {
                    UserId = userId,
                    Balance = 0
                };
                context.LibcoinUserBalances.Add(receiverBalance);
                context.SaveChanges();
            }

            receiverBalance.Balance += amount;
            context.LibcoinUserBalances.Update(receiverBalance);

            CreateLibcoinTransaction(new LibcoinTransaction
            {
                SendingUser = authorizingKey,
                ReceivingUser = userId,
                Amount = amount,
                TransactionMessage = message,
                TransactionType = LibcoinTransactionType.AdminTransaction,
                TransactionDate = DateTime.Now
            });
            
            context.SaveChanges();
            transaction.Commit();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing Libcoin grant");
            transaction.Rollback();
            throw;
        }
    }

    public void DeductLibcoin(string userId, double amount, string authorizingKey, string message)
    {
        using PanopticonContext context = contextFactory.CreateDbContext();
        using var transaction = context.Database.BeginTransaction();
        try
        {
            var receiverBalance = context.LibcoinUserBalances.FirstOrDefault(b => b.UserId == userId);

            if (receiverBalance == null)
            {
                throw new Exception("Deduct target not found");
            }


            if (receiverBalance.Balance < amount)
            {
                receiverBalance.Balance = 0;
            }
            else
            {
                receiverBalance.Balance -= amount;    
            }
            
            context.LibcoinUserBalances.Update(receiverBalance);

            CreateLibcoinTransaction(new LibcoinTransaction
            {
                SendingUser = authorizingKey,
                ReceivingUser = userId,
                Amount = amount,
                TransactionMessage = message,
                TransactionType = LibcoinTransactionType.AdminTransaction,
                TransactionDate = DateTime.Now
            });
            
            context.SaveChanges();
            transaction.Commit();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing Libcoin deduct");
            transaction.Rollback();
            throw;
        }
    }
}