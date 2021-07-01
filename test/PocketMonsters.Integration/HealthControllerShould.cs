using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Shouldly;
using Xunit;

namespace PocketMonsters.Integration
{
    [Trait("Category", "Integration")]
    public class HealthControllerShould
    {
        [Fact]
        public async Task ReturnOk()
        {
            var hostBuilder = new HostBuilder()
                .ConfigureWebHost(wh =>
                {
                    wh.UseTestServer();
                    wh.UseStartup<Startup>();
                });

            var host = await hostBuilder.StartAsync();
            var client = host.GetTestClient();

            var response = await client.GetAsync("/health");

            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }
    }
}