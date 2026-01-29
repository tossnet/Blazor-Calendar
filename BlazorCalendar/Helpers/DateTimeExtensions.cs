using System.Globalization;

namespace BlazorCalendar.Helpers;

internal static class DateTimeExtensions
{
    public static bool HasTime(this DateTime value)
    {
        return value.Hour + value.Minute > 0;
    }

    public static string GetFormatedHour(this DateTime value)
    {
        return value.ToString("t", CultureInfo.CurrentCulture);
    }


}
