using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceManagmentProject.Entities;
using PersonalFinanceManagmentProject.Entities.Dtos.TransactionDto;
using PersonalFinanceManagmentProject.Exceptions;
using PersonalFinanceManagmentProject.Filters;
using PersonalFinanceManagmentProject.Services.Interfaces;
using System.Runtime.CompilerServices;

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

    public async Task<List<TransactionShortDto>> GetTransactions(int pageNumber)
    {
        var transactionsFromDb = await _dbContext.Transactions.ToListAsync();
        var transactionDtos = MapToTransactionShortDto(transactionsFromDb);
        var numberOfPages = transactionsFromDb.Count / PageSize;
        pageNumber = CheckPageNumber(pageNumber, numberOfPages);
        return transactionDtos.Skip(pageNumber * PageSize).Take(PageSize).ToList();
    }

    public async Task<List<TransactionShortDto>> GetTransactionByMonth(int pageNumber, int monthNumber)
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
        var transactionsFromDb = await _dbContext.Transactions.Where(t => t.Date.Value.Month == monthNumber).ToListAsync();
        var transactionDtos = MapToTransactionShortDto(transactionsFromDb);
        var numberOfPages = transactionsFromDb.Count / PageSize;
        pageNumber = CheckPageNumber(pageNumber, numberOfPages);
        return transactionDtos.Skip(pageNumber * PageSize).Take(PageSize).ToList();
    }
    
    public async Task<TransactionExpandedDto> GetTransactionById(int id)
    {
        var transactionFromDb = await _dbContext.Transactions.FirstOrDefaultAsync(t => t.Id == id);
        if (transactionFromDb is null)
        {
            throw new EntityNotFoundException(404 ,"Transaction of id: " + id + " was not found!",
                id);    
        }
        var bill = await _dbContext.Bills.FirstOrDefaultAsync(b => b.Id == transactionFromDb.BillId);
        var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == transactionFromDb.CategoryId);
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

    public async Task<int> GetTransactionMaxPageNumber()
    {
        return await _dbContext.Transactions.CountAsync() / 6;
    }

    public async Task<int> GetTransactionByMonthMaxPageNumber(int monthNumber)
    {
        var transactions = await _dbContext.Transactions.ToListAsync();
        return transactions.Where(t => t.Date.Value.Month == monthNumber).ToList().Count / 6;
    }
    
    // add functions

    public async Task AddTransaction(TransactionAddDto transactionAddDto)
    {
        var transaction = _mapper.Map<Transaction>(transactionAddDto);
        var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == transaction.CategoryId);
        if (category is null)
        {
            throw new EntityNotFoundException(404, "Category of id: " + transaction.CategoryId + " was not found!",
                transaction.CategoryId);

        }
        await CalculateAmount(transaction.BillId, transaction.Status, transaction.Amount);
        await _dbContext.Transactions.AddAsync(transaction);
        await _dbContext.SaveChangesAsync();
    }
    
    // delete functions

    public async Task DeleteTransactionById(int id)
    {
        var transaction = await _dbContext.Transactions.FirstOrDefaultAsync(t => t.Id == id);
        if (transaction is null)
        {
            throw new EntityNotFoundException(404, "Transaction of id: " + id + " was not found!");
        }
        await CalculateAmount(transaction.BillId, Status.PAYMENTS, transaction.Amount);
        _dbContext.Transactions.Remove(transaction);
        await _dbContext.SaveChangesAsync();
    }
    
    // update functions

    public async Task UpdateTransactionName(int id, string name)
    {
        var transaction = await _dbContext.Transactions.FirstOrDefaultAsync(t => t.Id == id);
        if (transaction is null)
        {
            throw new EntityNotFoundException(404, "Transaction of id: " + id + " was not found!");
        }

        transaction.Name = name;
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateTransactionStatus(int id, int status)
    {
        var transaction = await _dbContext.Transactions.FirstOrDefaultAsync(t => t.Id == id);
        if (transaction is null)
        {
            throw new EntityNotFoundException(404, "Transaction of id: " + id + " was not found!");
        }
        transaction.Status = (Status)status;
        await CalculateAmount(transaction.BillId, (Status)status, transaction.Amount * 2);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateTransactionBill(int id, int billId)
    {
        var transaction = await _dbContext.Transactions.FirstOrDefaultAsync(t => t.Id == id);
        var newBill = await _dbContext.Bills.FirstOrDefaultAsync(b => b.Id == billId);
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
            await CalculateAmount(oldBillId, Status.DEPOSITS, transaction.Amount);
        }
        else
        {
            await CalculateAmount(oldBillId, Status.PAYMENTS, transaction.Amount);
        }
        await CalculateAmount(newBill.Id, transaction.Status, transaction.Amount);
        transaction.BillId = billId;
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateTransactionAmount(int id, double amount)
    {
        var transaction = await _dbContext.Transactions.FirstOrDefaultAsync(t => t.Id == id);
        if (transaction is null)
        {
            throw new EntityNotFoundException(404, "Transaction of id: " + id + " was not found!");
        }

        if (transaction.Status == Status.PAYMENTS)
        {
            await CalculateAmount(transaction.BillId, Status.DEPOSITS, transaction.Amount);
            await CalculateAmount(transaction.BillId, Status.PAYMENTS, amount);
        }
        else
        {
            await CalculateAmount(transaction.BillId, Status.PAYMENTS, transaction.Amount);
            await CalculateAmount(transaction.BillId, Status.DEPOSITS, amount);
        }
        transaction.Amount = amount;
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateTransactionCategory(int id, int categoryId)
    {
        var transaction = await _dbContext.Transactions.FirstOrDefaultAsync(t => t.Id == id);
        if (transaction is null)
        {
            throw new EntityNotFoundException(404, "Transaction of id: " + id + " was not found!");
        }
        transaction.CategoryId = categoryId;
        await _dbContext.SaveChangesAsync();
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


    private async Task CalculateAmount(int billId, Status status, double transactionAmount)
    {
        double newBillAmount;
        var bill = await _dbContext.Bills.FirstOrDefaultAsync(b => b.Id == billId);

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
            formatDay = '0' + day.ToString();
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