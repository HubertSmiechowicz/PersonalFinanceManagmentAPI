using PersonalFinanceManagmentProject.Entities.Dtos.TransactionDto;
using PersonalFinanceManagmentProject.Entities;

namespace PersonalFinanceManagmentProject.Services.Interfaces;

public interface ITransactionService
{
    Task<List<TransactionShortDto>> GetTransactions(int pageNumber);
    Task<List<TransactionShortDto>> GetTransactionByMonth(int pageNumber, int month);
    Task<TransactionExpandedDto> GetTransactionById(int id);
    Task<int> GetTransactionMaxPageNumber();
    Task<int> GetTransactionByMonthMaxPageNumber(int monthNumber);
    Task AddTransaction(TransactionAddDto transactionAddDto);
    Task DeleteTransactionById(int id);
    Task UpdateTransactionName(int id, string name);
    Task UpdateTransactionStatus(int id, int status);
    Task UpdateTransactionBill(int id, int billId);
    Task UpdateTransactionAmount(int id, double amount);
    Task UpdateTransactionCategory(int id, int categoryId);
}