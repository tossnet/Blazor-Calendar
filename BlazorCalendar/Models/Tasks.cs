using System.Diagnostics;

namespace BlazorCalendar.Models;

[DebuggerDisplay("{ID} {Code} {DateStart}")]
public class Tasks
{
    public int ID { get; set; }
    public string Caption { get; set; }
    public string Code { get; set; }
    public string Color { get; set; }
    public string? ForeColor { get; set; } = null;
    public string? Comment { get; set; } = null;
    public DateTime DateStart { get; set; }
    public DateTime DateEnd { get; set; }
    public bool NotBeDraggable { get; set; }
}
