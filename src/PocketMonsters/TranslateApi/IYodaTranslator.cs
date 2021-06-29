using System.Threading.Tasks;

namespace PocketMonsters.TranslateApi
{
    public interface IYodaTranslator
    {
        public Task<ITranslateResponse> TranslateToYodaSpeak(string text);
    }
}