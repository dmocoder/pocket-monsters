namespace PocketMonsters.PokeApi
{
    public record PokemonSpeciesNotFoundResponse : IPokeApiClientResponse 
    {
        public string Message { get; init; }
    }
}