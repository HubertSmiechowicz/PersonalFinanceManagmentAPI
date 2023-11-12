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

    public List<TransactionShortDto> GetTransactions()
    {
        var transactions = _dbContext.Transactions.ToList();
        return transactions.Select(t =>
        {
            var bill = _dbContext.Bills.FirstOrDefault(b => b.Id == t.BillId);
            if (bill == null)
            {
                throw new EntityNotFoundException(404, "Bill of id: " + t.BillId + " was not found!",
                    t.BillId);
            }
            return new TransactionShortDto(t.Id, $"{t.Date.Value.Day}.{t.Date.Value.Month}.{t.Date.Value.Year}", bill.Name, Math.Round(t.Amount, 2));
        }).ToList();
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
        var transactionDto = new TransactionExpandedDto(
            transactionFromDb.Name,
            $"{transactionFromDb.Date.Value.Day}.{transactionFromDb.Date.Value.Month}.{transactionFromDb.Date.Value.Year}",
            GetStatus(transactionFromDb.Status),
            bill.Name,
            transactionFromDb.Amount,
            category.Name
            );
        return transactionDto;
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
}