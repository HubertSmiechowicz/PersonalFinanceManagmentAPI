using PersonalFinanceManagmentProject.Entities;
using PersonalFinanceManagmentProject.Entities.Dtos;

namespace PersonalFinanceManagmentProject.Services.Interfaces
{
    public interface IBillService
    {
        BillExpandedDto AddBill(BillShortDto bill);
        
        List<BillShortDto> GetBills();

        BillExpandedDto GetBillById(int id);

        void DeleteBill(int id);

        BillExpandedDto ChangeBillName(int id, string name);

        List<BillNameDto> GetBillNames();
    }
}