using System.Threading.Tasks;

namespace PocketMonsters.PokemonTranslation.TranslateApi
{
    public interface IYodaTranslator
    {
        public Task<ITranslateResponse> TranslateToYodaSpeak(string text);
    }
}