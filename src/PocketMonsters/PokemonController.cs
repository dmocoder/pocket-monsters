using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace PocketMonsters
{
    [ApiController]
    [Route("[controller]")]
    public class PokemonController : ControllerBase
    {
        private readonly IPokeDexService _pokeDexService;
        private readonly IPokemonTranslationService _pokemonTranslationService;
        private static readonly Regex PokemonRegex = new Regex("^[A-Za-z-]+$", RegexOptions.Compiled);

        public PokemonController(IPokeDexService pokeDexService, IPokemonTranslationService pokemonTranslationService)
        {
            _pokeDexService = pokeDexService ?? throw new ArgumentNullException(nameof(pokeDexService));
            _pokemonTranslationService = pokemonTranslationService ?? throw new ArgumentNullException(nameof(pokemonTranslationService));
        }

        [HttpGet("{pokemonName}")]
        public async Task<IActionResult> GetPokemon([FromRoute] string pokemonName)
        {
            if (!PokemonRegex.IsMatch(pokemonName))
                return new BadRequestObjectResult("Pokemon name invalid");

            switch (await _pokeDexService.GetPokemonDetails(pokemonName.ToLowerInvariant()))
            {
                case PokemonNotFound notFound:
                    return new NotFoundResult();
                case PokemonDetails details:
                    return new OkObjectResult(details);
            }

            return new NotFoundResult();
        }

        [HttpGet("translated/{pokemonName}")]
        public async Task<IActionResult> GetTranslatedPokemon([FromRoute] string pokemonName)
        {
            if (!PokemonRegex.IsMatch(pokemonName))
                return new BadRequestObjectResult("Pokemon name invalid");

            if (await _pokeDexService.GetPokemonDetails(pokemonName.ToLowerInvariant()) is PokemonDetails pokemonDetails)
            {
                var translatedDescription =
                    await _pokemonTranslationService.TranslatePokemonDescription(pokemonDetails.Description, pokemonDetails.Habitat, pokemonDetails.IsLegendary);
                
                return new OkObjectResult(MapTranslated(pokemonDetails, translatedDescription));
            }

            return new NotFoundResult();
        }

        private PokemonDetails MapTranslated(PokemonDetails originalDetails, string translatedDescription)
        {
            return new PokemonDetails
            {
                Name = originalDetails.Name,
                Description = translatedDescription,
                Habitat = originalDetails.Habitat,
                IsLegendary = originalDetails.IsLegendary
            };
        }
    }
}