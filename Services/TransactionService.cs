using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker.Data;
using PersonalFinanceTracker.Models.DTO;
using PersonalFinanceTracker.Models.Entities;

namespace PersonalFinanceTracker.Services;

public class TransactionService
{
    private readonly FinanceDbContext _dbContext;
    private readonly IUserService _userService;

    public TransactionService(FinanceDbContext dbContext, IUserService userService)
    {
        _dbContext = dbContext;
        _userService = userService;
    }

    public async Task<ICollection<Transaction>> GetTransactions()
    {
        return await _dbContext.Transactions
            .Where(t => t.UserId == _userService.GetCurrentUserId())
            .Include(t => t.Category)
            .ToListAsync();
    }
    
    public async Task<Transaction?> GetTransaction(int id)
    {
        return await _dbContext.Transactions
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == _userService.GetCurrentUserId());
    }

    public async Task CreateTransaction(CreateTransactionDto transactionDto)
    {
        var newTransaction = new Transaction
        {
            Amount = transactionDto.Amount,
            CategoryId = transactionDto.CategoryId,
            Description = transactionDto.Description,
            Date = transactionDto.Date,
            UserId = _userService.GetCurrentUserId()
        };
        
        await _dbContext.Transactions.AddAsync(newTransaction);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateTransaction(int id, UpdateTransactionDto transactionDto)
    {
        var transactionToUpdate = await GetTransaction(id);

        if (transactionToUpdate is null)
        {
            return;
        }

        transactionToUpdate.Amount = transactionDto.Amount;
        transactionToUpdate.CategoryId = transactionDto.CategoryId;
        transactionToUpdate.Date = transactionDto.Date;
        transactionToUpdate.Description = transactionDto.Description;

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

    public async Task<bool> IsTransactionExist(int id)
    {
        return await _dbContext.Transactions.AnyAsync(t => t.Id == id && t.UserId == _userService.GetCurrentUserId());
    }
}