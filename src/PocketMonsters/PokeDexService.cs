using System.Threading.Tasks;
using System;
using PocketMonsters.PokeApi;
using Microsoft.Extensions.Logging;

namespace PocketMonsters
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
        
        public async Task<IPokemonDetailsResponse> GetPokemonDetails(string pokemonName)
        {
            var correctedName = pokemonName.ToLowerInvariant();
            //TODO: if pokeapi returns not found - get the species from the name endpoint
            try
            {
                switch(await _pokeApiClient.GetPokemonSpecies(correctedName))
                {
                    case PokemonSpeciesResponse species:
                        return Map(correctedName, species);
                    case UnsuccessfulResponse unsuccessful:
                    {
                        _logger.LogError("Unable to retrieve pokemon details for {PokemonName}: {HttpErrorCode}", 
                            pokemonName, 
                            unsuccessful.HttpStatusCode);
                        return new PokemonNotFound();
                    }
                    case PokemonSpeciesNotFoundResponse notFound:
                    default:
                        return new PokemonNotFound();
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
                Habitat = speciesResponse?.Habitat.Name,
                IsLegendary = speciesResponse?.IsLegendary ?? false
            };
        }
    }
}