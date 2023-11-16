using PersonalFinanceManagmentProject.Entities.Dtos.TransactionDto;

namespace PersonalFinanceManagmentProject.Services.Interfaces;

public interface ITransactionService
{
    List<TransactionShortDto> GetTransactions(int pageNumber);
    void AddTransaction(TransactionAddDto transactionAddDto);
    TransactionExpandedDto GetTransactionById(int id);
    int GetTransactionMaxPageNumber();
    void DeleteTransactionById(int id);

}