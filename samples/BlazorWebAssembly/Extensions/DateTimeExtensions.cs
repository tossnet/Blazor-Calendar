namespace BlazorWebAssembly.Extensions;

public static class DateTimeExtensions
{ 
    /// <summary>
    ///     DateTime previousDateTime = new DateTime(2024, 1, 15, 14, 30, 0);; //Our DateTime
    ///     DateTime newDate = new DateTime(2024, 2, 20, 9, 15, 0);
    ///     DateTime result = previousDateTime.KeepTime(newDate);
    ///     // result = 2024-02-20 14:30:00 (date of newDate, hours of previousDateTime)
    /// </summary>
    public static DateTime KeepTime(this DateTime dateTime, DateTime newDate)
    {
        return new DateTime(newDate.Year, newDate.Month, newDate.Day, dateTime.Hour, dateTime.Minute, 0, 0, dateTime.Kind);
    }
}
