using Microsoft.AspNetCore.Mvc;
using PersonalFinanceManagmentProject.Entities;
using PersonalFinanceManagmentProject.Entities.Dtos;
using PersonalFinanceManagmentProject.Services.Interfaces;

namespace PersonalFinanceManagmentProject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BillController : Controller
    {
        private readonly IBillService _billService;

        public BillController(IBillService billService)
        {
            _billService = billService;
        }

        [HttpPost]
        public ActionResult<BillExpandedDto> AddBill([FromBody] BillShortDto bill)
        {
            return Ok(_billService.AddBill(bill));
        }

        [HttpGet]
        public ActionResult<List<BillShortDto>> GetBills() 
        {
            return Ok(_billService.GetBills());
        }

        [HttpGet("{id}")]
        public ActionResult<BillExpandedDto> GetBillById([FromRoute]int id)
        {
            return Ok(_billService.GetBillById(id));
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteBill([FromRoute]int id) 
        {
            _billService.DeleteBill(id);
            return Ok();
        }

        [HttpPatch("{id}/{name}")]
        public ActionResult<BillExpandedDto> ChangeBillName([FromRoute]int id, [FromRoute]string name) 
        {
            return Ok(_billService.ChangeBillName(id, name));
        }
    }
}
