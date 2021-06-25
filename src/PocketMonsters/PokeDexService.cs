using System.Threading.Tasks;
using LanguageExt;

namespace PocketMonsters
{
    public class PokeDexService : IPokeDexService
    {
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