namespace PersonalFinanceManagmentProject.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Color { get; set; }
        public string? Icon { get; set; }
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
