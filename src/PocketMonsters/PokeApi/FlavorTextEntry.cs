namespace PocketMonsters.PokeApi
{
    public record FlavorTextEntry
    {
        public string FlavorText { get; init; }
        public Link Language { get; init; }
    }
}