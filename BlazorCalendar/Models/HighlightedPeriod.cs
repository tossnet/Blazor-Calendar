namespace BlazorCalendar.Models;

/// <summary>
/// Defines recurrence pattern for highlighted periods
/// </summary>
public enum RecurrenceType
{
    None = 0,           // Une seule fois
    Yearly = 1,         // Chaque année (Noël, anniversaires)
    Monthly = 2,        // Chaque mois (payday, etc.)
    Weekly = 3          // Chaque semaine (réunion hebdo, etc.)
}

/// <summary>
/// Represents a date range to highlight with custom styling.
/// Supports single occurrences or recurring patterns.
/// </summary>
public sealed class HighlightedPeriod
{
    /// <summary>
    /// Start date (inclusive)
    /// </summary>
    public DateTime DateStart { get; set; }

    /// <summary>
    /// End date (inclusive). If null, only DateStart is highlighted (single day).
    /// </summary>
    public DateTime? DateEnd { get; set; }

    /// <summary>
    /// Type of recurrence (default: None = one-time only)
    /// </summary>
    public RecurrenceType RecurrenceType { get; set; } = RecurrenceType.None;

    /// <summary>
    /// Optional end date for the recurrence. 
    /// If null with RecurrenceType != None, repeats indefinitely.
    /// </summary>
    public DateTime? RecurrenceEndDate { get; set; } = null;

    /// <summary>
    /// Background color in any CSS format (hex, rgb, rgba, named colors)
    /// </summary>
    public string BackgroundColor { get; set; }

    /// <summary>
    /// Optional text color. If not set, uses the default FontColor.
    /// </summary>
    public string? FontColor { get; set; } = null;

    /// <summary>
    /// Optional label/description (e.g., "Vacances", "Birthday", "Christmas")
    /// </summary>
    public string? Label { get; set; } = null;

    /// <summary>
    /// Optional icon to display on highlighted days (emoji or text).
    /// Default: "📌" (pushpin)
    /// Examples: "★", "🎄", "🎂", "✓", etc.
    /// </summary>
    public string Icon { get; set; } = "📌";

    /// <summary>
    /// Determines if the given day falls within this highlighted period (considering recurrence)
    /// </summary>
    public bool ContainsDate(DateTime day)
    {
        var dayOnly = day.Date;
        var startOnly = DateStart.Date;
        var endOnly = DateEnd?.Date ?? startOnly;

        return RecurrenceType switch
        {
            RecurrenceType.None =>
                dayOnly >= startOnly && dayOnly <= endOnly,

            RecurrenceType.Yearly =>
                IsYearlyMatch(dayOnly, startOnly, endOnly),

            RecurrenceType.Monthly =>
                IsMonthlyMatch(dayOnly, startOnly, endOnly),

            RecurrenceType.Weekly =>
                IsWeeklyMatch(dayOnly, startOnly, endOnly),

            _ => false
        };
    }

    private bool IsYearlyMatch(DateTime day, DateTime start, DateTime end)
    {
        // Vérifie si la date correspond chaque année
        if (day.Month != start.Month || day.Day != start.Day)
            return false;

        // Vérifie les limites de récurrence
        if (RecurrenceEndDate != null && day.Year > RecurrenceEndDate.Value.Year)
            return false;

        if (day.Year < start.Year)
            return false;

        // Pour les périodes (ex: 25-26 décembre), vérifie que le jour est dans la plage
        int daysInPeriod = (end - start).Days;
        return day.DayOfYear >= start.DayOfYear &&
               day.DayOfYear <= (start.DayOfYear + daysInPeriod);
    }

    private bool IsMonthlyMatch(DateTime day, DateTime start, DateTime end)
    {
        // Vérifie si le jour du mois correspond
        if (day.Day != start.Day)
            return false;

        if (RecurrenceEndDate != null && day > RecurrenceEndDate.Value)
            return false;

        return day >= start;
    }

    private bool IsWeeklyMatch(DateTime day, DateTime start, DateTime end)
    {
        // Vérifie si c'est le même jour de la semaine
        if (day.DayOfWeek != start.DayOfWeek)
            return false;

        if (RecurrenceEndDate != null && day > RecurrenceEndDate.Value)
            return false;

        return day >= start;
    }

    /// <summary>
    /// Indicates if this is a single-day highlight
    /// </summary>
    public bool IsSingleDay => DateEnd == null || DateStart.Date == DateEnd.Value.Date;
}