using System.Collections.Generic;
using PocketMonsters.PokeApi;

namespace PocketMonsters
{
    public static class FlavorTextMapper
    {
        public static bool TryMap(IEnumerable<FlavorTextEntry> flavorTextEntries, out string flavorText)
        {
            flavorText = null;

            if (flavorTextEntries == null)
                return false;

            foreach(var entry in flavorTextEntries)
            {
                if(entry?.Language?.Name == "en")
                {
                    flavorText = entry.FlavorText.Replace("\n", " ").Trim();
                    return true;
                } 
            }

            return false;
        }
    }
}