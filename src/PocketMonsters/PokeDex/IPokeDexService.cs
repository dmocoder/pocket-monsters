using System.Threading.Tasks;

namespace PocketMonsters.PokeDex
{
    public interface IPokeDexService
    {
        Task<IPokemonDetailsResponse> GetPokemonDetails(string pokemonName);
    }
}