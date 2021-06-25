using System.Threading.Tasks;
using LanguageExt;
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
            result.ShouldBe(Option<PokemonDetails>.None);
        }
    }
}