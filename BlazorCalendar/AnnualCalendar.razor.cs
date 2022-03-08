using BlazorCalendar.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorCalendar;

public partial class AnnualCalendar
{
    private int _months = 11;
    [Parameter]
    public int Months
    {
        get
        {
            return _months;
        }
        set
        {
            _months = value;
            if (_months > 1) _months--;
        }
    }

    private DateTime _firstdate;
    [Parameter]
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
    [Parameter] public int HeigthWindow { get; set; } = 500;
    [Parameter] public int WidthWindow { get; set; } = 800;
    [Parameter] public Tasks[] TasksList { get; set; }
    [Parameter] public EventCallback<ClickTaskParameter> TaskClick { get; set; }

    private DateTime m = DateTime.Today;
    private DateTime day = default;

    private async Task TaskClickInternal(MouseEventArgs e, DateTime day)
    {
        // There can be several tasks in one day :
        List<int> listID = new();
        for (var k = 0; k < TasksList.Length; k++)
        {
            Tasks t = TasksList[k];

            if (t.DateStart.Date <= day.Date && day.Date <= t.DateEnd.Date)
            {
                listID.Add(t.ID);
            }
        }

        if (listID.Count > 0)
        {
            ClickTaskParameter clickTaskParameter = new()
            {
                IDList = listID,
                X = e.ClientX,
                Y = e.ClientY
            };

            await TaskClick.InvokeAsync(clickTaskParameter);
        }
    }
}
