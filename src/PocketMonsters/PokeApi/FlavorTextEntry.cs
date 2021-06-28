namespace PocketMonsters.PokeApi
{
    public record FlavorTextEntry
    {
        public string FlavorText { get; set; }
        public Link Language { get; set; }
        public Link Version { get; set; }
    }
}