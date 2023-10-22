namespace PersonalFinanceManagmentProject.Entities

{
    public class Bill
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public double Amount { get; set; }
        public DateTime? CreateDate { get; set; } = DateTime.Now;
        public DateTime? LastUpdateDate { get; set; } = DateTime.Now;
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
