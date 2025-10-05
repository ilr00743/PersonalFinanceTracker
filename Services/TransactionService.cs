using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker.Data;
using PersonalFinanceTracker.Models.DTO;
using PersonalFinanceTracker.Models.Entities;

namespace PersonalFinanceTracker.Services;

public class TransactionService
{
    private readonly FinanceDbContext _dbContext;
    private readonly IUserService _userService;
    private readonly ILogger<TransactionService> _logger;

    public TransactionService(FinanceDbContext dbContext, IUserService userService, ILogger<TransactionService> logger)
    {
        _dbContext = dbContext;
        _userService = userService;
        _logger = logger;
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
        _logger.LogInformation("Creating transaction for user {userId}", _userService.GetCurrentUserId());
        
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
        
        _logger.LogInformation("Transaction created for user {userId}", newTransaction.UserId);
    }

    public async Task UpdateTransaction(int id, UpdateTransactionDto transactionDto)
    {
        _logger.LogInformation("Updating transaction {id} for user {userId}", id, _userService.GetCurrentUserId());
        
        var transactionToUpdate = await GetTransaction(id);

        if (transactionToUpdate is null)
        {
            _logger.LogWarning("Transaction {id} not found for user {userId}", id, _userService.GetCurrentUserId());
            return;
        }

        transactionToUpdate.Amount = transactionDto.Amount;
        transactionToUpdate.CategoryId = transactionDto.CategoryId;
        transactionToUpdate.Date = transactionDto.Date;
        transactionToUpdate.Description = transactionDto.Description;

        await _dbContext.SaveChangesAsync();
        _logger.LogInformation("Transaction {id} updated for user {userId}", id, _userService.GetCurrentUserId());
    }

    public async Task DeleteTransaction(int id)
    {
        _logger.LogInformation("Deleting transaction {id} for user {userId}", id, _userService.GetCurrentUserId());
        
        var transactionToDelete = await GetTransaction(id);

        if (transactionToDelete is not null)
        {
            _dbContext.Transactions.Remove(transactionToDelete);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Transaction {id} deleted for user {userId}", id, _userService.GetCurrentUserId());
        }
        else
        {
            _logger.LogWarning("Transaction {id} not found for user {userId}", id, _userService.GetCurrentUserId());
        }
    }

    public async Task<bool> IsTransactionExist(int id)
    {
        return await _dbContext.Transactions.AnyAsync(t => t.Id == id && t.UserId == _userService.GetCurrentUserId());
    }
}