using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace PocketMonsters
{
    [ApiController]
    [Route("[controller]")]
    public class PokemonController : ControllerBase
    {
        IPokeDexService _pokeDexService;

        public PokemonController(IPokeDexService pokeDexService)
        {
            _pokeDexService = pokeDexService ?? throw new ArgumentNullException(nameof(pokeDexService));
        }

        [HttpGet("{pokemonName}")]
        public async Task<IActionResult> GetPokemon([FromRoute] string pokemonName)
        {
            //TODO: Add validation for name i.e. force to lowercase, return 422
            switch(await _pokeDexService.GetPokemonDetails(pokemonName))
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