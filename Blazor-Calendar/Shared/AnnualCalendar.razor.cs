using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using Blazor_Calendar;
using Blazor_Calendar.Shared;

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

        private DateTime m = DateTime.Today;
        private DateTime day = default;


    }
}