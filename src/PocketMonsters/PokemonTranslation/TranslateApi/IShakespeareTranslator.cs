using System.Threading.Tasks;

namespace PocketMonsters.PokemonTranslation.TranslateApi
{
    public interface IShakespeareTranslator
    {
        public Task<ITranslateResponse> TranslateToShakespearean(string text);
    }
}