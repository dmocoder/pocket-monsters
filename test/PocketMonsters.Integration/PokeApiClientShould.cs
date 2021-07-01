using System.Net.Http;
using System.Threading.Tasks;
using PocketMonsters.PokeDex.PokeApi;
using Shouldly;
using Xunit;

namespace PocketMonsters.Integration
{
    [Trait("Category", "Integration")]
    public class PokeApiClientShould
    {
        private readonly PokeApiOptions _options = new() {BaseUrl = @"https://pokeapi.co/api/v2/pokemon-species/"};

        [Fact]
        public async Task ReturnNotFound_WhenPokemonDoesNotExist()
        {
            var pokeApiClient = new PokeApiClient(new HttpClient(), _options);

            var response = await pokeApiClient.GetPokemonSpecies("danchu");

            response.ShouldBeOfType<PokemonSpeciesNotFoundResponse>();
        }

        [Fact]
        public async Task ReturnPokemonSpecies_WithHabitat_WhenPokemonDoesExist()
        {
            var pokeApiClient = new PokeApiClient(new HttpClient(), _options);

            var response = await pokeApiClient.GetPokemonSpecies("charmander");

            var pokemonSpecies = response.ShouldBeOfType<PokemonSpeciesResponse>();
            pokemonSpecies.Habitat?.Name.ShouldBe("mountain");
        }

        [Fact]
        public async Task ReturnPokemonSpecies_WithFlavorText_WhenPokemonDoesExist()
        {
            var pokeApiClient = new PokeApiClient(new HttpClient(), _options);

            var response = await pokeApiClient.GetPokemonSpecies("charmander");

            var pokemonSpecies = response.ShouldBeOfType<PokemonSpeciesResponse>();
            pokemonSpecies.FlavorTextEntries.ShouldNotBeEmpty();
        }
    }
}