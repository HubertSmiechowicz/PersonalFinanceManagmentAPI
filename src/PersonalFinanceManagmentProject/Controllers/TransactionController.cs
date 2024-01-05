using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceManagmentProject.Entities;
using PersonalFinanceManagmentProject.Entities.Dtos.TransactionDto;
using PersonalFinanceManagmentProject.Services.Interfaces;

namespace PersonalFinanceManagmentProject.Controllers;

[ApiController]
[Route("[controller]")]
public class TransactionController : Controller
{
    private readonly ITransactionService _transactionService;

    public TransactionController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpGet]
    public async Task<ActionResult<List<TransactionShortDto>>> GetTransactions([FromQuery] int pageNumber)
    {
        return Ok(await _transactionService.GetTransactions(pageNumber));
    }

    [HttpGet("month")]
    public async Task<ActionResult<List<TransactionShortDto>>> GetTransactionByMonth([FromQuery] int pageNumber,
        [FromQuery] int monthNumber)
    {
        return Ok(await _transactionService.GetTransactionByMonth(pageNumber, monthNumber));
    }
    
    [HttpGet("single")]
    public async Task<ActionResult<TransactionExpandedDto>> GetTransactionById([FromQuery] int id)
    {
        return Ok(await _transactionService.GetTransactionById(id));
    }

    [HttpGet("page")]
    public async Task<ActionResult<int>> GetTransactionMaxPageNumber()
    {
        return Ok(await _transactionService.GetTransactionMaxPageNumber());
    }

    [HttpGet("page/month")]
    public async Task<ActionResult<int>> GetTransactionByMonthMaxPageNumber(int monthNumber)
    {
        return Ok(await _transactionService.GetTransactionByMonthMaxPageNumber(monthNumber));
    }

    [HttpPost]
    public async Task<ActionResult> AddTransaction([FromBody] TransactionAddDto transactionAddDto)
    {
        await _transactionService.AddTransaction(transactionAddDto);
        return Ok();
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteTransactionById([FromQuery] int id)
    {
        await _transactionService.DeleteTransactionById(id);
        return Ok();
    }
    
    [HttpPatch("name")]
    public async Task<ActionResult> UpdateTransactionName(int id, string name)
    {
        await _transactionService.UpdateTransactionName(id, name);
        return Ok();
    }

    [HttpPatch("status")]
    public async Task<ActionResult> UpdateTransactionStatus(int id, int status)
    {
        await _transactionService.UpdateTransactionStatus(id, status);
        return Ok();
    }
    
    [HttpPatch("bill")]
    public async Task<ActionResult> UpdateTransactionBill(int id, int bill)
    {
        await _transactionService.UpdateTransactionBill(id, bill);
        return Ok();
    }

    [HttpPatch("amount")]
    public async Task<ActionResult> UpdateTransactionAmount(int id, double amount)
    {
        await _transactionService.UpdateTransactionAmount(id, amount);
        return Ok();
    }

    [HttpPatch("category")]
    public async Task<ActionResult> UpdateTransactionCategory(int id, int category)
    {
        await _transactionService.UpdateTransactionCategory(id, category);
        return Ok();
    }
}