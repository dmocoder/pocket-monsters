using System.Net.Http;
using System.Threading.Tasks;
using PocketMonsters.PokemonTranslation.TranslateApi;
using Shouldly;
using Xunit;

namespace PocketMonsters.Integration
{
    [Trait("Category", "Integration")]
    [Trait("Category", "RateLimited")]
    public class FunTranslateApiClientShould
    {
        private readonly FunTranslateApiClient _translateApi;

        private readonly TranslateApiOptions translateApiOptions = new()
        {
            BaseUrl = "https://api.funtranslations.com/translate/",
            YodaEndpoint = "yoda.json",
            ShakespeareEndpoint = "shakespeare.json"
        };

        public FunTranslateApiClientShould()
        {
            _translateApi = new FunTranslateApiClient(new HttpClient(), translateApiOptions);
        }

        [Fact]
        public async Task ReturnTranslatedText_WhenTranslatingToShakespearean()
        {
            var response = await _translateApi
                .TranslateToShakespearean(
                    "You gave Mr. Tim a hearty meal, but unfortunately what he ate made him die.");

            var translation = response.ShouldBeOfType<TranslatedResponse>();
            translation.TranslatedText
                .ShouldBe(
                    "Thee did giveth mr. Tim a hearty meal,  but unfortunately what he did doth englut did maketh him kicketh the bucket.");
        }

        [Fact]
        public async Task ReturnTranslatedText_WhenTranslatingToYodaSpeak()
        {
            var response = await _translateApi
                .TranslateToYodaSpeak("Master Obiwan has lost a planet.");

            var translation = response.ShouldBeOfType<TranslatedResponse>();
            translation.TranslatedText
                .ShouldBe("Lost a planet,  master obiwan has.");
        }
    }
}