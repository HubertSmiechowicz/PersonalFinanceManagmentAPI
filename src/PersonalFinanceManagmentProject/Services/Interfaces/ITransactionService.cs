using PersonalFinanceManagmentProject.Entities.Dtos.TransactionDto;

namespace PersonalFinanceManagmentProject.Services.Interfaces;

public interface ITransactionService
{
    List<TransactionShortDto> GetTransactions(int pageNumber);
    void AddTransaction(TransactionAddDto transactionAddDto);
    TransactionExpandedDto GetTransactionById(int id);
    int GetTransactionMaxPageNumber();
    void DeleteTransactionById(int id);
    void UpdateTransactionName(int id, string name);
    void UpdateTransactionStatus(int id, int status);
    void UpdateTransactionBill(int id, int billId);
    void UpdateTransactionAmount(int id, double amount);
    void UpdateTransactionCategory(int id, int categoryId);
}