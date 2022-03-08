namespace Blazor_Calendar.Models;

public class Tasks
{
    public int ID { get; set; }
    public string Caption { get; set; }
    public string Code { get; set; }
    public string Color { get; set; }
    public string ForeColor { get; set; }
    public string Comment { get; set; }
    public DateTime DateStart { get; set; }
    public DateTime DateEnd { get; set; }
}
