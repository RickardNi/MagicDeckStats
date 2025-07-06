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
                "Aura Overload v*"         => ["Not Owned", "Arena", "Custom"],
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
                "Licence to Mill v1"       => ["Archived"],
                "Licence to Mill v2.1"     => ["Archived", "Custom"],
                "Licence to Mill v2.2"     => ["Archived", "Custom"],
                "Life Hack v1"             => ["Not Owned", "Arena", "B"],
                "Night & Day v1"           => ["Not Owned", "Arena"],
                "Power Cycling v1"         => ["Not Owned", "M"],
                "Pyramid Scheme v1"        => ["Archived"],
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
                "Lust for Life v2"           => [],
                "New Blood v1"               => [],
                "Out of Hand v1"             => [],
                "Pilot Program v1"           => ["B"],
                "Power Tools v1"             => [],
                "Pyramid Scheme v1.2"        => ["Custom"],
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

        public static List<string> GetAllArenaDecks()
        {
            return new List<string>
            {
                "Adventure Time v4",
                "Aether Ambush v1",
                "Aether Ambush v2",
                "Aether Ambush v3",
                "Aether Ambush v4",
                "Altered Beasts v1",
                "Altered Beasts v2",
                "Altered Beasts v3",
                "Altered Beasts v4",
                "Amped Up v3",
                "Aura Overload v3",
                "Aura Overload v7",
                "Aura Overload v8",
                "Bad Omens v1",
                "Bad Omens v2",
                "Bad Omens v3",
                "Bestial Blink v2",
                "Blood Brothers V17",
                "Bombs Away v8",
                "Bone Crushers v1",
                "Bone Crushers v2",
                "Bone Crushers v3",
                "Counter Culture v1",
                "Counter Culture v3",
                "Counter Culture V10",
                "Critical Mass v1",
                "Critical Mass v2",
                "Devil's Advocate v4",
                "Devil's Advocate v6",
                "Devil's Advocate v7",
                "Dino Might v1",
                "Dino Might v2",
                "Dino Might v3",
                "Dino Might v4",
                "Dino Might v5",
                "Dino Might v6",
                "Dino Might v7",
                "Dino Might v8",
                "Dino Might v9",
                "Dino Might V10",
                "Dino Might V12",
                "Dino Might V14",
                "Dino Might V15",
                "Dino Might V16",
                "Dino Might V18",
                "Dino Might V19",
                "Dino Might V20",
                "Domain Event v2",
                "Extremely Toxic v1",
                "Fightin' Fish v2",
                "Fightin' Fish v3",
                "Fightin' Fish v5",
                "Fightin' Fish v6",
                "Fightin' Fish v9",
                "Flight Club v4",
                "Food Fighters v3",
                "Food Fighters v4",
                "Food Fighters v5",
                "Food Fighters v6",
                "Gear Heads v3",
                "Gear Heads v4",
                "Genesis Rampage v5",
                "Goblin Gang v1",
                "Goblin Gang v2",
                "Goblin Gang v3",
                "Goblin Gang v4",
                "Goblin Gang v5",
                "Goblin Gang v6",
                "Goblin Gang v7",
                "Grisly Graverobbers v4",
                "Grisly Graverobbers V15",
                "Hatching Plans v1",
                "Heat Wave v1",
                "Heat Wave v2",
                "Heroes of Old v1",
                "Heroes of Old v2",
                "Heroes of Old v3",
                "Heroes of Old v4",
                "Heroes of Old v5",
                "Heroes of Old v6",
                "Heroes of Old v9",
                "Heroes of Old V10",
                "Heroes of Old V11",
                "Heroes of Old V13",
                "Heroes of Old V14",
                "High Life v1",
                "High Life v2",
                "High Life v3",
                "High Life v4",
                "History Lesson v1",
                "Hit the Books v1",
                "Hit the Books v2",
                "Hit the Books v3",
                "Hit the Books v4",
                "Hit the Books v5",
                "Kicking & Screaming v1",
                "Knight Time v1",
                "Knight Time v2",
                "Knight Time v3",
                "Knight Time v4",
                "Last Rites v1",
                "Last Rites v2",
                "Life Hack v1",
                "Life Hack v2",
                "Life Hack v3",
                "Life Hack v6",
                "Light Brigade v2",
                "Light Brigade v3",
                "Light Brigade v4",
                "Light Brigade v7",
                "Light Brigade V10",
                "Mighty Mites v1",
                "Monumental Force v1",
                "New Blood V12",
                "Night & Day v1",
                "Night Crawlers v1",
                "Night Crawlers v2",
                "Oil Barons v1",
                "Raiding Party v1",
                "Raiding Party v2",
                "Raiding Party v3",
                "Scrap Heap v1",
                "Search Party v1",
                "Search Party v2",
                "Snack Attack v1",
                "Spell Check v2",
                "Spell Check v3",
                "Spell Check v4",
                "Spell Check v5",
                "Spell Check v6",
                "Spell Check v7",
                "Spell Check v8",
                "Super Conductors v1",
                "Super Conductors v2",
                "Super Conductors v3",
                "Super Conductors v4",
                "Super Conductors v5",
                "Techno Beats v1"
            };
        }
    }
} 