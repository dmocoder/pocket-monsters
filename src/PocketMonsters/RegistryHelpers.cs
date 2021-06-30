using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PocketMonsters.PokeDex;
using PocketMonsters.PokeDex.PokeApi;
using PocketMonsters.PokemonTranslation;
using PocketMonsters.PokemonTranslation.TranslateApi;

namespace PocketMonsters
{
    public static class RegistryHelpers
    {
        public static void RegisterPokeDex(this IServiceCollection services, IConfiguration configuration)
        {
            var pokeApiOptions = new PokeApiOptions();
            configuration.GetSection("PokeApi").Bind(pokeApiOptions);
            services.AddSingleton(pokeApiOptions);
            
            services.AddHttpClient<PokeApiClient>();
            services.AddSingleton<IPokeApiClient, PokeApiClient>();
            services.AddSingleton<IPokeDexService, PokeDexService>();
        }
        
        public static void RegisterPokemonTranslator(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMemoryCache();

            var translateApiOptions = new TranslateApiOptions();
            configuration.GetSection("TranslateApi").Bind(translateApiOptions);
            services.AddSingleton(translateApiOptions);
            
            services.AddHttpClient<FunTranslateApiClient>();
            services.AddSingleton<FunTranslateApiClient>();
            services.AddSingleton<IShakespeareTranslator>(x => x.GetRequiredService<FunTranslateApiClient>());
            services.AddSingleton<IYodaTranslator>(x => x.GetRequiredService<FunTranslateApiClient>());
            services.AddSingleton<IPokemonTranslationService, PokemonTranslationService>();
        }
    }
}