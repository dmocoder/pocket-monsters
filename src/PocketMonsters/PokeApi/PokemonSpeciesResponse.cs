namespace PocketMonsters.PokeApi
{
    public record PokemonSpeciesResponse : IPokeApiClientResponse
    {
        public FlavorTextEntry[] FlavorTextEntries { get; set; }
        public Link Habitat { get; set; }
        public bool? IsLegendary { get; set; }
    }
}