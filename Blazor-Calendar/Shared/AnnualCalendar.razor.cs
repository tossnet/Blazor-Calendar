using Blazor_Calendar.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Blazor_Calendar.Shared
{
    public partial class AnnualCalendar
    {
        [Parameter] public int Months { get; set; } = 11;

        private DateTime _firstdate;
        [Parameter] public DateTime FirstDate
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
}