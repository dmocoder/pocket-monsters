using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using PocketMonsters.PokeDex;

namespace PocketMonsters
{
    [ApiController]
    [Route("[controller]")]
    public class PokemonController : ControllerBase
    {
        private readonly IPokeDexService _pokeDexService;
        private readonly IPokemonTranslationService _pokemonTranslationService;
        private readonly ILogger<PokemonController> _logger;
        private static readonly Regex PokemonRegex = new("^[A-Za-z-]+$", RegexOptions.Compiled);

        public PokemonController(IPokeDexService pokeDexService, IPokemonTranslationService pokemonTranslationService, ILogger<PokemonController> logger)
        {
            _pokeDexService = pokeDexService ?? throw new ArgumentNullException(nameof(pokeDexService));
            _pokemonTranslationService = pokemonTranslationService ?? throw new ArgumentNullException(nameof(pokemonTranslationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("{pokemonName}")]
        public async Task<IActionResult> GetPokemon([FromRoute] string pokemonName)
        {
            _logger.LogInformation("Handing GetPokemon request for {Pokemon}", pokemonName);
            
            if (!PokemonRegex.IsMatch(pokemonName))
                return new BadRequestObjectResult("Pokemon name invalid");

            var detailsResponse = await _pokeDexService.GetPokemonDetails(pokemonName);
            
            if(detailsResponse is PokemonDetails pokemonDetails)
                return new OkObjectResult(new Pokemon(pokemonDetails));

            //ideally a failure such as this would return something more descriptive or a failed/success flag in the body
            //however this might require adapting the happy-path body in the spec
            if (detailsResponse is GetPokemonDetailsFailed)
                return StatusCode(503); 

            return new NotFoundResult();
        }

        [HttpGet("translated/{pokemonName}")]
        public async Task<IActionResult> GetTranslatedPokemon([FromRoute] string pokemonName)
        {
            _logger.LogInformation("Handing GetTranslatedPokemon request for {Pokemon}", pokemonName);
            
            if (!PokemonRegex.IsMatch(pokemonName))
                return new BadRequestObjectResult("Pokemon name invalid");

            if (await _pokeDexService.GetPokemonDetails(pokemonName) is PokemonDetails pokemonDetails)
            {
                var translatedDescription =
                    await _pokemonTranslationService.TranslatePokemonDescription(new Pokemon(pokemonDetails));
                
                return new OkObjectResult(MapTranslated(pokemonDetails, translatedDescription));
            }

            return new NotFoundResult();
        }

        private PokemonDetails MapTranslated(PokemonDetails originalDetails, string translatedDescription)
        {
            return new()
            {
                Name = originalDetails.Name,
                Description = translatedDescription,
                Habitat = originalDetails.Habitat,
                IsLegendary = originalDetails.IsLegendary
            };
        }
    }
}