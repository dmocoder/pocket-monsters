using System.Net;
using System.Net.Http;
using Xunit;
using Microsoft.AspNetCore.TestHost;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Shouldly;

namespace PocketMonsters.Integration 
{
    public class PokemonControllerShould //: IAsyncLifetime
    {
        /*
        private HttpClient _client;

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        public async Task InitializeAsync()
        {
            var hostBuilder = new HostBuilder()
                .ConfigureWebHost(wh =>
                {
                    wh.UseTestServer();
                    wh.UseStartup<PocketMonsters.Startup>();
                });

            var host = await hostBuilder.StartAsync();
            _client = host.GetTestClient();        
        }
        
        [Fact]
        public async Task Return404_WhenSuppliedNonPokemon()
        {
            (await _client.GetAsync("/pokemon/fakemon")).StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Return422_WhenSuppliedBadPokemon()
        {
            (await _client.GetAsync("$quirt!e"))
                .StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        public async Task Return500_WhenPokeApiUnavailable() { }

        public async Task ReturnPokemon_WhenSuppliedValidPokemon(){ }
        */
    }
}