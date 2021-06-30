using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PocketMonsters.PokemonTranslation.TranslateApi 
{
    public class FunTranslateApiClient : IShakespeareTranslator, IYodaTranslator
    {
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }
        };

        private readonly string _shakespeareEndpoint;
        private readonly string _yodaEndpoint;
        private readonly HttpClient _client;
        private readonly JsonSerializer _serializer;

        public FunTranslateApiClient(HttpClient client, TranslateApiOptions options)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _client.BaseAddress = new Uri(options?.BaseUrl ?? throw new ArgumentNullException(nameof(options.BaseUrl)));

            _shakespeareEndpoint = options?.ShakespeareEndpoint ?? "shakespeare.json";
            _yodaEndpoint = options?.YodaEndpoint ?? "yoda.json";

            _serializer = JsonSerializer.Create(SerializerSettings);
        }

        public async Task<ITranslateResponse> TranslateToShakespearean(string text)
        {
            return await GetTranslation(_shakespeareEndpoint, text);
        } 

        public async Task<ITranslateResponse> TranslateToYodaSpeak(string text)
        {
            return await GetTranslation(_yodaEndpoint, text);
        }

        private async Task<ITranslateResponse> GetTranslation(string endpoint, string textToTranslate)
        {
            var translationRequest = new TranslateApiRequest{ Text = textToTranslate };
            
            try
            {
                var requestBody = JsonConvert.SerializeObject(translationRequest, SerializerSettings);
                var httpContent = new StringContent(requestBody, Encoding.UTF8, "application/json");

                var response = await _client.PostAsync(endpoint, httpContent);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    await using var stream = await response.Content.ReadAsStreamAsync();
                    using var sr = new StreamReader(stream);
                    using var reader = new JsonTextReader(sr);
                    var translated = _serializer.Deserialize<TranslateApiSuccessResponse>(reader);
                        
                    if(translated?.Success?.Total > 0)
                        return new TranslatedResponse{ TranslatedText = translated?.Contents?.Translated };
                        
                    return new TranslationFailedResponse("unknown", "No translations available");
                }

                return new TranslationFailedResponse(response.StatusCode.ToString(), response.ReasonPhrase);
            }
            catch(Exception ex)
            {
                return new TranslationFailedResponse("unknown", ex.Message);
            }
        }
        
        private record TranslateApiRequest
        {
            public string Text { get; init; }
        }

        private record TranslateApiSuccessResponse
        {
            public SuccessBody Success { get; init;}
            public ContentsBody Contents { get; init; }

            public record SuccessBody
            {
                public int Total { get; init; }
            }

            public record ContentsBody 
            {
                public string Translated { get; init; }
            }
        }
    }
}