using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PocketMonsters.PokeDex.PokeApi
{
    public class PokeApiClient : IPokeApiClient
    {
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }
        };

        private readonly HttpClient _httpClient;
        private readonly JsonSerializer _serializer;

        public PokeApiClient(HttpClient httpClient, PokeApiOptions options)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _httpClient.BaseAddress = new Uri(options?.BaseUrl ?? throw new ArgumentNullException(nameof(options)));
            _serializer = JsonSerializer.Create(SerializerSettings);
        }

        public async Task<IPokeApiClientResponse> GetPokemonSpecies(string pokemonName)
        {
            var response = await _httpClient.GetAsync(pokemonName);
            
            switch (response.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    return new PokemonSpeciesNotFoundResponse();
                case HttpStatusCode.OK:
                {
                    await using var stream = await response.Content.ReadAsStreamAsync();
                    using var sr = new StreamReader(stream);
                    using var reader = new JsonTextReader(sr);
                    return _serializer.Deserialize<PokemonSpeciesResponse>(reader);
                }
                default:
                    return new UnsuccessfulResponse
                    {
                        HttpStatusCode = response.StatusCode
                    };
            }
        }
    }
}