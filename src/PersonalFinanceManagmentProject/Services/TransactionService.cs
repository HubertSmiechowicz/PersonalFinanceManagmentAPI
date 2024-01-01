﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceManagmentProject.Entities;
using PersonalFinanceManagmentProject.Entities.Dtos.TransactionDto;
using PersonalFinanceManagmentProject.Exceptions;
using PersonalFinanceManagmentProject.Filters;
using PersonalFinanceManagmentProject.Services.Interfaces;

namespace PersonalFinanceManagmentProject.Services;

public class TransactionService : ITransactionService
{
    private readonly PersonalFinanceManagmentDbContext _dbContext;
    private readonly IMapper _mapper;
    private const int PageSize = 6;

    public TransactionService(PersonalFinanceManagmentDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    
    // get functions

    public List<TransactionShortDto> GetTransactions(int pageNumber)
    {
        var transactionsFromDb = _dbContext.Transactions.ToList();
        var numberOfPages = transactionsFromDb.Count / PageSize;
        var transactionDtos = MapToTransactionShortDto(transactionsFromDb);
        pageNumber = CheckPageNumber(pageNumber, numberOfPages);
        return transactionDtos.Skip(pageNumber * PageSize).Take(PageSize).ToList();
    }

    public List<TransactionShortDto> GetTransactionByMonth(int pageNumber, int monthNumber)
    {
        if (monthNumber is > 12 or < 1)
        {
            throw new MonthOutOfRangeException(400, "Month of number: " + monthNumber + " is out of range!");
        }

        foreach (var t in _dbContext.Transactions)
        {
            if (t.Date is null)
            {
                throw new DateNullException(404, t.Id, "Date in transaction of id: " + t.Id + " was not found!"); 
            }
        }
        var transactionsFromDb = _dbContext.Transactions.Where(t => t.Date.Value.Month == monthNumber).ToList();
        var numberOfPages = transactionsFromDb.Count / PageSize;
        var transactionDtos = MapToTransactionShortDto(transactionsFromDb);
        pageNumber = CheckPageNumber(pageNumber, numberOfPages);
        return transactionDtos.Skip(pageNumber * PageSize).Take(PageSize).ToList();
    }
    
    public TransactionExpandedDto GetTransactionById(int id)
    {
        var transactionFromDb = _dbContext.Transactions.FirstOrDefault(t => t.Id == id);
        if (transactionFromDb is null)
        {
            throw new EntityNotFoundException(404 ,"Transaction of id: " + id + " was not found!",
                id);    
        }
        var bill = _dbContext.Bills.FirstOrDefault(b => b.Id == transactionFromDb.BillId);
        var category = _dbContext.Categories.FirstOrDefault(c => c.Id == transactionFromDb.CategoryId);
        if (bill is null)
        {
            throw new EntityNotFoundException(404, "Bill of id: " + transactionFromDb.BillId + " was not found!",
                transactionFromDb.BillId);
        }
        if (category is null)
        {
            throw new EntityNotFoundException(404, "Category of id: " + transactionFromDb.CategoryId + " was not found!",
                transactionFromDb.CategoryId);
            
        }

        if (transactionFromDb.Date is null)
        {
            throw new DateNullException(404, transactionFromDb.Id, "Date in transaction of id: " + transactionFromDb.Id + " was not found!");
        }
        
        var day = FormatDay(transactionFromDb.Date.Value.Day);
        var transactionDto = new TransactionExpandedDto(
            transactionFromDb.Name,
            $"{day}.{transactionFromDb.Date.Value.Month}.{transactionFromDb.Date.Value.Year}",
            GetStatus(transactionFromDb.Status),
            bill.Name,
            Math.Round(transactionFromDb.Amount, 2),
            category.Name
        );
        return transactionDto;
    }

    public int GetTransactionMaxPageNumber()
    {
        return _dbContext.Transactions.Count() / 6;
    }

    public int GetTransactionByMonthMaxPageNumber(int monthNumber)
    {
        return _dbContext.Transactions.Where(t => t.Date.Value.Month == monthNumber).ToList().Count / 6;
    }
    
    // add functions

    public void AddTransaction(TransactionAddDto transactionAddDto)
    {
        var transaction = _mapper.Map<Transaction>(transactionAddDto);
        var category = _dbContext.Categories.FirstOrDefault(c => c.Id == transaction.CategoryId);
        if (category is null)
        {
            throw new EntityNotFoundException(404, "Category of id: " + transaction.CategoryId + " was not found!",
                transaction.CategoryId);

        }
        CalculateAmount(transaction.BillId, transaction.Status, transaction.Amount);
        _dbContext.Transactions.Add(transaction);
        _dbContext.SaveChanges();
    }
    
    // delete functions

    public void DeleteTransactionById(int id)
    {
        var transaction = _dbContext.Transactions.FirstOrDefault(t => t.Id == id);
        if (transaction is null)
        {
            throw new EntityNotFoundException(404, "Transaction of id: " + id + " was not found!");
        }
        CalculateAmount(transaction.BillId, Status.PAYMENTS, transaction.Amount);
        _dbContext.Transactions.Remove(transaction);
        _dbContext.SaveChanges();
    }
    
    // update functions

    public void UpdateTransactionName(int id, string name)
    {
        var transaction = _dbContext.Transactions.FirstOrDefault(t => t.Id == id);
        if (transaction is null)
        {
            throw new EntityNotFoundException(404, "Transaction of id: " + id + " was not found!");
        }

        transaction.Name = name;
        _dbContext.SaveChanges();
    }

    public void UpdateTransactionStatus(int id, int status)
    {
        var transaction = _dbContext.Transactions.FirstOrDefault(t => t.Id == id);
        if (transaction is null)
        {
            throw new EntityNotFoundException(404, "Transaction of id: " + id + " was not found!");
        }
        transaction.Status = (Status)status;
        CalculateAmount(transaction.BillId, (Status)status, transaction.Amount * 2);
        _dbContext.SaveChanges();
    }

    public void UpdateTransactionBill(int id, int billId)
    {
        var transaction = _dbContext.Transactions.FirstOrDefault(t => t.Id == id);
        var newBill = _dbContext.Bills.FirstOrDefault(b => b.Id == billId);
        if (transaction is null)
        {
            throw new EntityNotFoundException(404, "Transaction of id: " + id + " was not found!");
        }
        var oldBillId = transaction.BillId;
        if (newBill is null)
        {
            throw new EntityNotFoundException(404, "Bill of id: " + billId + " was not found!");
        }
        if (transaction.Status == Status.PAYMENTS)
        {
            CalculateAmount(oldBillId, Status.DEPOSITS, transaction.Amount);
        }
        else
        {
            CalculateAmount(oldBillId, Status.PAYMENTS, transaction.Amount);
        }
        CalculateAmount(newBill.Id, transaction.Status, transaction.Amount);
        transaction.BillId = billId;
        _dbContext.SaveChanges();
    }

    public void UpdateTransactionAmount(int id, double amount)
    {
        var transaction = _dbContext.Transactions.FirstOrDefault(t => t.Id == id);
        if (transaction is null)
        {
            throw new EntityNotFoundException(404, "Transaction of id: " + id + " was not found!");
        }

        if (transaction.Status == Status.PAYMENTS)
        {
            CalculateAmount(transaction.BillId, Status.DEPOSITS, transaction.Amount);
            CalculateAmount(transaction.BillId, Status.PAYMENTS, amount);
        }
        else
        {
            CalculateAmount(transaction.BillId, Status.PAYMENTS, transaction.Amount);
            CalculateAmount(transaction.BillId, Status.DEPOSITS, amount);
        }
        transaction.Amount = amount;
        _dbContext.SaveChanges();
    }

    public void UpdateTransactionCategory(int id, int categoryId)
    {
        var transaction = _dbContext.Transactions.FirstOrDefault(t => t.Id == id);
        if (transaction is null)
        {
            throw new EntityNotFoundException(404, "Transaction of id: " + id + " was not found!");
        }
        transaction.CategoryId = categoryId;
        _dbContext.SaveChanges();
    }
    
    // private functions


    private List<TransactionShortDto> MapToTransactionShortDto(List<Transaction> transactionsFromDb)
    {
        return transactionsFromDb.Select(t =>
        {
            var bill = _dbContext.Bills.FirstOrDefault(b => b.Id == t.BillId);
            if (bill is null)
            {
                throw new EntityNotFoundException(404, "Bill of id: " + t.BillId + " was not found!",
                    t.BillId);
            }

            if (t.Date is null)
            {
                throw new DateNullException(404, t.Id, "Date in transaction of id: " + t.Id + " was not found!");
            }

            var day = FormatDay(t.Date.Value.Day);
            return new TransactionShortDto(t.Id, $"{day}.{t.Date.Value.Month}.{t.Date.Value.Year}", bill.Name, Math.Round(t.Amount, 2));
        }).ToList();
    }
    
    private void CalculateAmount(int billId, Status status, double transactionAmount)
    {
        double newBillAmount;
        var bill = _dbContext.Bills.FirstOrDefault(b => b.Id == billId);

        if (bill is null)
        {
            throw new EntityNotFoundException(404, "Bill of id: " + billId + " was not found!",
                billId);
        }

        if (status is Status.PAYMENTS)
        {
            if (transactionAmount > bill.Amount)
            {
                throw new AmountOutOfRangeException(400, "Amount " + transactionAmount + " is greater than the current bill amount which is " + bill.Amount + "!");
            }
            newBillAmount = bill.Amount -= transactionAmount;
        }
        else if (status is Status.DEPOSITS)
        {
            newBillAmount = bill.Amount + transactionAmount;
        }
        else
        {
            throw new BadStatusException(400, "You gave wrong status!");
        }
        bill.Amount = newBillAmount;
        bill.LastUpdateDate = DateTime.Now;
    }

    private static string GetStatus(Status status)
    {
        if (status is Status.DEPOSITS)
        {
            return "Deposit";
        }
        else if (status is Status.PAYMENTS)
        {
            return "Payment";
        }
        else
        {
            throw new BadStatusException(400, "You gave wrong status!");
        }
    }
    
    private string FormatDay(int day)
    {
        string formatDay;
        if (day < 10)
        {
            formatDay =  '0' + day.ToString();
        }
        else
        {
            formatDay = day.ToString();
        }

        return formatDay;
    }
    
    private static int CheckPageNumber(int pageNumber, int numberOfPages)
    {
        if (pageNumber > numberOfPages)
        {
            pageNumber = numberOfPages;
        }
        else if (pageNumber < 0)
        {
            pageNumber = 0;
        }

        return pageNumber;
    }
}