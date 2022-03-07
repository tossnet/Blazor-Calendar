using Blazor_Calendar.Models;
using Microsoft.AspNetCore.Components;

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

        private DateTime m = DateTime.Today;
        private DateTime day = default;


    }
}