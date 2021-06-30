using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using PocketMonsters.TranslateApi;

namespace PocketMonsters
{
    public class PokemonTranslationService : IPokemonTranslationService
    {
        private readonly IShakespeareTranslator _shakespeareTranslator;
        private readonly IYodaTranslator _yodaTranslator;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<PokemonTranslationService> _logger;

        private const int CacheDurationMin = 60;

        public PokemonTranslationService(
            IShakespeareTranslator shakespeareTranslator, 
            IYodaTranslator yodaTranslator, 
            IMemoryCache memoryCache, 
            ILogger<PokemonTranslationService> logger)
        {
            _shakespeareTranslator = shakespeareTranslator ?? throw new ArgumentNullException(nameof(shakespeareTranslator));
            _yodaTranslator = yodaTranslator ?? throw new ArgumentNullException(nameof(yodaTranslator));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Provides the translation of a supplied Pokemon's description
        /// </summary>
        /// <param name="pokemonDetails">The supplied Pokemon Details</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Pokemon Name must be valid</exception>
        public async Task<string> TranslatePokemonDescription(PokemonDetails pokemonDetails)
        {
            if (string.IsNullOrEmpty(pokemonDetails?.Name))
                throw new ArgumentException($"{nameof(pokemonDetails.Name)} cannot be null or empty");

            if (string.IsNullOrEmpty(pokemonDetails.Description))
                return string.Empty;
            
            return await _memoryCache.GetOrCreateAsync(pokemonDetails.Name, async entry =>
            {
                if (entry.SlidingExpiration != null)
                    entry.SlidingExpiration = TimeSpan.FromMinutes(CacheDurationMin);
                
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
                _logger.LogError("An exception occurred when translating", ex);
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