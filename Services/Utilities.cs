using System.Web;

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
            
            // Return the relative path - the base href will handle the repository prefix
            return $"/deck/{encodedDeckName}";
        }
    }
} 