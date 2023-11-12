namespace PersonalFinanceManagmentProject.Entities.Dtos.TransactionDto;

public class TransactionShortDto
{
    public int Id { get; set; }
    public string? Date { get; set; }
    public string? BillName { get; set; }
    public double Amount { get; set; }

    public TransactionShortDto(int id, string? date, string? billName, double amount)
    {
        Id = id;
        Date = date;
        BillName = billName;
        Amount = amount;
    }
}