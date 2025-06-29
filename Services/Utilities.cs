using System.Web;
using MagicDeckStats.Models;

namespace MagicDeckStats.Services
{
    public static class Utilities
    {
        public static string FormatTotalPlayTime(int totalMinutes)
        {
            var hours = totalMinutes / 60;
            var minutes = totalMinutes % 60;
            
            if (hours > 0)
                return $"{hours}h {minutes}m";
            else
                return $"{minutes}m";
        }

        public static string GetDeckUrl(string deckName)
        {
            // URL encode the deck name to handle special characters like spaces, asterisks, etc.
            var encodedDeckName = HttpUtility.UrlEncode(deckName);
            
            // Return a relative path without leading slash to work with base href
            // This ensures it works correctly on GitHub Pages with /MagicDeckStats/ base href
            return $"deck/{encodedDeckName}";
        }

        public static string GetPlayerUrl(int playerId)
        {
            // Return a relative path without leading slash to work with base href
            // This ensures it works correctly on GitHub Pages with /MagicDeckStats/ base href
            return $"player/{playerId}";
        }

        public static string GetWinRateColorForDeck(double winRate) => winRate switch
        {
            > 60 => "text-success-dark",
            >= 45 => "text-success",
            >= 35 => "text-warning",
            _ => "text-danger"
        };

        public static string GetWinRateColorForPlayer(double winRate) => winRate switch
        {
            > 50 => "text-success",
            >= 35 => "text-warning",
            _ => "text-danger"
        };

        public static List<string> GetDeckTags(string deckName)
        {
            return deckName switch
            {
                "* Aura Overload v*"         => [],
                "* Blue Steal"               => [],
                "* Cascadia v1"              => [],
                "* Consuming Mutation v1"    => [],
                "* Elvish Ingenuity v1"      => [],
                "* Extremely Toxic v1"       => [],
                "* Fun Guys! v1"             => ["Finalist"],
                "* Genesis Rampage v1"       => [],
                "* Goblin Gang v2"           => [],
                "* Heat Wave v1"             => [],
                "* High Rollers v1"          => [],
                "* Land Masses v5"           => ["Archived"],
                "* Life Hack v1"             => [],
                "* Power Cycling v1"         => [],
                "* Pyramid Scheme v1"        => ["Archived"],
                "* Scrap Heap v1"            => [],
                "* Sinister Shrines v1"      => [],
                "* Snack Attack v1"          => [],
                "* Steadfast & Furious v1"   => ["Tour Winner"],
                "Adventure Time v1"          => ["Tour Winner"],
                "Aether Flux v11"            => ["Tour Winner"],
                "Blue Skies v1"              => [],
                "Chimera Flash v2"           => [],
                "Converging Domains v6"      => ["Finalist"],
                "Counter Culture v1"         => [],
                "Day Breaker v1"             => ["Tour Winner"],
                "Dino Might v1"              => [],
                "Domain Event v1"            => [],
                "Dragon Horde v8"            => ["Tour Winner"],
                "Eternal Harvest v1"         => ["Tour Winner"],
                "Fightin' Fish v2"           => ["Finalist"],
                "Firing Squad v1"            => [],
                "Gray Matter v1"             => ["Finalist"],
                "Green Giants v1"            => [],
                "Imperious Elves v4"         => ["Tour Winner"],
                "Karmageddon v3"             => ["Finalist"],
                "Knight Time v1"             => ["Tour Winner"],
                "Land Masses v1"             => ["Tour Winner"],
                "Licence to Mill v1"         => ["Archived"],
                "Lust for Life v2"           => [],
                "New Blood v1"               => [],
                "Night & Day v1"             => [],
                "Out of Hand v1"             => [],
                "Pilot Program v1"           => [],
                "Power Tools v1"             => [],
                "Pyramid Scheme v1.2"        => [],
                "Reanimaniacs v1"            => ["Tour Winner"],
                "Red Menace v8"              => [],
                "Rune Nation v1"             => [],
                "Second Wind v4"             => [],
                "Self Defense v1"            => ["Finalist"],
                "Shock Troupes v6"           => ["Tour Winner"],
                "Sphinx Control v3"          => ["Tour Winner"],
                "Target Practice v2"         => ["Tour Winner"],
                "Underworld Schemes v3"      => ["Finalist"],
                "Zombie Apocalypse v11"      => ["Tour Winner"],
                _                            => []
            };
        }
    }
} 