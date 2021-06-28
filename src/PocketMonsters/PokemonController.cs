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
        IPokeDexService _pokeDexService;
        private static Regex _pokemonRegex = new Regex("^[A-Za-z-]+$", RegexOptions.Compiled); 

        public PokemonController(IPokeDexService pokeDexService)
        {
            _pokeDexService = pokeDexService ?? throw new ArgumentNullException(nameof(pokeDexService));
        }

        [HttpGet("{pokemonName}")]
        public async Task<IActionResult> GetPokemon([FromRoute] string pokemonName)
        {
            if(!_pokemonRegex.IsMatch(pokemonName))
                return new BadRequestObjectResult("Pokemon name invalid");

            switch(await _pokeDexService.GetPokemonDetails(pokemonName.ToLowerInvariant()))
            {
                case PokemonNotFound notFound:
                    return new NotFoundResult();
                case PokemonDetails details:
                    return new OkObjectResult(details);
            }

            return new NotFoundResult();    
        }
    }
}