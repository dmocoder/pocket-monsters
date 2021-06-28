using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Serialization;

namespace PocketMonsters.PokeApi
{
    public class PokeApiClient : IPokeApiClient
    {
        static readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }
        };

        HttpClient _httpClient;
        JsonSerializer _serializer;

        public PokeApiClient(HttpClient httpClient, string baseAddress)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            
            _httpClient.BaseAddress = new Uri(baseAddress);
            _serializer = JsonSerializer.Create(_serializerSettings);
        }

        public async Task<IPokeApiClientResponse> GetPokemonSpecies(string pokemonName)
        {
            var response = await _httpClient.GetAsync(pokemonName);
            
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return new PokemonSpeciesNotFoundResponse();
            }
            
            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (var stream = await response.Content.ReadAsStreamAsync())
                using(var sr = new StreamReader(stream))
                using (var reader = new JsonTextReader(sr))
                {
                    return _serializer.Deserialize<PokemonSpeciesResponse>(reader);
                }
            }

            return new UnsuccessfulResponse
            {
                HttpStatusCode = response.StatusCode
            };
        }
    }
}