using PersonalFinanceManagmentProject.Entities.Dtos.TransactionDto;

namespace PersonalFinanceManagmentProject.Services.Interfaces;

public interface ITransactionService
{
    List<TransactionShortDto> GetTransactions();
    void AddTransaction(TransactionAddDto transactionAddDto);
    TransactionExpandedDto GetTransactionById(int id);
}