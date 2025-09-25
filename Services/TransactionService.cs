using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker.Data;
using PersonalFinanceTracker.Models.Entities;

namespace PersonalFinanceTracker.Services;

public class TransactionService
{
    private readonly FinanceDbContext _dbContext;

    public TransactionService(FinanceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ICollection<Transaction>> GetTransactions()
    {
        return await _dbContext.Transactions
            .Include(t => t.Category)
            .ToListAsync();
    }
    
    public async Task<Transaction?> GetTransaction(int id)
    {
        return await _dbContext.Transactions
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task CreateTransaction(Transaction transaction)
    {
        await _dbContext.Transactions.AddAsync(transaction);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateTransaction(int id, Transaction transaction)
    {
        var transactionToUpdate = await GetTransaction(id);

        if (transactionToUpdate is null)
        {
            return;
        }

        transactionToUpdate.Amount = transaction.Amount;
        transactionToUpdate.CategoryId = transaction.CategoryId;
        transactionToUpdate.Date = transaction.Date;
        transactionToUpdate.Description = transaction.Description;

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteTransaction(int id)
    {
        var transactionToDelete = await GetTransaction(id);

        if (transactionToDelete is not null)
        {
            _dbContext.Transactions.Remove(transactionToDelete);
            await _dbContext.SaveChangesAsync();
        }
    }
}