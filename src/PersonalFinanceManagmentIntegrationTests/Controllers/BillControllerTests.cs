using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Moq;
using PersonalFinanceManagmentProject.Entities;
namespace PersonalFinanceManagmentIntegrationTests.Controllers
{
    public class BillControllerTests
    {
        private HttpClient getHttpClient()
        {
            var factory = new WebApplicationFactory<Program>();
            factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var dbContextOptions = services.SingleOrDefault(service => service.ServiceType == typeof(DbContextOptions<PersonalFinanceManagmentDbContext>));
                    services.Remove(dbContextOptions);
                    services.AddDbContext<PersonalFinanceManagmentDbContext>(options => options.UseInMemoryDatabase("PersonalFinanceDb"));
                });
            });
            return factory.CreateClient();
        }

        [Fact]
        public async Task GetBills_ReturnsOkResult()
        {
            //act
            var response = await getHttpClient().GetAsync("/Bill");

            //assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("10.5")]
        [InlineData("one")]
        [InlineData("jeden")]
        public async Task GetBillById_WithInvalidQueryParams_ReturnsBadRequest(string param)
        { 
            //act
            var response = await getHttpClient().GetAsync("/Bill/single?id=" + param);

            //assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
