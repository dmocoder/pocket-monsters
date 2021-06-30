using System.Threading.Tasks;

namespace PocketMonsters
{
    public interface IPokemonTranslationService
    {
        Task<string> TranslatePokemonDescription(Pokemon pokemon);
    }
}