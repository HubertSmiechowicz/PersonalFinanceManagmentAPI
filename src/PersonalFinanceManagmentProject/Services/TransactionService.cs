using AutoMapper;
using PersonalFinanceManagmentProject.Entities;
using PersonalFinanceManagmentProject.Entities.Dtos.TransactionDto;
using PersonalFinanceManagmentProject.Exceptions;
using PersonalFinanceManagmentProject.Services.Interfaces;

namespace PersonalFinanceManagmentProject.Services;

public class TransactionService : ITransactionService
{
    private readonly PersonalFinanceManagmentDbContext _dbContext;
    private readonly IMapper _mapper;

    public TransactionService(PersonalFinanceManagmentDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public List<TransactionShortDto> GetTransactions(int pageNumber)
    {
        int pageSize = 6;
        var transactions = _dbContext.Transactions.ToList();
        int numberOfPages = transactions.Count / pageSize;
        var transactionDtos =  transactions.Select(t =>
        {
            var bill = _dbContext.Bills.FirstOrDefault(b => b.Id == t.BillId);
            if (bill == null)
            {
                throw new EntityNotFoundException(404, "Bill of id: " + t.BillId + " was not found!",
                    t.BillId);
            }
            
            return new TransactionShortDto(t.Id, $"{t.Date.Value.Day}.{t.Date.Value.Month}.{t.Date.Value.Year}", bill.Name, Math.Round(t.Amount, 2));
        }).ToList();
        pageNumber = CheckPageNumber(pageNumber, numberOfPages);
        return transactionDtos.Skip(pageNumber * pageSize).Take(pageSize).ToList();
    }

    public void AddTransaction(TransactionAddDto transactionAddDto)
    {
        var transaction = _mapper.Map<Transaction>(transactionAddDto);
        var bill = _dbContext.Bills.FirstOrDefault(b => b.Id == transaction.BillId);
        var category = _dbContext.Categories.FirstOrDefault(c => c.Id == transaction.CategoryId);
        if (bill == null)
        {
            throw new EntityNotFoundException(404, "Bill of id: " + transaction.BillId + " was not found!",
                transaction.BillId);
            
        }
        if (category == null)
        {
            throw new EntityNotFoundException(404, "Category of id: " + transaction.CategoryId + " was not found!",
                transaction.CategoryId);
            
        }
        _dbContext.Transactions.Add(transaction);
        var newBillAmount = CalculateAmount(bill, transaction.Status, transaction.Amount);
        bill.Amount = newBillAmount;
        bill.LastUpdateDate = DateTime.Now;
        _dbContext.SaveChanges();
    }
    
    public TransactionExpandedDto GetTransactionById(int id)
    {
        var transactionFromDb = _dbContext.Transactions.FirstOrDefault(t => t.Id == id);
        if (transactionFromDb == null)
        {
            throw new EntityNotFoundException(404 ,"Transaction of id: " + id + " was not found!",
                id);    
        }
        var bill = _dbContext.Bills.FirstOrDefault(b => b.Id == transactionFromDb.BillId);
        var category = _dbContext.Categories.FirstOrDefault(c => c.Id == transactionFromDb.CategoryId);
        if (bill == null)
        {
            throw new EntityNotFoundException(404, "Bill of id: " + transactionFromDb.BillId + " was not found!",
                transactionFromDb.BillId);
        }
        if (category == null)
        {
            throw new EntityNotFoundException(404, "Category of id: " + transactionFromDb.CategoryId + " was not found!",
                transactionFromDb.CategoryId);
            
        }
        var day = FormatDay(transactionFromDb);
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

    public void DeleteTransactionById(int id)
    {
        var transaction = _dbContext.Transactions.FirstOrDefault(t => t.Id == id);
        if (transaction == null)
        {
            throw new EntityNotFoundException(404, "Transaction of id: " + id + " was not found!");
        }

        _dbContext.Transactions.Remove(transaction);
        _dbContext.SaveChanges();
    }

    public void UpdateTransactionName(int id, string name)
    {
        var transaction = _dbContext.Transactions.FirstOrDefault(t => t.Id == id);
        if (transaction == null)
        {
            throw new EntityNotFoundException(404, "Transaction of id: " + id + " was not found!");
        }

        transaction.Name = name;
        _dbContext.SaveChanges();
    }

    public void UpdateTransactionStatus(int id, int status)
    {
        var transaction = _dbContext.Transactions.FirstOrDefault(t => t.Id == id);
        if (transaction == null)
        {
            throw new EntityNotFoundException(404, "Transaction of id: " + id + " was not found!");
        }
        transaction.Status = (Status)status;
        _dbContext.SaveChanges();
    }

    public void UpdateTransactionBill(int id, int billId)
    {
        var transaction = _dbContext.Transactions.FirstOrDefault(t => t.Id == id);
        if (transaction == null)
        {
            throw new EntityNotFoundException(404, "Transaction of id: " + id + " was not found!");
        }
        transaction.BillId = billId;
        _dbContext.SaveChanges();
    }

    public void UpdateTransactionAmount(int id, double amount)
    {
        var transaction = _dbContext.Transactions.FirstOrDefault(t => t.Id == id);
        if (transaction == null)
        {
            throw new EntityNotFoundException(404, "Transaction of id: " + id + " was not found!");
        }
        transaction.Amount = amount;
        _dbContext.SaveChanges();
    }

    public void UpdateTransactionCategory(int id, int categoryId)
    {
        var transaction = _dbContext.Transactions.FirstOrDefault(t => t.Id == id);
        if (transaction == null)
        {
            throw new EntityNotFoundException(404, "Transaction of id: " + id + " was not found!");
        }
        transaction.CategoryId = categoryId;
        _dbContext.SaveChanges();
    }
    
    private static double CalculateAmount(Bill bill, Status status, double transactionAmount)
    {
        if (status == Status.PAYMENTS)
        {
            return bill.Amount - transactionAmount;
        }
        else if (status == Status.DEPOSITS)
        {
            return bill.Amount + transactionAmount;
        }
        else
        {
            throw new BadStatusException(400, "You gave wrong status!");
        }
    }

    private static string GetStatus(Status status)
    {
        if (status == Status.DEPOSITS)
        {
            return "Deposit";
        }
        else if (status == Status.PAYMENTS)
        {
            return "Payment";
        }
        else
        {
            throw new BadStatusException(400, "You gave wrong status!");
        }
    }
    
    private static string FormatDay(Transaction transaction)
    {
        if (transaction.Date.Value.Day < 10)
        {
            return '0' + transaction.Date.Value.Day.ToString();
        }
        return transaction.Date.Value.Day.ToString();
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