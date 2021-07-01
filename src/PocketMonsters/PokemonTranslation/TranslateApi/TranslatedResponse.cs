namespace PocketMonsters.PokemonTranslation.TranslateApi
{
    public record TranslatedResponse : ITranslateResponse
    {
        public string TranslatedText { get; set; }
    }
}