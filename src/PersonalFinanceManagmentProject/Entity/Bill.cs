namespace PersonalFinanceManagmentProject.Entity
{
    public class Bill
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public double Amount { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
