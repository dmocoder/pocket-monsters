using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using PocketMonsters.TranslateApi;

namespace PocketMonsters
{
    public class PokemonTranslationService : IPokemonTranslationService
    {
        private readonly IShakespeareTranslator _shakespeareTranslator;
        private readonly IYodaTranslator _yodaTranslator;
        private readonly IMemoryCache _memoryCache;

        public PokemonTranslationService(IShakespeareTranslator shakespeareTranslator, IYodaTranslator yodaTranslator, IMemoryCache memoryCache)
        {
            _shakespeareTranslator = shakespeareTranslator ?? throw new ArgumentNullException(nameof(shakespeareTranslator));
            _yodaTranslator = yodaTranslator ?? throw new ArgumentNullException(nameof(yodaTranslator));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        public async Task<string> TranslatePokemonDescription(PokemonDetails pokemonDetails)
        {
            if (string.IsNullOrEmpty(pokemonDetails?.Name))
                throw new ArgumentException($"{nameof(pokemonDetails.Name)} cannot be null or empty");

            if (string.IsNullOrEmpty(pokemonDetails.Description))
                return string.Empty;
            
            return await _memoryCache.GetOrCreateAsync(pokemonDetails.Name, async entry =>
            {
                //TODO: Add to Config
                if (entry.SlidingExpiration != null)
                    entry.SlidingExpiration = TimeSpan.FromHours(1);
                
                return await Translate(pokemonDetails.Description, pokemonDetails.Habitat, pokemonDetails.IsLegendary);
            });
        }

        private async Task<string> Translate(string description, string habitat, bool isLegendary)
        {
            var translate = ResolveTranslator(habitat, isLegendary);

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