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

        public static string GetWinRateColor(double winRatePercentage) => winRatePercentage switch
        {
            > 60 => "text-success-dark",
            >= 45 => "text-success",
            >= 35 => "text-warning",
            _ => "text-danger"
        };
    }
} 