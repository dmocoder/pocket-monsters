namespace PocketMonsters.TranslateApi
{
    public record TranslationFailedResponse : ITranslateResponse 
    { 
        public string ErrorCode { get; }
        public string ErrorMessage { get; }

        public TranslationFailedResponse(string errorCode, string errorMessage)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }
    }
}