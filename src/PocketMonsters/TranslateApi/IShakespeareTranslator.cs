using System.Threading.Tasks;

namespace PocketMonsters.TranslateApi
{
    public interface IShakespeareTranslator
    {
        public Task<ITranslateResponse> TranslateToShakespearean(string text);
    }
}