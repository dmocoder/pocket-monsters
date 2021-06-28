using System.Threading.Tasks;

namespace PocketMonsters.PokeApi
{
    public interface IPokeApiClient
    {
        Task<IPokeApiClientResponse> GetPokemonSpecies(string pokemonName);
    }
}