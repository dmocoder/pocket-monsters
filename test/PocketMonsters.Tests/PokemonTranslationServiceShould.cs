using System;
using System.Threading.Tasks;
using Moq;
using PocketMonsters.TranslateApi;
using Shouldly;
using Xunit;

namespace PocketMonsters.Tests
{
    public class PokemonTranslationServiceShould
    {
        private PokemonTranslationService _translationService;
        private readonly Mock<IShakespeareTranslator> _shakespeareTranslator;
        private readonly Mock<IYodaTranslator> _yodaTranslator;
        
        public PokemonTranslationServiceShould()
        {
            _shakespeareTranslator = new Mock<IShakespeareTranslator>();
            _yodaTranslator = new Mock<IYodaTranslator>();
            _translationService = new PokemonTranslationService(_shakespeareTranslator.Object, _yodaTranslator.Object);
        }

        [Fact]
        public async Task ReturnUntranslated_IfTranslationFails()
        {
            //setup
            var originalDescription = "a lousy good for nothin' slouch";
            _shakespeareTranslator
                .Setup(x => x.TranslateToShakespearean(It.IsAny<string>()))
                .ReturnsAsync(new TranslationFailedResponse("error", "too bad"));
            
            //act
            var response = await _translationService.TranslatePokemonDescription(originalDescription, "couch", false);
            
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

            SetupMockTranslations(originalDescription, shakespeareTranslation, _shakespeareTranslator, yodaTranslation, _yodaTranslator);
            
            //act
            var response = await _translationService.TranslatePokemonDescription(originalDescription, "cave", false);
            
            //assert
            response.ShouldBe(yodaTranslation);
        }
        
        [Fact]
        public async Task ReturnYodaTranslation_IfLegendary()
        {
            //setup
            var originalDescription = "a very legendary pokemon with over 1 million followers";
            var shakespeareTranslation = "A much talked about pokemon wit' boundless crowds of devotees, verily";
            var yodaTranslation = "many followers has the pokemon, legendary is she";

            SetupMockTranslations(originalDescription, shakespeareTranslation, _shakespeareTranslator, yodaTranslation, _yodaTranslator);
            
            //act
            var response = await _translationService.TranslatePokemonDescription(originalDescription, "instagram", true);
            
            //assert
            response.ShouldBe(yodaTranslation);
        }
        
        [Fact]
        public async Task ReturnShakespeareTranslation_WhenNotLegendaryOrCaveHabitat()
        {
            //setup
            var originalDescription = "a very ordinary pokemon that lives in a studio apartment";
            var shakespeareTranslation = "a humdrum pokemon that doth dwell within an abode most tiny";
            var yodaTranslation = "a boring pokemon, tiny house, he has";

            SetupMockTranslations(originalDescription, shakespeareTranslation, _shakespeareTranslator, yodaTranslation, _yodaTranslator);
            
            //act
            var response = await _translationService.TranslatePokemonDescription(originalDescription, "shoreditch", false);
            
            //assert
            response.ShouldBe(shakespeareTranslation);
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

            await _translationService
                .TranslatePokemonDescription(originalDescription, "the future", false)
                .ShouldThrowAsync<Exception>();
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
    }
}