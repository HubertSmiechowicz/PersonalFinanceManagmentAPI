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
    public ActionResult<List<TransactionShortDto>> GetTransactions()
    {
        return Ok(_transactionService.GetTransactions());
    }

    [HttpPost]
    public ActionResult AddTransaction([FromBody] TransactionAddDto transactionAddDto)
    {
        _transactionService.AddTransaction(transactionAddDto);
        return Ok();
    }

    [HttpGet("single")]
    public ActionResult<TransactionExpandedDto> GetTransactionById([FromQuery] int id)
    {
        return Ok(_transactionService.GetTransactionById(id));
    }
}