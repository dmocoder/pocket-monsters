using System.Threading.Tasks;

namespace PocketMonsters
{
    public interface IPokeDexService
    {
        Task<IPokemonDetailsResponse> GetPokemonDetails(string pokemonName);
    }
}