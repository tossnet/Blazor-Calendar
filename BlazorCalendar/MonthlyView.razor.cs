using BlazorCalendar.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorCalendar;

partial class MonthlyView : CalendarBase
{
    private DateTime _firstdate;
    [CascadingParameter(Name = "FirstDate")]
    public DateTime FirstDate
    {
        get
        {
            if (_firstdate == DateTime.MinValue) _firstdate = DateTime.Today;
            return _firstdate.Date;
        }
        set
        {
            _firstdate = value;
        }
    }


    [CascadingParameter(Name = "TasksList")]
    public Tasks[]? TasksList { get; set; }

    [Parameter]
    public EventCallback<int> OutsideCurrentMonthClick { get; set; }


    private async Task HandleClickOutsideCurrentMonthClick(int AddMonth)
    {
        await OutsideCurrentMonthClick.InvokeAsync(AddMonth);
    }
}
