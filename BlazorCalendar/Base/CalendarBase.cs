using Microsoft.AspNetCore.Components;

namespace BlazorCalendar;

public abstract class CalendarBase : ComponentBase
{
    /// <summary>
    /// User class names, separated by space. Applied on top of the component's own classes
    /// </summary>
    [Parameter]
    public string Class { get; set; }

    /// <summary>
    /// User styles, applied on top of the component's own styles.
    /// </summary>
    [Parameter]
    public string Style { get; set; }

    /// <summary>
    /// User styles of column headers, applied on top of the component's own  styles.
    /// </summary>
    [Parameter]
    public string HeaderStyle { get; set; }

    /// <summary>
    /// Allows the user to move the tasks
    /// </summary>
    [Parameter]
    public bool Draggable { get; set; } = false;
}
