using Xunit;
using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Shouldly;
using Moq;
using Microsoft.Extensions.Logging;
using PocketMonsters.PokeDex;
using PocketMonsters.PokeDex.PokeApi;

namespace PocketMonsters.Tests
{
    [Trait("Category", "Unit")]
    public class PokeDexServiceShould
    {
        private readonly Mock<IPokeApiClient> _pokeApiClient;
        private readonly PokeDexService _pokeDexService;

        public PokeDexServiceShould()
        {
            _pokeApiClient = new Mock<IPokeApiClient>();
            _pokeDexService = new PokeDexService(_pokeApiClient.Object, Mock.Of<ILogger<PokeDexService>>());
        }

        [Fact]
        public async Task ReturnNone_IfPokemonDoesNotExist()
        {
            //setup
            _pokeApiClient.Setup(x => x.GetPokemonSpecies("no-pokemon"))
                .ReturnsAsync(new PokemonSpeciesNotFoundResponse());

            //act
            var result = await _pokeDexService.GetPokemonDetails("no-pokemon");

            //assert
            result.ShouldBeOfType<PokemonNotFound>();
        }

        [Fact]
        public async Task ReturnDescriptionWithoutNewLines_IfPokemonReturnedFromPokeApi()
        {
            //setup
            var pokemonArg = "coder-chu";
            var flavorText = "a really cool\nkind of dude\n";
            SetupPokeApi(pokemonArg, _pokeApiClient, flavorText: flavorText);
            
            //act
            var result = await _pokeDexService.GetPokemonDetails(pokemonArg);

            //assert
            var details = result.ShouldBeOfType<PokemonDetails>();
            details.Description.ShouldBe("a really cool kind of dude");
        }

        [Fact]
        public async Task ReturnLegendaryStatus_IfPokemonReturnedFromPokeApi()
        {
            //setup
            var pokemonArg = "hitmondan";
            SetupPokeApi(pokemonArg, _pokeApiClient, isLegendary: true);

            //act
            var result = await _pokeDexService.GetPokemonDetails(pokemonArg);

            //assert
            var details = result.ShouldBeOfType<PokemonDetails>();
            details.IsLegendary.ShouldBeTrue();
        }

        [Fact]
        public async Task ReturnHabitat_IfPokemonReturnedFromPokeApi()
        {
            //setup
            var pokemonArg = "hipstermon";
            var habitat = "cafe";
            SetupPokeApi(pokemonArg, _pokeApiClient, habitat: habitat);

            //act
            var result = await _pokeDexService.GetPokemonDetails(pokemonArg);

            //assert
            var details = result.ShouldBeOfType<PokemonDetails>();
            details.Habitat.ShouldBe(habitat);
        }

        [Fact]
        public async Task ReturnGetPokemonDetailsFailed_IfPokeApiThrows()
        {
            //setup
            var pokemonArg = "irrelevant";
            _pokeApiClient.Setup(x => x.GetPokemonSpecies(pokemonArg))
                .ThrowsAsync(new Exception("team rocket error"));

            //act
            var result = await _pokeDexService.GetPokemonDetails(pokemonArg);

            //assert
            result.ShouldBeOfType<GetPokemonDetailsFailed>();
        }
        
        [Fact]
        public async Task ReturnGetPokemonDetailsFailed_IfPokeApiReturns429()
        {
            //setup
            var pokemonArg = "irrelevant";
            _pokeApiClient.Setup(x => x.GetPokemonSpecies(pokemonArg))
                .ReturnsAsync(new UnsuccessfulResponse {HttpStatusCode = HttpStatusCode.TooManyRequests});

            //act
            var result = await _pokeDexService.GetPokemonDetails(pokemonArg);

            //assert
            result.ShouldBeOfType<GetPokemonDetailsFailed>();
        }

        [Fact]
        public async Task ReturnNotFound_IfNoEnglishFlavorText()
        {
            //setup
            var pokemonArg = "tommychu";
            SetupPokeApi(pokemonArg, _pokeApiClient, language: "fr");

            //act
            var result = await _pokeDexService.GetPokemonDetails(pokemonArg);

            //assert
            var details = result.ShouldBeOfType<PokemonNotFound>();
        }

        private static void SetupPokeApi(string pokemonName, Mock<IPokeApiClient> mock, string flavorText = "a default pokemon", bool isLegendary = false, string habitat = "caves", string language = "en")
        {
            var pokemonSpeciesStub = new PokemonSpeciesResponse
            {
                FlavorTextEntries = new []{ new FlavorTextEntry {
                    FlavorText = flavorText,
                    Language = new Link{ Name = language}
                 }},
                 Habitat = new Link{ Name = habitat},
                 IsLegendary = isLegendary
            };

            mock.Setup(x => x.GetPokemonSpecies(pokemonName))
                .ReturnsAsync(pokemonSpeciesStub);
        }
    }
}