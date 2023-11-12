namespace PersonalFinanceManagmentProject.Entities
{
    public class Transaction
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime? Date { get; set; } = DateTime.Now;
        public Status Status { get; set; }
        public int BillId { get; set; }
        public double Amount { get; set; }
        public int CategoryId { get; set; }
        public Bill? Bill { get; set; }
        public Category? Category { get; set; }
    }
}
