using System.Net.Http;
using System.Threading.Tasks;
using System;
using LanguageExt;
using PocketMonsters.PokeApi;

namespace PocketMonsters
{
    public class PokeDexService : IPokeDexService
    {
        IPokeApiClient _pokeApiClient;

        public PokeDexService(IPokeApiClient pokeApiClient)
        {   
            _pokeApiClient = pokeApiClient ?? throw new ArgumentNullException(nameof(pokeApiClient));
        }
        
        public Task<Option<PokemonDetails>> GetPokemonDetails(string pokemonName)
        {
            return Task.FromResult(Option<PokemonDetails>.None);
        }
    }

    public interface IPokeDexService
    {
        Task<Option<PokemonDetails>> GetPokemonDetails(string pokemonName);
    }

    public record PokemonDetails
    {
        public string Name { get; init; }
        public string Description { get; init; }
        public string Habitat { get; init; }
        public bool IsLegendary { get; init; }
    }
}