using System.Globalization;

public static class DateTimeUtility
{
    /// <summary>
    /// Returns the first day of the week that the specified
    /// date is in using the current culture. 
    /// </summary>
    public static DateTime GetFirstDayOfWeek(DateTime dayInWeek)
    {
        CultureInfo defaultCultureInfo = CultureInfo.CurrentCulture;
        return GetFirstDayOfWeek(dayInWeek, defaultCultureInfo);
    }

    /// <summary>
    /// Returns the first day of the week that the specified date 
    /// is in. 
    /// </summary>
    public static DateTime GetFirstDayOfWeek(DateTime dayInWeek, CultureInfo cultureInfo)
    {
        DayOfWeek firstDay = cultureInfo.DateTimeFormat.FirstDayOfWeek;
        DateTime firstDayInWeek = dayInWeek.Date;
        while (firstDayInWeek.DayOfWeek != firstDay)
            firstDayInWeek = firstDayInWeek.AddDays(-1);

        return firstDayInWeek;
    }
}