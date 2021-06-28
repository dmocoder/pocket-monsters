using System.Threading.Tasks;
using System;
using PocketMonsters.PokeApi;
using Serilog;

namespace PocketMonsters
{
    public class PokeDexService : IPokeDexService
    {
        IPokeApiClient _pokeApiClient;
        ILogger _logger;

        public PokeDexService(IPokeApiClient pokeApiClient, ILogger logger)
        {   
            _pokeApiClient = pokeApiClient ?? throw new ArgumentNullException(nameof(pokeApiClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<IPokemonDetailsResponse> GetPokemonDetails(string pokemonName)
        {
            //TODO: if pokeapi returns not found - get the species from the name endpoint
            try
            {
                switch(await _pokeApiClient.GetPokemonSpecies(pokemonName))
                {
                    case PokemonSpeciesResponse species:
                        return Map(pokemonName, species);
                    default:
                        return new PokemonNotFound();
                } 
            }
            catch (Exception ex)
            {
                _logger.Error("An error occurred retrieving details for {PokemonName}", ex, pokemonName);
                return new ActionFailed();
            }
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
}