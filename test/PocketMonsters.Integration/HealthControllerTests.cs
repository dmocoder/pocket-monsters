using System.Net;
using Xunit;
using Microsoft.AspNetCore.TestHost;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Shouldly;

namespace PocketMonsters.Integration 
{
    public class HealthControllerTests 
    {
        [Fact]
        public async Task HealthEndpoint_ReturnsOk()
        {
            using var server = new TestServer(new WebHostBuilder().UseStartup<PocketMonsters.Startup>());
            var response = await server.CreateRequest("/health")
                .SendAsync("GET");

            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }
    }
}