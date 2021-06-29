using System;
using System.Threading.Tasks;
using PocketMonsters.TranslateApi;

namespace PocketMonsters
{
    public class PokemonTranslationService : IPokemonTranslationService
    {
        private readonly IShakespeareTranslator _shakespeareTranslator;
        private readonly IYodaTranslator _yodaTranslator;

        public PokemonTranslationService(IShakespeareTranslator shakespeareTranslator, IYodaTranslator yodaTranslator)
        {
            _shakespeareTranslator = shakespeareTranslator ?? throw new ArgumentNullException(nameof(shakespeareTranslator));
            _yodaTranslator = yodaTranslator ?? throw new ArgumentNullException(nameof(yodaTranslator));
        }

        public async Task<string> TranslatePokemonDescription(string description, string pokemonHabitat, bool isLegendary)
        {
            var translate = ResolveTranslator(pokemonHabitat, isLegendary);

            try
            {
                if (await translate.Invoke(description) is TranslatedResponse translated)
                    return translated.TranslatedText;
            }
            catch (Exception ex)
            {
                //TODO: log ex
            }

            return description;
        }

        private Func<string, Task<ITranslateResponse>> ResolveTranslator(string pokemonHabitat, bool isLegendary)
        {
            if (pokemonHabitat == "cave" || isLegendary)
                return async s => await _yodaTranslator.TranslateToYodaSpeak(s);
            
            return async s => await _shakespeareTranslator.TranslateToShakespearean(s);
        }
    }
}