using System.Threading.Tasks;

namespace PocketMonsters
{
    public interface IPokemonTranslationService
    {
        Task<string> TranslatePokemonDescription(PokemonDetails pokemonDetails);
    }
}