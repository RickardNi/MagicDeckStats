namespace MagicDeckStats.Services
{
    public static class Utilities
    {
        public static string FormatTotalPlayTime(double totalMinutes)
        {
            var totalHours = totalMinutes / 60.0;
            var days = (int)(totalHours / 24);
            var hours = (int)(totalHours % 24);
            
            if (days > 0)
            {
                return $"{days} day{(days == 1 ? "" : "s")}, {hours} hour{(hours == 1 ? "" : "s")}";
            }
            else
            {
                return $"{hours} hour{(hours == 1 ? "" : "s")}";
            }
        }
    }
} 