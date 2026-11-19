using System.Globalization;

namespace BlazorCalendar.Helpers;

/// <summary>
/// Weekday header texts, from the most complete to the most compact.
/// The view picks the variant that fits the column width (see WeekView.razor.css).
/// </summary>
internal sealed class WeekDayTitle
{
    private const string DayFormat = "dd";
    private const string DayAndMonthFormat = "dd.MM";

    private readonly DateTime date;
    private readonly string format;
    private readonly CultureInfo culture;

    private WeekDayTitle(DateTime date, string format, CultureInfo culture)
    {
        this.date = date;
        this.format = format;
        this.culture = culture;

        Letter = FirstLetterOfDayName(date.DayOfWeek, culture);
        Full = date.ToString(format, culture);
        DayAndMonth = $"{Letter} {date.ToString(DayAndMonthFormat, culture)}";
        Day = $"{Letter} {date.ToString(DayFormat, culture)}";
    }

    /// <summary>The date formatted with the header format, e.g. "jeu. 19.11".</summary>
    public string Full { get; }

    /// <summary>Weekday letter, day and month, e.g. "J 19.11".</summary>
    public string DayAndMonth { get; }

    /// <summary>Weekday letter and day, e.g. "J 19".</summary>
    public string Day { get; }

    /// <summary>Uppercase first letter of the weekday, e.g. "J". Always displayed.</summary>
    public string Letter { get; }

    public static WeekDayTitle Create(DateTime date, string format, CultureInfo culture) =>
        new(date, format, culture);

    /// <summary>True when this title was built from the same inputs, so it can be reused.</summary>
    public bool IsFor(DateTime date, string format, CultureInfo culture) =>
        this.date == date
        && ReferenceEquals(this.culture, culture)
        && string.Equals(this.format, format, StringComparison.Ordinal);

    private static string FirstLetterOfDayName(DayOfWeek dayOfWeek, CultureInfo culture)
    {
        string name = culture.DateTimeFormat.GetAbbreviatedDayName(dayOfWeek);
        if (name.Length == 0) return string.Empty;

        int letterLength = StringInfo.GetNextTextElementLength(name);
        return culture.TextInfo.ToUpper(name[..letterLength]);
    }
}
