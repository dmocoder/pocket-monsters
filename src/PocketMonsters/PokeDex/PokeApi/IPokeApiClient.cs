using System.Threading.Tasks;

namespace PocketMonsters.PokeDex.PokeApi
{
    public interface IPokeApiClient
    {
        Task<IPokeApiClientResponse> GetPokemonSpecies(string pokemonName);
    }
}