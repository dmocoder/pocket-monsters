using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PocketMonsters.PokeDex.PokeApi;

namespace PocketMonsters.PokeDex
{
    public class PokeDexService : IPokeDexService
    {
        private readonly IPokeApiClient _pokeApiClient;
        private readonly ILogger _logger;

        public PokeDexService(IPokeApiClient pokeApiClient, ILogger<PokeDexService> logger)
        {   
            _pokeApiClient = pokeApiClient ?? throw new ArgumentNullException(nameof(pokeApiClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        /// <summary>
        /// Returns English language details for the Pokemon if it is a valid Pokemon.
        /// </summary>
        /// <param name="pokemonName"></param>
        /// <returns>
        /// PokemonDetails: An object containing details surrounding the Pokemon including description
        /// PokemonNotFound: Result type indicating that the Pokemon could not be found using the external lookup service 
        /// </returns>
        public async Task<IPokemonDetailsResponse> GetPokemonDetails(string pokemonName)
        {
            var correctedName = pokemonName.ToLowerInvariant();
            
            try
            {
                switch(await _pokeApiClient.GetPokemonSpecies(correctedName))
                {
                    case PokemonSpeciesResponse species:
                        return Map(correctedName, species);
                    case PokemonSpeciesNotFoundResponse:
                    {
                        _logger.LogWarning("{PokemonName} details not found", pokemonName);
                        return new PokemonNotFound();
                    }
                    case UnsuccessfulResponse unsuccessful:
                    {
                        _logger.LogError("Unable to retrieve pokemon details for {PokemonName}: {HttpErrorCode}", 
                            pokemonName, unsuccessful.HttpStatusCode);
                        return new GetPokemonDetailsFailed();
                    }
                    default:
                        return new GetPokemonDetailsFailed();
                } 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred retrieving details for {PokemonName}", pokemonName);
                return new GetPokemonDetailsFailed();
            }
        }

        private static IPokemonDetailsResponse Map(string name, PokemonSpeciesResponse speciesResponse)
        {
            if (!FlavorTextMapper.TryMap(speciesResponse?.FlavorTextEntries, out var flavorText))
                return new PokemonNotFound();
                
            return new PokemonDetails
            {
                Name = name,
                Description = flavorText,
                Habitat = speciesResponse?.Habitat?.Name,
                IsLegendary = speciesResponse?.IsLegendary ?? false
            };
        }
    }
}