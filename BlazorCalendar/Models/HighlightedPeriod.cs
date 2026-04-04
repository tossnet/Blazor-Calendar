namespace BlazorCalendar.Models;

/// <summary>
/// Represents a date range to highlight with custom styling.
/// Can be a single day (DateEnd not set) or a period (DateEnd is set).
/// </summary>
public sealed class HighlightedPeriod
{
    /// <summary>
    /// Start date of the highlighted period (inclusive).
    /// </summary>
    public DateTime DateStart { get; set; }

    /// <summary>
    /// End date of the highlighted period (inclusive).
    /// If null, only DateStart is highlighted (single day).
    /// </summary>
    public DateTime? DateEnd { get; set; }

    /// <summary>
    /// Background color in any CSS format (hex, rgb, rgba, named colors).
    /// </summary>
    public string BackgroundColor { get; set; }

    /// <summary>
    /// Optional text color. If not set, uses the calendar's default FontColor.
    /// </summary>
    public string? FontColor { get; set; }

    /// <summary>
    /// Optional label/description (e.g., "Vacances", "Birthday", "Project Alpha").
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// Indicates if this is a single-day highlight.
    /// </summary>
    public bool IsSingleDay => DateEnd == null || DateStart.Date == DateEnd.Value.Date;

    /// <summary>
    /// Determines if the given day falls within this highlighted period.
    /// </summary>
    public bool ContainsDate(DateTime day)
    {
        var dayOnly = day.Date;
        var startOnly = DateStart.Date;
        var endOnly = DateEnd?.Date ?? startOnly;

        return dayOnly >= startOnly && dayOnly <= endOnly;
    }
}
