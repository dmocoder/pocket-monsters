using System;
using PocketMonsters.PokeDex;
using PocketMonsters.PokeDex.PokeApi;
using Shouldly;
using Xunit;

namespace PocketMonsters.Tests
{
    [Trait("Category", "Unit")]
    public class FlavorTextMapperShould
    {
        [Fact]
        public void RemoveNewLines()
        {
            //setup
            var flavorTextEntry = new FlavorTextEntry
            {
                FlavorText = "i am\nacross many\nlines",
                Language = new Link {Name = "en"}
            };

            //act & assert
            FlavorTextMapper.TryMap(new[] {flavorTextEntry}, out var flavorText).ShouldBeTrue();
            flavorText.ShouldBe("i am across many lines");
        }

        [Fact]
        public void ReplaceFeedWithSpace()
        {
            //setup
            var flavorTextEntry = new FlavorTextEntry
            {
                FlavorText = "i am\nacross many\ffeeds",
                Language = new Link {Name = "en"}
            };

            //act & assert
            FlavorTextMapper.TryMap(new[] {flavorTextEntry}, out var flavorText).ShouldBeTrue();
            flavorText.ShouldBe("i am across many feeds");
        }

        [Fact]
        public void NotMap_WhenNoEnglishFlavorTextAvailable()
        {
            //setup
            var frenchFlavorTextEntry = new FlavorTextEntry
            {
                FlavorText = "vrai vrai vrai",
                Language = new Link {Name = "fr"}
            };

            var germanTextEntry = new FlavorTextEntry
            {
                FlavorText = "ja ja ja",
                Language = new Link {Name = "dk"}
            };

            //act & assert
            FlavorTextMapper.TryMap(new[] {frenchFlavorTextEntry, germanTextEntry}, out var _)
                .ShouldBeFalse();
        }

        [Fact]
        public void NotMap_WhenNullFlavorTextProvided()
        {
            //setup, act & assert
            FlavorTextMapper.TryMap(null, out var _).ShouldBeFalse();
        }

        [Fact]
        public void NotMap_WhenEmptyFlavorTextProvided()
        {
            //setup, act & assert
            FlavorTextMapper.TryMap(Array.Empty<FlavorTextEntry>(), out var _).ShouldBeFalse();
        }
    }
}