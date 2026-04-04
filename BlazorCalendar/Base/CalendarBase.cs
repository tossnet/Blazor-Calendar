using BlazorCalendar.Models;
using Microsoft.AspNetCore.Components;

namespace BlazorCalendar;

public abstract class CalendarBase : ComponentBase
{
    /// <summary>
    /// User class names, separated by space. Applied on top of the component's own classes
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// User styles, applied on top of the component's own styles.
    /// </summary>
    [Parameter]
    public string? Style { get; set; }

    /// <summary>
    /// User styles of column headers, applied on top of the component's own  styles.
    /// </summary>
    [Parameter]
    public string? HeaderStyle { get; set; }

    /// <summary>
    /// Allows the user to move the tasks
    /// </summary>
    [Parameter]
    public bool Draggable { get; set; } = false;

    /// <summary>
    /// Allows the user to change the background color of empty days
    /// </summary>
    [Parameter]
    public string WeekDaysColor { get; set; } = "#FFF";

    /// <summary>
    /// Allows the user to change the saturday background color
    /// </summary>
    [Parameter]
    public string SaturdayColor { get; set; } = "#ECF4FD";

    /// <summary>
    /// Allows the user to change the sunday background color
    /// </summary>
    [Parameter]
    public string SundayColor { get; set; } = "#DBE7F8";

    /// <summary>
    /// Allows the user to change the disabled day background color
    /// </summary>
    [Parameter]
    public string DisabledDayColor { get; set; } = "#f4f6f7";

    [Parameter]
    public string FontColor { get; set; } = "#6d7377";

    /// <summary>
    /// List of periods or single days with custom background colors (vacations, holidays, special events, etc.)
    /// </summary>
    [Parameter]
    public HighlightedPeriod[]? HighlightedPeriods { get; set; }

    public string GetBackground(DateTime day)
    {
        int d = (int)day.DayOfWeek;

        if (d == 6)
        {
            return $"background:{SaturdayColor};color:{FontColor}";
        }
        else if (d == 0)
        {
            return $"background:{SundayColor};color:{FontColor}";
        }

        return $"background:{WeekDaysColor};color:{FontColor}";
    }

    /// <summary>
    /// Gets the background style for a specific day, applying highlighted period styling if the day
    /// falls within any of the <see cref="HighlightedPeriods"/>. Falls back to the default day background.
    /// The first matching period wins when periods overlap.
    /// </summary>
    public string GetBackgroundWithHighlight(DateTime day)
    {
        if (HighlightedPeriods != null)
        {
            foreach (var period in HighlightedPeriods)
            {
                if (period.ContainsDate(day))
                {
                    string color = period.FontColor ?? FontColor;
                    return $"background:{period.BackgroundColor};color:{color}";
                }
            }
        }

        return GetBackground(day);
    }

    public HighlightedPeriod? GetHighlightPeriod(DateTime day)
    {
        if (HighlightedPeriods == null)
            return null;

        return HighlightedPeriods.FirstOrDefault(h => h.ContainsDate(day));
    }

    public string? GetHighlightLabel(DateTime day)
    {
        return GetHighlightPeriod(day)?.Label;
    }
}
