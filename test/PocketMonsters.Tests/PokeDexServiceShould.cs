using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace PocketMonsters.Tests
{
    public class PokeDexServiceShould
    {
        [Fact]
        public async Task Returns_IfPokemonDoesNotExist()
        {
            var pokeDexService = new PokeDexService();
            var result = await pokeDexService.GetPokemonDetails("fake-mon");
            result.ShouldBeNull();
        }
    }
}