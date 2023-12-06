﻿using Microsoft.AspNetCore.Mvc;
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
    public ActionResult<List<TransactionShortDto>> GetTransactions([FromQuery] int pageNumber)
    {
        return Ok(_transactionService.GetTransactions(pageNumber));
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

    [HttpGet("page")]
    public ActionResult<int> GetTransactionMaxPageNumber()
    {
        return Ok(_transactionService.GetTransactionMaxPageNumber());
    }

    [HttpDelete]
    public ActionResult DeleteTransactionById([FromQuery] int id)
    {
        _transactionService.DeleteTransactionById(id);
        return Ok();
    }
    
    [HttpPatch("name")]
    public ActionResult UpdateTransactionName(int id, string name)
    {
        _transactionService.UpdateTransactionName(id, name);
        return Ok();
    }

    [HttpPatch("status")]
    public ActionResult UpdateTransactionStatus(int id, int status)
    {
        _transactionService.UpdateTransactionStatus(id, status);
        return Ok();
    }
    
    [HttpPatch("bill")]
    public ActionResult UpdateTransactionBill(int id, int billId)
    {
        _transactionService.UpdateTransactionBill(id, billId);
        return Ok();
    }

    [HttpPatch("amount")]
    public ActionResult UpdateTransactionAmount(int id, double amount)
    {
        _transactionService.UpdateTransactionAmount(id, amount);
        return Ok();
    }

    [HttpPatch("category")]
    public ActionResult UpdateTransactionCategory(int id, int categoryId)
    {
        _transactionService.UpdateTransactionCategory(id, categoryId);
        return Ok();
    }
}