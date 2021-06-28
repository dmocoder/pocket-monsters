using Xunit;
using Shouldly;
using System.Threading.Tasks;
using PocketMonsters.PokeApi;

namespace PocketMonsters.Integration 
{
    public class PokeApiClientShould
    {
        [Fact]
        public async Task ReturnNotFound_WhenPokemonDoesNotExist()
        {
            var pokeApiClient = new PokeApiClient(new System.Net.Http.HttpClient(), @"https://pokeapi.co/api/v2/pokemon-species/");

            var response = await pokeApiClient.GetPokemonSpecies("danchu");

            response.ShouldBeOfType<PokemonSpeciesNotFoundResponse>();
        }

        [Fact]
        public async Task ReturnPokemonSpecies_WithHabitat_WhenPokemonDoesExist()
        {
            var pokeApiClient = new PokeApiClient(new System.Net.Http.HttpClient(), @"https://pokeapi.co/api/v2/pokemon-species/");

            var response = await pokeApiClient.GetPokemonSpecies("charmander");

            var pokemonSpecies = response.ShouldBeOfType<PokemonSpeciesResponse>();
            pokemonSpecies.Habitat?.Name.ShouldBe("mountain");
        }

        [Fact]
        public async Task ReturnPokemonSpecies_WithFlavorText_WhenPokemonDoesExist()
        {
            var pokeApiClient = new PokeApiClient(new System.Net.Http.HttpClient(), @"https://pokeapi.co/api/v2/pokemon-species/");

            var response = await pokeApiClient.GetPokemonSpecies("charmander");

            var pokemonSpecies = response.ShouldBeOfType<PokemonSpeciesResponse>();
            pokemonSpecies.FlavorTextEntries.ShouldNotBeEmpty();
        }

        // [Fact]
        // public async Task ReturnErrorCode_WhenRequestFails() { }
    }
}