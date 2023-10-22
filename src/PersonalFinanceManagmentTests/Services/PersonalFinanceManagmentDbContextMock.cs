using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using PersonalFinanceManagmentProject.Entities;

namespace PersonalFinanceManagmentTests.Services
{
    public class PersonalFinanceManagmentDbContextMock
    {
        public Mock<PersonalFinanceManagmentDbContext> DbContextMock { get; set; }

        private IQueryable<Bill> GetBillTestData()
        {
            var bills = new List<Bill>();
            for (int i = 0; i < 10; i++)
            {
                bills.Add(new Bill()
                {
                    Id = i,
                    Name = "Bill " + i.ToString(),
                    Amount = i * 1000,
                    CreateDate = DateTime.Now,
                    LastUpdateDate = DateTime.Now,
                });
            }
            return bills.AsQueryable();
        }

        public PersonalFinanceManagmentDbContextMock()
        {
            var billsData = GetBillTestData();
            var dbContextMock = new Mock<PersonalFinanceManagmentDbContext>();

            var billsMock = new Mock<DbSet<Bill>>();
            billsMock.As<IQueryable<Bill>>().Setup(m => m.Provider).Returns(billsData.Provider);
            billsMock.As<IQueryable<Bill>>().Setup(m => m.Expression).Returns(billsData.Expression);
            billsMock.As<IQueryable<Bill>>().Setup(m => m.ElementType).Returns(billsData.ElementType);
            billsMock.As<IQueryable<Bill>>().Setup(m => m.GetEnumerator()).Returns(() => billsData.GetEnumerator());

            dbContextMock.Setup(c => c.Bills).Returns(billsMock.Object);

            DbContextMock = dbContextMock;
        }
    }
}
