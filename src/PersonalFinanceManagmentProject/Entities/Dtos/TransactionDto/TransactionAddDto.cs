namespace PersonalFinanceManagmentProject.Entities.Dtos.TransactionDto;

public class TransactionAddDto
{
    public string? Name { get; set; }
    public Status Status { get; set; }
    public int BillId { get; set; }
    public double Amount { get; set; }
    public int CategoryId { get; set; }
}