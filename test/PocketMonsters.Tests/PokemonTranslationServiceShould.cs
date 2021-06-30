using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using PocketMonsters.TranslateApi;
using Shouldly;
using Xunit;

namespace PocketMonsters.Tests
{
    public class PokemonTranslationServiceShould
    {
        private readonly PokemonTranslationService _translationService;
        private readonly Mock<IShakespeareTranslator> _shakespeareTranslator;
        private readonly Mock<IYodaTranslator> _yodaTranslator;
        private readonly IMemoryCache _memoryCache;
        
        public PokemonTranslationServiceShould()
        {
            _memoryCache ??= new MemoryCache(new MemoryCacheOptions());
            
            _shakespeareTranslator = new Mock<IShakespeareTranslator>();
            _yodaTranslator = new Mock<IYodaTranslator>();
            _translationService = new PokemonTranslationService(
                    _shakespeareTranslator.Object, 
                    _yodaTranslator.Object, 
                    _memoryCache, 
                    Mock.Of<ILogger<PokemonTranslationService>>());
        }

        [Fact]
        public async Task ReturnUntranslated_IfTranslationFails()
        {
            //setup
            var originalDescription = "a lousy good for nothin' slouch";
            var pokemonDetails = Pokemon("slouchachu", originalDescription, "couch", false);

            _shakespeareTranslator
                .Setup(x => x.TranslateToShakespearean(It.IsAny<string>()))
                .ReturnsAsync(new TranslationFailedResponse("error", "too bad"));
            
            //act
            var response = await _translationService.TranslatePokemonDescription(pokemonDetails);
            
            //assert
            response.ShouldBe(originalDescription);
        }
        
        [Fact]
        public async Task ReturnYodaTranslation_IfHabitatCave()
        {
            //setup
            var originalDescription = "a small green mouse kind of thing that talks funny";
            var shakespeareTranslation = "A bawbling green mouse kind of thing yond talks comical";
            var yodaTranslation = "small green mouse it is, talk funny it does";
            var pokemonDetails = Pokemon("rat-boy", originalDescription, "cave", false);

            SetupMockTranslations(originalDescription, shakespeareTranslation, _shakespeareTranslator, yodaTranslation, _yodaTranslator);
            
            //act
            var response = await _translationService.TranslatePokemonDescription(pokemonDetails);
            
            //assert
            response.ShouldBe(yodaTranslation);
            response.ShouldNotBe(shakespeareTranslation);
        }
        
        [Fact]
        public async Task ReturnYodaTranslation_IfLegendary()
        {
            //setup
            var originalDescription = "a very legendary pokemon with over 1 million followers";
            var shakespeareTranslation = "A much talked about pokemon wit' boundless crowds of devotees, verily";
            var yodaTranslation = "many followers has the pokemon, legendary is she";
            var pokemonDetails = Pokemon("ms. legend", originalDescription, "instagram", true);

            SetupMockTranslations(originalDescription, shakespeareTranslation, _shakespeareTranslator, yodaTranslation, _yodaTranslator);
            
            //act
            var response = await _translationService.TranslatePokemonDescription(pokemonDetails);
            
            //assert
            response.ShouldBe(yodaTranslation);
            response.ShouldNotBe(shakespeareTranslation);
        }
        
        [Fact]
        public async Task ReturnShakespeareTranslation_WhenNotLegendaryOrCaveHabitat()
        {
            //setup
            var originalDescription = "a very ordinary pokemon that lives in a studio apartment";
            var shakespeareTranslation = "a humdrum pokemon that doth dwell within an abode most tiny";
            var yodaTranslation = "a boring pokemon, tiny house, he has";
            var pokemonDetails = Pokemon("royce", originalDescription, "shoreditch", false);
            
            SetupMockTranslations(originalDescription, shakespeareTranslation, _shakespeareTranslator, yodaTranslation, _yodaTranslator);
            
            //act
            var response = await _translationService.TranslatePokemonDescription(pokemonDetails);
            
            //assert
            response.ShouldBe(shakespeareTranslation);
            response.ShouldNotBe(yodaTranslation);
        }

        [Fact]
        public async Task ReturnUntranslated_WhenTranslatorThrows()
        {
            //setup
            var originalDescription = "a pokemon that can warn of bad times ahead";
            var shakespeareTranslation = "a pokemon what doth raise alarm for foreboding futures ahead";

            _shakespeareTranslator
                .Setup(x => x.TranslateToShakespearean(originalDescription))
                .ThrowsAsync(new Exception("oh no! panic!"));

            var response = await _translationService
                .TranslatePokemonDescription(Pokemon("alan", originalDescription, "the future", false));

            response.ShouldBe(originalDescription);
            response.ShouldNotBe(shakespeareTranslation);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task ReturnEmptyDescription_IfInvalidDescriptionSupplied(string description)
        {
            var emptyPokemon = new PokemonDetails
            {
                Name = "john",
                Description = description
            };
            
            (await _translationService.TranslatePokemonDescription(emptyPokemon))
                .ShouldBeEmpty();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task ThrowException_IfInvalidPokemonNameSupplied(string name)
        {
            var namelessOne = new PokemonDetails
            {
                Name = name,
                Description = string.Empty
            };

            await _translationService.TranslatePokemonDescription(namelessOne)
                .ShouldThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task CacheUsingNameAsKey()
        {
            var pokemonName = "coolchu";
            var originalDescription = "cool description A";
            var alternativeDescription = "alternative Description B";
            var translatedOriginal = "the coolest of all the 'mon";
            
            var pokemon = Pokemon(pokemonName, "cool description A", "the beach", false);
            var pokemonRepeat = Pokemon(pokemonName, "another description", "anywhere", false);
            
            SetupMockTranslations(
                originalDescription, 
                translatedOriginal, 
                _shakespeareTranslator,
                "very cool, he is",
                _yodaTranslator);
            
            SetupMockTranslations(
                alternativeDescription,
                "cooler than thou, for certain",
                _shakespeareTranslator,
                "ice cool, he is",
                _yodaTranslator);

            var firstTranslation = await _translationService.TranslatePokemonDescription(pokemon);
            var secondTranslation = await _translationService.TranslatePokemonDescription(pokemonRepeat);

            firstTranslation.ShouldBe(translatedOriginal);
            secondTranslation.ShouldBe(firstTranslation);
        }

        private static void SetupMockTranslations(
            string originalDescription, 
            string shakespeareTranslation, 
            Mock<IShakespeareTranslator> shakespeareTranslator, 
            string yodaTranslation, 
            Mock<IYodaTranslator> yodaTranslator)
        {
            shakespeareTranslator
                .Setup(x => x.TranslateToShakespearean(originalDescription))
                .ReturnsAsync(new TranslatedResponse {TranslatedText = shakespeareTranslation});

            yodaTranslator
                .Setup(x => x.TranslateToYodaSpeak(originalDescription))
                .ReturnsAsync(new TranslatedResponse {TranslatedText = yodaTranslation});
        }

        private static PokemonDetails Pokemon(string name, string description, string habitat, bool isLegendary)
        {
            return new PokemonDetails
            {
                Name = name,
                Description = description,
                Habitat =  habitat,
                IsLegendary = isLegendary
            };
        }
    }
}