using BlazorCalendar.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorCalendar;

partial class MonthlyView : CalendarBase
{
    [CascadingParameter(Name = "SelectedView")]
    public DisplayedView DisplayedView { get; set; } = DisplayedView.Monthly;

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
    public PriorityLabel PriorityDisplay { get; set; } = PriorityLabel.Code;

    [Parameter]
    public EventCallback<int> OutsideCurrentMonthClick { get; set; }

    [Parameter]
    public EventCallback<ClickEmptyDayParameter> DayClick { get; set; }

    [Parameter]
    public EventCallback<ClickTaskParameter> TaskClick { get; set; }

    [Parameter]
    public EventCallback<DragDropParameter> DragStart { get; set; }

    [Parameter]
    public EventCallback<DragDropParameter> DropTask { get; set; }

    private Tasks? TaskDragged;
    private TaskPosition[] occupiedPosition = new TaskPosition[32];

    protected override void OnInitialized()
    {
        
    }

    private async Task HandleClickOutsideCurrentMonthClick(int AddMonth)
    {
        await OutsideCurrentMonthClick.InvokeAsync(AddMonth);
    }


    private async Task ClickTaskInternal(MouseEventArgs e, int taskID, DateTime day)
    {
        List<int> listID = new();
        listID.Add(taskID);

        ClickTaskParameter clickTaskParameter = new()
        {
            IDList = listID,
            X = e.ClientX,
            Y = e.ClientY,
            Day = day
        };

        await TaskClick.InvokeAsync(clickTaskParameter);
    }

    private async Task ClickAllDayInternal(MouseEventArgs e, DateTime day)
    {
        if (day == default) return;

        // There can be several tasks in one day :
        List<int> listID = new();
        if (TasksList != null)
        {
            for (var k = 0; k < TasksList.Length; k++)
            {
                Tasks t = TasksList[k];

                if (t.DateStart.Date <= day.Date && day.Date <= t.DateEnd.Date)
                {
                    listID.Add(t.ID);
                }
            }

            ClickTaskParameter clickTaskParameter = new()
            {
                IDList = listID,
                X = e.ClientX,
                Y = e.ClientY,
                Day = day
            };

            await TaskClick.InvokeAsync(clickTaskParameter);
        }
    }

    private async Task ClickDayInternal(MouseEventArgs e, DateTime day)
    {
        ClickEmptyDayParameter clickEmptyDayParameter = new()
        {
            Day = day,
            X = e.ClientX,
            Y = e.ClientY
        };

        await DayClick.InvokeAsync(clickEmptyDayParameter);
    }

    private async Task HandleDragStart(int taskID)
    {
        TaskDragged = new Tasks()
        {
            ID = taskID
        };

        DragDropParameter dragDropParameter = new()
        {
            taskID = TaskDragged.ID
        };

        await DragStart.InvokeAsync(dragDropParameter);
    }

    private async Task HandleDayOnDrop(DateTime day)
    {
        if ( !Draggable ) return;
        if ( TaskDragged == null ) return;

        DragDropParameter dragDropParameter = new()
        {
            Day = day,
            taskID = TaskDragged.ID
        };

        await DropTask.InvokeAsync(dragDropParameter);

        TaskDragged = null;
    }

    private string GetBackground(DateTime day)
    {
        int d = (int)day.DayOfWeek;

        if (d == 6)
        {
            return $"background:{SaturdayColor}";
        }
        else if (d == 0)
        {
            return $"background:{SundayColor}";
        }
        else
        {
            return $"background:{WeekDaysColor}";
        }
    }
}
