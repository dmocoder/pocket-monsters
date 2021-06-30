using Xunit;
using Shouldly;
using System.Threading.Tasks;
using PocketMonsters.PokeDex.PokeApi;

namespace PocketMonsters.Integration 
{
    public class PokeApiClientShould
    {
        PokeApiOptions _options = new PokeApiOptions{ BaseUrl = @"https://pokeapi.co/api/v2/pokemon-species/" };

        [Fact]
        public async Task ReturnNotFound_WhenPokemonDoesNotExist()
        {
            var pokeApiClient = new PokeApiClient(new System.Net.Http.HttpClient(), _options);

            var response = await pokeApiClient.GetPokemonSpecies("danchu");

            response.ShouldBeOfType<PokemonSpeciesNotFoundResponse>();
        }

        [Fact]
        public async Task ReturnPokemonSpecies_WithHabitat_WhenPokemonDoesExist()
        {
            var pokeApiClient = new PokeApiClient(new System.Net.Http.HttpClient(), _options);

            var response = await pokeApiClient.GetPokemonSpecies("charmander");

            var pokemonSpecies = response.ShouldBeOfType<PokemonSpeciesResponse>();
            pokemonSpecies.Habitat?.Name.ShouldBe("mountain");
        }

        [Fact]
        public async Task ReturnPokemonSpecies_WithFlavorText_WhenPokemonDoesExist()
        {
            var pokeApiClient = new PokeApiClient(new System.Net.Http.HttpClient(), _options);

            var response = await pokeApiClient.GetPokemonSpecies("charmander");

            var pokemonSpecies = response.ShouldBeOfType<PokemonSpeciesResponse>();
            pokemonSpecies.FlavorTextEntries.ShouldNotBeEmpty();
        }


        // [Fact]
        // public async Task ReturnErrorCode_WhenRequestFails() { }
    }
}