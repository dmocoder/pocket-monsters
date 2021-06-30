namespace PocketMonsters.PokeDex.PokeApi
{
    public record PokemonSpeciesResponse : IPokeApiClientResponse
    {
        public FlavorTextEntry[] FlavorTextEntries { get; init; }
        public Link Habitat { get; init; }
        public bool? IsLegendary { get; init; }
    }
}