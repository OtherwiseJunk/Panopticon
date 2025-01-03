using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Panopticon.Data.Contexts;
using Panopticon.Data.Interfaces;
using Panopticon.Enums;
using Panopticon.Models.Core;
using Panopticon.Models.Libcoin;
using System;

namespace Panopticon.Data.Services;

public class LibcoinService(PanopticonContext context, ILogger<LibcoinService> logger) : ILibcoinService
{
    private void CreateApiTransaction(ApiTransaction transaction)
    {
        context.ApiTransactions.Add(transaction);
        context.SaveChanges();
    }

    public void CreateLibcoinTransaction(LibcoinTransaction transaction)
    {
        context.LibcoinTransactions.Add(transaction);
        context.SaveChanges();
    }

    public List<LibcoinTransaction> GetAllLibcoinTransactionsForUser(string userId)
    {
        return context.LibcoinTransactions
            .Where(t => t.SendingUser == userId || t.ReceivingUser == userId)
            .ToList();
    }

    public List<LibcoinTransaction> GetAllLibcoinTransactions()
    {
        return context.LibcoinTransactions.ToList();
    }

    public List<LibcoinTransaction> GetReceivedLibcoinTransactionsForUser(string userId)
    {
        return context.LibcoinTransactions
            .Where(t => t.ReceivingUser == userId)
            .ToList();
    }

    public List<LibcoinTransaction> GetSentLibcoinTransactionsForUser(string userId)
    {
        return context.LibcoinTransactions
            .Where(t => t.SendingUser == userId)
            .ToList();
    }

    public double GetLibcoinBalance(string userId)
    {
        var balance = context.LibcoinUserBalances
            .FirstOrDefault(b => b.UserId == userId);
        return balance?.Balance ?? 0;
    }

    public List<LibcoinUserBalance> GetAllLibcoinBalances()
    {
        return context.LibcoinUserBalances.ToList();
    }

    public void SendLibcoin(string senderId, string receiverId, double amount, string message)
    {
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
                receiverBalance = new LibcoinUserBalance
                {
                    UserId = receiverId,
                    Balance = 0
                };
                context.LibcoinUserBalances.Add(receiverBalance);
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
                var authorizingKey = senderUlong == 0 ? senderId : receiverId;
                CreateApiTransaction(new ApiTransaction
                {
                    ApiKey = authorizingKey,
                    Type = ApiTransactionType.Libcoin,
                });
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

    public void GrantLibcoin(string userId, double amount, string authorizingKey, string message)
    {
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
            CreateApiTransaction(new ApiTransaction
            {
                ApiKey = authorizingKey,
                Type = ApiTransactionType.Libcoin,
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing Libcoin grant");
            transaction.Rollback();
            throw;
        }
    }
}