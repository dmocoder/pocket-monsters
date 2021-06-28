using System.Threading.Tasks;
using System;
using PocketMonsters.PokeApi;
using Microsoft.Extensions.Logging;

namespace PocketMonsters
{
    public class PokeDexService : IPokeDexService
    {
        IPokeApiClient _pokeApiClient;
        ILogger _logger;

        public PokeDexService(IPokeApiClient pokeApiClient, ILogger<PokeDexService> logger)
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
                _logger.LogError("An error occurred retrieving details for {PokemonName}", ex, pokemonName);
                return new ActionFailed();
            }
        }

        private IPokemonDetailsResponse Map(string name, PokemonSpeciesResponse speciesResponse)
        {
            if (!FlavorTextMapper.TryMap(speciesResponse?.FlavorTextEntries, out var flavorText))
                return new PokemonNotFound();
                
            return new PokemonDetails
            {
                Name = name,
                Description = flavorText,
                Habitat = speciesResponse.Habitat.Name,
                IsLegendary = speciesResponse.IsLegendary ?? false
            };
        }
    }
}