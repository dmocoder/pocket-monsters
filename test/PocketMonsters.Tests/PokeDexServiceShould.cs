using Xunit;
using System;
using System.Threading.Tasks;
using Shouldly;
using Moq;
using PocketMonsters.PokeApi;
using Serilog;

namespace PocketMonsters.Tests
{
    public class PokeDexServiceShould
    {
        Mock<IPokeApiClient> _pokeApiClient;
        PokeDexService _pokeDexService;

        public PokeDexServiceShould()
        {
            _pokeApiClient = new Mock<IPokeApiClient>();
            _pokeDexService = new PokeDexService(_pokeApiClient.Object, Mock.Of<ILogger>());
        }

        [Fact]
        public async Task ReturnNone_IfPokemonDoesNotExist()
        {
            //setup
            _pokeApiClient.Setup(x => x.GetPokemonSpecies("no-pokemon"))
                .ReturnsAsync(new PokemonSpeciesNotFoundResponse());

            //act
            var result = await _pokeDexService.GetPokemonDetails("fake-mon");

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
        public async Task ReturnActionFailed_IfPokeApiThrows()
        {
            //setup
            var pokemonArg = "irrelevant";
            _pokeApiClient.Setup(x => x.GetPokemonSpecies(pokemonArg))
                .Throws(new Exception("team rocket error"));

            //act
            var result = await _pokeDexService.GetPokemonDetails(pokemonArg);

            //assert
            var details = result.ShouldBeOfType<ActionFailed>();
        }

        private static void SetupPokeApi(string pokemonName, Mock<IPokeApiClient> mock, string flavorText = "a default pokemon", bool isLegendary = false, string habitat = "caves")
        {
            var pokemonSpeciesStub = new PokemonSpeciesResponse
            {
                FlavorTextEntries = new []{ new FlavorTextEntry {
                    FlavorText = flavorText,
                    Language = new Link{ Name = "uk"},
                    Version = new Link{ Name = "best"}
                 }},
                 Habitat = new Link{ Name = habitat},
                 IsLegendary = isLegendary
            };

            mock.Setup(x => x.GetPokemonSpecies(pokemonName))
                .ReturnsAsync(pokemonSpeciesStub);
        }
    }
}