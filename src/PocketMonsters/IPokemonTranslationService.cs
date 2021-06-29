using System.Threading.Tasks;

namespace PocketMonsters
{
    public interface IPokemonTranslationService
    {
        Task<string> TranslatePokemonDescription(string description, string pokemonHabitat, bool isLegendary);
    }
}