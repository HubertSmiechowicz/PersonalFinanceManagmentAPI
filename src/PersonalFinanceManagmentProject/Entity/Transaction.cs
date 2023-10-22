namespace PersonalFinanceManagmentProject.Entity
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public DateTime? Date { get; set; }
        public Status Status { get; set; }
        public Guid BillId { get; set; }
        public double Amount { get; set; }
        public Guid CategoryId { get; set; }
        public Bill? Bill { get; set; }
        public Category? Category { get; set; }
    }
}
