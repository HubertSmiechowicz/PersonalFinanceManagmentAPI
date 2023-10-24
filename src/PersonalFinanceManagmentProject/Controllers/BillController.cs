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

        [HttpGet("single")]
        public ActionResult<BillExpandedDto> GetBillById([FromQuery]int id)
        {
            if (id == null) { return  BadRequest(); }
            return Ok(_billService.GetBillById(id));
        }

        [HttpDelete]
        public ActionResult DeleteBill([FromQuery]int id) 
        {
            _billService.DeleteBill(id);
            return NoContent();
        }

        [HttpPatch]
        public ActionResult<BillExpandedDto> ChangeBillName([FromQuery] int id, [FromQuery] string name) 
        {
            return Ok(_billService.ChangeBillName(id, name));
        }
    }
}
