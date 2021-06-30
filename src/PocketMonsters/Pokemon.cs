using PocketMonsters.PokeDex;

namespace PocketMonsters
{
    public record Pokemon
    {
        public string Name { get; init; }
        public string Description { get; init; }
        public string Habitat { get; init; }
        public bool IsLegendary { get; init; }

        public string Version => "1.0.0"; 
        
        public Pokemon()
        { }

        public Pokemon(PokemonDetails pokemonDetails)
        {
            Name = pokemonDetails.Name;
            Description = pokemonDetails.Description;
            Habitat = pokemonDetails.Habitat;
            IsLegendary = pokemonDetails.IsLegendary;
        }
    }
}