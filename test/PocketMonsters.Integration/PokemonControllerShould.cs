using System.Net;
using Xunit;
using Microsoft.AspNetCore.TestHost;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Shouldly;

namespace PocketMonsters.Integration 
{
    public class PokemonControllerShould
    {
        [Fact]
        public async Task Return404_WhenSuppliedNonPokemon()
        {
            var hostBuilder = new HostBuilder()
                .ConfigureWebHost(wh =>
                {
                    wh.UseTestServer();
                    wh.UseStartup<PocketMonsters.Startup>();
                });

            var host = await hostBuilder.StartAsync();
            var client = host.GetTestClient();

            var response = await client.GetAsync("/pokemon/fakemon");

            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        public async Task Return500_WhenPokeApiUnavailable() { }

        public async Task ReturnPokemon_WhenSuppliedValidPokemon(){ }
    }
}