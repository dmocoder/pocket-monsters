using System.Threading.Tasks;
using System;
using PocketMonsters.PokeApi;

namespace PocketMonsters
{
    public class PokeDexService : IPokeDexService
    {
        IPokeApiClient _pokeApiClient;

        public PokeDexService(IPokeApiClient pokeApiClient)
        {   
            _pokeApiClient = pokeApiClient ?? throw new ArgumentNullException(nameof(pokeApiClient));
        }
        
        public async Task<IPokemonDetailsResponse> GetPokemonDetails(string pokemonName)
        {
            //TODO: if pokeapi returns not found - get the species from the name endpoint
            try
            {
                switch(await _pokeApiClient.GetPokemonSpecies(pokemonName))
                {
                    case PokemonSpeciesNotFoundResponse:
                        return new PokemonNotFound();
                    case PokemonSpeciesResponse species:
                        return Map(pokemonName, species);
                } 
            }
            catch
            { }

            return new PokemonNotFound();
        }

        private PokemonDetails Map(string name, PokemonSpeciesResponse speciesResponse)
        {
            return new PokemonDetails
            {
                Name = name,
                Description = MapFlavorText(speciesResponse.FlavorTextEntries[0].FlavorText),
                Habitat = speciesResponse.Habitat.Name,
                IsLegendary = speciesResponse.IsLegendary ?? false
            };
        }

        private static string MapFlavorText(string flavorText)
        {
            if (string.IsNullOrEmpty(flavorText))
                throw new ArgumentException("Flavor text cannot be null or empty", nameof(flavorText));

            return flavorText.Replace("\n", " ").Trim(); 
        }
    }

    public interface IPokemonDetailsResponse 
    { }

    public record PokemonNotFound : IPokemonDetailsResponse
    { }
}