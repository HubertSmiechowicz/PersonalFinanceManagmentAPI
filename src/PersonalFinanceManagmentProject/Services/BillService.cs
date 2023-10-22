using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using PersonalFinanceManagmentProject.Entities;
using PersonalFinanceManagmentProject.Entities.Dtos;
using PersonalFinanceManagmentProject.Services.Interfaces;
using PersonalFinanceManagmentProject.Exceptions;
using Microsoft.IdentityModel.Tokens;

namespace PersonalFinanceManagmentProject.Services
{
    public class BillService : IBillService
    {
        private readonly PersonalFinanceManagmentDbContext _dbContext;
        private readonly IMapper _mapper;

        public BillService(PersonalFinanceManagmentDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public BillExpandedDto AddBill(BillShortDto billDto)
        {
            if (billDto == null) { throw new ArgumentNullException("Given bill is null!"); }
            var bill = _mapper.Map<Bill>(billDto);
            _dbContext.Add(bill);
            _dbContext.SaveChanges();
            return _mapper.Map<BillExpandedDto>(bill);
        }

        public List<BillShortDto> GetBills()
        {
            return _mapper.Map<List<BillShortDto>>(_dbContext.Bills.ToList());
        }

        public BillExpandedDto GetBillById(int id)
        {
            var bill = _dbContext.Bills.FirstOrDefault(b => b.Id == id);
            if (bill == null) { throw new EntityNotFoundException("Bill of id: " + id + " was not found!", id); }
            return _mapper.Map<BillExpandedDto>(bill);
        }

        public void DeleteBill(int id) 
        {
            var bill = _dbContext.Bills.FirstOrDefault(b => b.Id == id);
            if (bill == null) { throw new EntityNotFoundException("Bill of id: " + id + " was not found!", id); }
            _dbContext.Remove(bill);
            _dbContext.SaveChanges();
        }

        public BillExpandedDto ChangeBillName(int id, string name)
        {
            var bill = _dbContext.Bills.FirstOrDefault(b => b.Id == id);
            if (bill == null) { throw new EntityNotFoundException("Bill of id: " + id + " was not found!", id); }
            if (name.IsNullOrEmpty()) { throw new ArgumentNullException("Given name is null or empty!"); } 
            bill.Name = name;
            _dbContext.SaveChanges();
            return _mapper.Map<BillExpandedDto>(_dbContext.Bills.Where(b => b.Id == id).Single());
        }
    }
}
