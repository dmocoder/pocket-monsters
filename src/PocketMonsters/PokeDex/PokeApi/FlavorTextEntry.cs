namespace PocketMonsters.PokeDex.PokeApi
{
    public record FlavorTextEntry
    {
        public string FlavorText { get; init; }
        public Link Language { get; init; }
    }
}