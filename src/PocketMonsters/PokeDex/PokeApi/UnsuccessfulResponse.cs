using System.Net;

namespace PocketMonsters.PokeDex.PokeApi
{
    public record UnsuccessfulResponse : IPokeApiClientResponse
    {
        public HttpStatusCode HttpStatusCode { get; init; }
    }
}