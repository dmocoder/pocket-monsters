using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PocketMonsters.PokeDex;
using PocketMonsters.PokemonTranslation;

namespace PocketMonsters
{
    [ApiController]
    [Route("[controller]")]
    public class PokemonController : ControllerBase
    {
        private static readonly Regex PokemonRegex = new("^[A-Za-z-]+$", RegexOptions.Compiled);
        private readonly ILogger<PokemonController> _logger;
        private readonly IPokeDexService _pokeDexService;
        private readonly IPokemonTranslationService _pokemonTranslationService;

        public PokemonController(IPokeDexService pokeDexService, IPokemonTranslationService pokemonTranslationService,
            ILogger<PokemonController> logger)
        {
            _pokeDexService = pokeDexService ?? throw new ArgumentNullException(nameof(pokeDexService));
            _pokemonTranslationService = pokemonTranslationService ??
                                         throw new ArgumentNullException(nameof(pokemonTranslationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        ///     Given a Pokemon name, returns standard Pokemon description and other basic information
        /// </summary>
        /// <param name="pokemonName"></param>
        /// <returns>Pokemon Details</returns>
        /// <response code="200">Returns the Pokemon Details</response>
        /// <response code="404">If the supplied Pokemon name cannot be identified</response>
        /// <response code="400">If the supplied Pokemon name is invalid</response>
        /// <response code="503">If the dependent services used by the API are unavailable</response>
        [Produces("application/json")]
        [ProducesResponseType(typeof(Pokemon), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        [HttpGet("{pokemonName}")]
        public async Task<IActionResult> GetPokemon([FromRoute] string pokemonName)
        {
            _logger.LogInformation("Handing GetPokemon request for {Pokemon}", pokemonName);

            if (!PokemonRegex.IsMatch(pokemonName))
                return new BadRequestObjectResult("Pokemon name invalid");

            var detailsResponse = await _pokeDexService.GetPokemonDetails(pokemonName);

            if (detailsResponse is PokemonDetails pokemonDetails)
                return new OkObjectResult(new Pokemon(pokemonDetails));

            //ideally a failure such as this would return something more descriptive or a failed/success flag in the body
            //however this might require adapting the happy-path body in the spec
            if (detailsResponse is GetPokemonDetailsFailed)
                return StatusCode(503);

            return new NotFoundObjectResult($"{pokemonName} could not be found");
        }

        /// <summary>
        ///     Given a Pokemon name, returns translated Pokemon description and other basic information
        /// </summary>
        /// <param name="pokemonName"></param>
        /// <returns>Pokemon Details</returns>
        /// <response code="200">Returns the Translated Pokemon Details</response>
        /// <response code="404">If the supplied Pokemon name cannot be identified</response>
        /// <response code="400">If the supplied Pokemon name is invalid</response>
        /// <response code="503">If the dependent services used by the API are unavailable</response>
        [Produces("application/json")]
        [ProducesResponseType(typeof(Pokemon), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        [HttpGet("translated/{pokemonName}")]
        public async Task<IActionResult> GetTranslatedPokemon([FromRoute] string pokemonName)
        {
            _logger.LogInformation("Handing GetTranslatedPokemon request for {Pokemon}", pokemonName);

            if (!PokemonRegex.IsMatch(pokemonName))
                return new BadRequestObjectResult("Pokemon name invalid");

            var detailsResponse = await _pokeDexService.GetPokemonDetails(pokemonName);

            if (detailsResponse is PokemonDetails pokemonDetails)
            {
                var translatedDescription =
                    await _pokemonTranslationService.TranslatePokemonDescription(new Pokemon(pokemonDetails));

                return new OkObjectResult(MapTranslated(pokemonDetails, translatedDescription));
            }

            if (detailsResponse is GetPokemonDetailsFailed)
                return StatusCode(503);

            return new NotFoundObjectResult($"{pokemonName} could not be found");
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