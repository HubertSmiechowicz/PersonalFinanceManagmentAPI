namespace PersonalFinanceManagmentProject.Entities.Dtos.TransactionDto;

public class TransactionExpandedDto
{
    public string? Name { get; set; }
    public string? Date { get; set; }
    public string? Status { get; set; }
    public string? BillName { get; set; }
    public double Amount { get; set; }
    public string? CategoryName { get; set; }

    public TransactionExpandedDto(string? name, string? date, string? status, string? billName, double amount, string? categoryName)
    {
        Name = name;
        Date = date;
        Status = status;
        BillName = billName;
        Amount = amount;
        CategoryName = categoryName;
    }
}