namespace BlazorCalendar.Models;

public class ClickTaskParameter
{
    public List<int> IDList { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
}

public class ClickEmptyDayParameter
{
    public DateTime Day { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
}