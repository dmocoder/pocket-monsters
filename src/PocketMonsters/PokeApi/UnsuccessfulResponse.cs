using System.Net;

namespace PocketMonsters.PokeApi
{
    public record UnsuccessfulResponse : IPokeApiClientResponse
    {
        public HttpStatusCode HttpStatusCode { get; init; }
    }
}