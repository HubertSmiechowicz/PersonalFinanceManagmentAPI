using AutoMapper;
using PersonalFinanceManagmentProject.Entities;
using PersonalFinanceManagmentProject.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using PersonalFinanceManagmentProject.Entities.Dtos;
using PersonalFinanceManagmentProject.Exceptions;

namespace PersonalFinanceManagmentTests.Services
{
    public class BillServiceTests
    {
        private Mock<PersonalFinanceManagmentDbContext> dbContextMock = new PersonalFinanceManagmentDbContextMock().DbContextMock;

        private IMapper getMapper()
        {
            var mappingProfile = new FinanceMapperProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(mappingProfile));
            return new Mapper(configuration);
        }

        [Fact]
        public void AddBill_ForGivenBillDto_ReturnsBillWithCorrectCreateDate()
        {
            //arrange
            var today = DateTime.Now;
            var billService = new BillService(dbContextMock.Object, getMapper());
            var correctCreateDate = today.Day.ToString() + "." + today.Month.ToString() + "." + today.Year.ToString();

            //act
            var createDate = billService.AddBill(
                new BillShortDto
                {
                    Name = "Test",
                    Amount = 1000,
                }).CreateDate;

            //assert
            Assert.Equal(correctCreateDate, createDate);
        }

        [Fact]
        public void AddBill_ForGivenBillDto_ReturnsBillWithCorrectLastUpdateDate()
        {
            //arrange
            var today = DateTime.Now;
            var billService = new BillService(dbContextMock.Object, getMapper());
            var correctLastUpdateDate = today.Day.ToString() + "." + today.Month.ToString() + "." + today.Year.ToString();

            //act
            var lastUpdateDate = billService.AddBill(
                new BillShortDto
                {
                    Name = "Test",
                    Amount = 1000,
                }).LastUpdateDate;

            //assert
            Assert.Equal(correctLastUpdateDate, lastUpdateDate);
        }

        [Fact]
        public void AddBill_ForGivenNullBill_ThrowArgumentNullException()
        {
            //arrange
            var billService = new BillService(dbContextMock.Object, getMapper());

            //assert
            Assert.Throws<ArgumentNullException>(() => billService.AddBill(null));
        }

        [Fact]
        public void GetBills_ForGivenData_ReturnListOfCorrectLenght()
        {
            //arrange 
            var billService = new BillService(dbContextMock.Object, getMapper());
            
            //act
            var listLenght = billService.GetBills().LongCount();

            //assert
            Assert.True(listLenght == 10);
        }

        [Theory]
        [InlineData(0, "Bill 0")]
        [InlineData(1, "Bill 1")]
        [InlineData(2, "Bill 2")]
        [InlineData(3, "Bill 3")]
        [InlineData(4, "Bill 4")]
        public void GetBillById_ForGivenId_ReturnBillWithCorrectName(int id, string correctName)
        {
            //arrange
            var billService = new BillService(dbContextMock.Object, getMapper());

            //act
            var name = billService.GetBillById(id).Name;

            //assert
            Assert.Equal(correctName, name);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 1000)]
        [InlineData(2, 2000)]
        [InlineData(3, 3000)]
        [InlineData(4, 4000)]
        public void GetBillById_ForGivenId_ReturnBillWithCorrectAmount(int id, double correctAmount)
        {
            //arrange
            var billService = new BillService(dbContextMock.Object, getMapper());

            //act
            var amount = billService.GetBillById(id).Amount;

            //assert
            Assert.Equal(correctAmount, amount);
        }

        [Theory]
        [InlineData(500)]
        [InlineData(1000)]
        [InlineData(2000)]
        [InlineData(3000)]
        [InlineData(4000)]
        public void GetBillById_ForGivenIdOutOfRange_ThrowsEntityNotFoundException(int id)
        {
            //arrange
            var billService = new BillService(dbContextMock.Object, getMapper());

            //assert
            Assert.Throws<EntityNotFoundException>(() => billService.GetBillById(id));
        }

        [Theory]
        [InlineData(500)]
        [InlineData(1000)]
        [InlineData(2000)]
        [InlineData(3000)]
        [InlineData(4000)]
        public void DeleteBill_ForGivenIdOutOfRange_ThrowsEntityNotFoundException(int id)
        {
            //arrange
            var billService = new BillService(dbContextMock.Object, getMapper());

            //assert
            Assert.Throws<EntityNotFoundException>(() => billService.DeleteBill(id));
        }

        [Theory]
        [InlineData(500)]
        [InlineData(1000)]
        [InlineData(2000)]
        [InlineData(3000)]
        [InlineData(4000)]
        public void ChangeBillName_ForGivenIdOutOfRange_ThrowsEntityNotFoundException(int id)
        {
            //arrange
            var billService = new BillService(dbContextMock.Object, getMapper());

            //assert
            Assert.Throws<EntityNotFoundException>(() => billService.ChangeBillName(id, "name"));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void ChangeBillName_ForGivenEmptyOrNullName_ThrowsArgumentNullException(string name)
        {
            //arrange
            var billService = new BillService(dbContextMock.Object, getMapper());

            //assert
            Assert.Throws<ArgumentNullException>(() => billService.ChangeBillName(1, name));
        }

        [Theory]
        [InlineData(0, "Bill 10")]
        [InlineData(1, "Bill 11")]
        [InlineData(2, "Bill 21")]
        [InlineData(3, "Bill 31")]
        [InlineData(4, "Bill 41")]
        public void ChangeBillName_ForGivenNewName_ReturnBillWithCorrectName(int id, string correctName)
        {
            //arrange
            var billService = new BillService(dbContextMock.Object, getMapper());

            //act
            var name = billService.ChangeBillName(id, correctName).Name;

            //assert
            Assert.Equal(correctName, name);
        }
    }
}
