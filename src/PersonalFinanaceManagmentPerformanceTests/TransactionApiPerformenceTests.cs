using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Http;
using NBomber.Http.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace PersonalFinanaceManagment.Tests.Performance
{
    public class TransactionApiPerformenceTests
    {
        private readonly ITestOutputHelper _outputHelper;

        public TransactionApiPerformenceTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        [Fact]
        public void GetTransactionsShouldHandleAtLeast100RequestPerSeconds()
        {
            const string url = "http://localhost:5074/transaction?pageNumber=0";

            var getTransactionScenario = Scenario.Create("get transaction", async context =>
            {
                var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(url);
                try
                {
                    var request = Http.CreateRequest("GET", url);
                    return await Http.Send(httpClient, request);
                }
                catch
                {
                    return Response.Fail();
                }
            })
                .WithWarmUpDuration(TimeSpan.FromSeconds(5))
                .WithLoadSimulations(LoadSimulation.NewKeepConstant(10000, TimeSpan.FromSeconds(5)));

            var stats = NBomberRunner
                .RegisterScenarios(getTransactionScenario)
                .Run();

            _outputHelper.WriteLine(stats.GetScenarioStats("get transaction").ToString());
        }
    }
}
