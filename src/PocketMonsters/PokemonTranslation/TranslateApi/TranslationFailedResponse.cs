namespace PocketMonsters.PokemonTranslation.TranslateApi
{
    public record TranslationFailedResponse : ITranslateResponse
    {
        public TranslationFailedResponse(string errorCode, string errorMessage)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }

        public string ErrorCode { get; }
        public string ErrorMessage { get; }
    }
}