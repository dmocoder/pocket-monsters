using Microsoft.AspNetCore.Mvc;

namespace PocketMonsters
{
    [ApiController]
    [Route("[controller]")]
    public class PokemonController : ControllerBase
    {
        public PokemonController()
        {
            
        }
        
        [HttpGet]
        public IActionResult GetPokemon([FromRoute] string pokemonName)
        {
            return new NotFoundResult();
        }
    }
}