using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using Xunit;
using Microsoft.AspNetCore.TestHost;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using PocketMonsters.PokeDex.PokeApi;
using PocketMonsters.PokemonTranslation.TranslateApi;
using Shouldly;

namespace PocketMonsters.Integration 
{
    [Trait("Category","Integration")]
    public class PokemonControllerShould : IAsyncLifetime
    {
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
                    wh.ConfigureAppConfiguration(c => c.AddJsonFile("appsettings.json"));
                    wh.UseStartup<PocketMonsters.Startup>();
                });

            var host = await hostBuilder.StartAsync();
            _client = host.GetTestClient();        
        }
        
        [Fact]
        public async Task Return404_WhenSuppliedNonPokemon()
        {
            (await _client.GetAsync("/pokemon/fakemon"))
                .StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Return422_WhenSuppliedBadPokemon()
        {
            (await _client.GetAsync("/pokemon/$quirt!e"))
                .StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task ReturnServiceUnavailable_WhenPokeApiFails()
        {
            var hostBuilder = new HostBuilder()
                .ConfigureWebHost(wh =>
                {
                    wh.UseTestServer();
                    wh.ConfigureAppConfiguration(c => c.AddJsonFile("appsettings.json"));
                    wh.UseStartup<PocketMonsters.Startup>();
                    wh.ConfigureServices(c =>
                        c.Where(s => s.ServiceType == typeof(IPokeApiClient)).Select(c.Remove));
                    wh.ConfigureTestServices(c => c.AddSingleton<IPokeApiClient, BadPokeApi>());
                });

            var host = await hostBuilder.StartAsync();
            var badClient = host.GetTestClient();
            
            (await badClient.GetAsync("/pokemon/pikachu"))
                .StatusCode.ShouldBe(HttpStatusCode.ServiceUnavailable);
        }

        [Fact]
        public async Task ReturnOriginalDescription_WhenTranslationClientsFail()
        {
            var hostBuilder = new HostBuilder()
                .ConfigureWebHost(wh =>
                {
                    wh.UseTestServer();
                    wh.ConfigureAppConfiguration(c => c.AddJsonFile("appsettings.json"));
                    wh.UseStartup<PocketMonsters.Startup>();
                    wh.ConfigureServices(c =>
                        c.Where(s => s.ServiceType == typeof(IShakespeareTranslator) || s.ServiceType == typeof(IYodaTranslator))
                        .Select(c.Remove));
                    wh.ConfigureTestServices(c =>
                    {
                        c.AddSingleton<IShakespeareTranslator, BadBard>();
                        c.AddSingleton<IYodaTranslator, BadYoda>();
                    });
                });

            var host = await hostBuilder.StartAsync();
            var badClient = host.GetTestClient();
            
            var getDetailsResponse = await badClient.GetAsync("/pokemon/pikachu");
            var getTranslatedDetailsResponse = await badClient.GetAsync("/pokemon/translated/pikachu");
            
            dynamic untranslatedDetails = JToken.Parse(await getDetailsResponse.Content.ReadAsStringAsync());
            dynamic translatedDetails = JToken.Parse(await getTranslatedDetailsResponse.Content.ReadAsStringAsync());

            getTranslatedDetailsResponse?.StatusCode.ShouldBe(HttpStatusCode.OK);

            var translatedDescription = translatedDetails["description"] as string;
            var originalDescription = untranslatedDetails["description"] as string;

            translatedDescription.ShouldBe(originalDescription);
        }

        [Fact]
        public async Task ReturnPokemon_WhenSuppliedValidPokemon()
        {
            var mewtwoResponse = await _client.GetAsync("/pokemon/mewtwo");
            
            mewtwoResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

            var mewtwo = JToken.Parse(await mewtwoResponse.Content.ReadAsStringAsync());

            mewtwo["name"].ShouldBe("mewtwo");
            mewtwo["description"].ShouldBe("It was created by a scientist after years of horrific gene splicing and DNA engineering experiments.");
            mewtwo["habitat"].ShouldBe("rare");
            var isLegendary = mewtwo["isLegendary"]?.ToString() ?? "False";
            bool.Parse(isLegendary).ShouldBeTrue();
        }
        
        // Commented out because this will be non-determinate due to Request limits on the Translator Apis.
        // This test would ideally be fulfilled with a proper mock of the translators injected or a simulator
        // [Fact]
        // public async Task ReturnPokemon_WhenSuppliedValidPokemon()
        // { }

        private class BadPokeApi : IPokeApiClient
        {
            public Task<IPokeApiClientResponse> GetPokemonSpecies(string pokemonName)
            {
                throw new Exception("Error: team rocket are here!");
            }
        }

        private class BadBard : IShakespeareTranslator
        {
            public Task<ITranslateResponse> TranslateToShakespearean(string text)
            {
                throw new Exception("et tu, pikachu?");
            }
        }

        private class BadYoda : IYodaTranslator
        {
            public Task<ITranslateResponse> TranslateToYodaSpeak(string text)
            {
                throw new Exception("Not afraid? You will be. You WILL be!");
            }
        }
    }
}