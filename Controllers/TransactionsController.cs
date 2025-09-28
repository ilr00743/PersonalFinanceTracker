using Microsoft.AspNetCore.Mvc;
using PersonalFinanceTracker.Models.DTO;
using PersonalFinanceTracker.Models.Entities;
using PersonalFinanceTracker.Services;

namespace PersonalFinanceTracker.Controllers;

[ApiController]
[Route("api/transactions")]
public class TransactionsController : ControllerBase
{
    private readonly TransactionService _transactionService;
    private readonly CategoryService _categoryService;

    public TransactionsController(TransactionService transactionService, CategoryService categoryService)
    {
        _transactionService = transactionService;
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTransactions()
    {
        var transactions = await _transactionService.GetTransactions();
        return Ok(transactions);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTransaction(int id)
    {
        var transaction = await _transactionService.GetTransaction(id);

        if (transaction is null)
        {
            return NotFound();
        }
        
        return Ok(transaction);
    }

    [HttpPost]
    public async Task<ActionResult<Transaction>> CreateTransaction(CreateTransactionDto transactionDto)
    {
        if (!await _categoryService.IsCategoryExist(transactionDto.CategoryId))
        {
            return BadRequest("Category not found");
        }

        var newTransaction = new Transaction
        {
            Amount = transactionDto.Amount,
            CategoryId = transactionDto.CategoryId,
            Description = transactionDto.Description,
            Date = transactionDto.Date,
            UserId = transactionDto.UserId
        };

        await _transactionService.CreateTransaction(newTransaction);
        
        return CreatedAtAction(nameof(GetTransaction), new { id = newTransaction.Id }, newTransaction);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTransaction(int id, UpdateTransactionDto transactionDto)
    {
        if (!await _transactionService.IsTransactionExist(id))
        {
            return NotFound("Transaction not found");
        }

        if (!await _categoryService.IsCategoryExist(transactionDto.CategoryId))
        {
            return BadRequest("Category not found");
        }

        var transactionToUpdate = new Transaction
        {
            Id = id,
            Amount = transactionDto.Amount,
            CategoryId = transactionDto.CategoryId,
            Description = transactionDto.Description,
            Date = transactionDto.Date,
            UserId = transactionDto.UserId
        };
        
        await _transactionService.UpdateTransaction(id, transactionToUpdate);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTransaction(int id)
    {
        if (!await _transactionService.IsTransactionExist(id))
        {
            return NotFound();
        }
        
        await _transactionService.DeleteTransaction(id);
        return NoContent();
    }
}