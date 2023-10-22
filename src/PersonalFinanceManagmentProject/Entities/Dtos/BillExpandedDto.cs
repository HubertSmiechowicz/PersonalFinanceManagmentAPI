namespace PersonalFinanceManagmentProject.Entities.Dtos
{
    public class BillExpandedDto
    {
        public string? Name { get; set; }
        public double Amount { get; set; }
        public string? CreateDate { get; set; }
        public string? LastUpdateDate { get; set; }
    }
}
