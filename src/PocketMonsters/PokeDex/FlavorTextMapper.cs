using System.Collections.Generic;
using PocketMonsters.PokeDex.PokeApi;

namespace PocketMonsters.PokeDex
{
    public static class FlavorTextMapper
    {
        /// <summary>
        ///     Maps the flavor text entries returned by PokeApi
        /// </summary>
        /// <param name="flavorTextEntries"></param>
        /// <param name="flavorText"></param>
        /// <returns>English language flavo(u)r text with escaped characters removed</returns>
        public static bool TryMap(IEnumerable<FlavorTextEntry> flavorTextEntries, out string flavorText)
        {
            flavorText = null;

            if (flavorTextEntries == null)
                return false;

            foreach (var entry in flavorTextEntries)
                if (entry?.Language?.Name == "en")
                {
                    flavorText = entry.FlavorText
                        .Replace("\n", " ")
                        .Replace("\f", " ")
                        .Trim();
                    return true;
                }

            return false;
        }
    }
}