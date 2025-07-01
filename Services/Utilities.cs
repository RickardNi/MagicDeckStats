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

        public static string FormatWinRate(double winRate)
        {
            if (winRate == 0 || winRate == 1)
                return winRate.ToString("P0");
            else
                return winRate.ToString("P1");
        }

        public static string GetWinRateColorForDeck(double winRate) => winRate switch
        {
            >= 0.6 => "text-success-dark",
            >= 0.45 => "text-success",
            >= 0.40 => "text-warning",
            >= 0.35 => "text-mediocre",
            _ => "text-danger"
        };

        public static string GetWinRateColorForPlayer(double winRate) => winRate switch
        {
            >= 0.6 => "text-success-dark",
            >= 0.5 => "text-success",
            >= 0.4 => "text-warning",
            >= 0.3 => "text-mediocre",
            _ => "text-danger"
        };

        public static List<string> GetDeckTags(string deckName)
        {
            return deckName switch
            {
                "Aura Overload v*"         => ["Not Owned", "Arena"],
                "Blue Steal v1"            => ["Not Owned", "M"],
                "Cascadia v1"              => ["Not Owned", "M"],   
                "Consuming Mutation v1"    => ["Not Owned", "M"],
                "Elvish Ingenuity v1"      => ["Not Owned", "Arena"],
                "Extremely Toxic v1"       => ["Not Owned", "Arena"],
                "Fun Guys! v1"             => ["Not Owned", "Finalist"],
                "Genesis Rampage v1"       => ["Not Owned", "M"],
                "Goblin Gang v2"           => ["Not Owned", "Arena", "M"],
                "Heat Wave v1"             => ["Not Owned", "Arena"],
                "High Rollers v1"          => ["Not Owned", "M", "B"],
                "Land Masses v5"           => ["Not Owned", "Archived"],
                "Life Hack v1"             => ["Not Owned", "Arena", "B"],
                "Night & Day v1"           => ["Not Owned", "Arena"],
                "Power Cycling v1"         => ["Not Owned", "M"],
                "Pyramid Scheme v1"        => ["Not Owned", "Archived"],
                "Scrap Heap v1"            => ["Not Owned", "Arena"],
                "Sinister Shrines v1"      => ["Not Owned", "M"],
                "Snack Attack v1"          => ["Not Owned", "M", "Arena"],
                "Steadfast & Furious v1"   => ["Not Owned", "Tour Winner"],

                "Adventure Time v1"          => ["Tour Winner"],
                "Aether Flux v11"            => ["Tour Winner"],
                "Blue Skies v1"              => [],
                "Chimera Flash v2"           => [],
                "Converging Domains v6"      => ["Finalist"],
                "Counter Culture v1"         => ["Arena"],
                "Day Breaker v1"             => ["Tour Winner"],
                "Dino Might v1"              => ["Arena", "B"],
                "Domain Event v1"            => [],
                "Dragon Horde v8"            => ["Tour Winner"],
                "Eternal Harvest v1"         => ["Tour Winner"],
                "Fightin' Fish v2"           => ["Finalist", "Arena"],
                "Firing Squad v1"            => [],
                "Gray Matter v1"             => ["Finalist"],
                "Green Giants v1"            => [],
                "Imperious Elves v4"         => ["Tour Winner"],
                "Karmageddon v3"             => ["Finalist"],
                "Knight Time v1"             => ["Tour Winner", "Arena"],
                "Land Masses v1"             => ["Tour Winner"],
                "Licence to Mill v1"         => ["Archived"],
                "Lust for Life v2"           => [],
                "New Blood v1"               => [],
                "Out of Hand v1"             => [],
                "Pilot Program v1"           => ["B"],
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