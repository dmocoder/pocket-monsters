namespace PocketMonsters.TranslateApi
{
    public record TranslatedResponse : ITranslateResponse
    {
        public string TranslatedText { get; set; } 
    }
}