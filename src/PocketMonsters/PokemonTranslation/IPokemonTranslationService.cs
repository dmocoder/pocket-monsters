using System.Threading.Tasks;

namespace PocketMonsters.PokemonTranslation
{
    public interface IPokemonTranslationService
    {
        Task<string> TranslatePokemonDescription(Pokemon pokemon);
    }
}