namespace BlazorCalendar;

using BlazorCalendar.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

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
    public bool HighlightToday { get; set; } = false;

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

    private enum StateCase
    {
        Before = 0, // First empty cells part
        InMonth = 1,
        After = 2,
    }

    private async Task HandleClickOutsideCurrentMonthClick(int AddMonth)
    {
        if (OutsideCurrentMonthClick.HasDelegate)
		{
			await OutsideCurrentMonthClick.InvokeAsync(AddMonth);
		}
    }


    private async Task ClickTaskInternal(MouseEventArgs e, int taskID, DateTime day)
    {
        if (!TaskClick.HasDelegate) 
            return;
 
		List<int> listID = new()
		{
			taskID
		};

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
        if (day == default) 
            return;

		if (!TaskClick.HasDelegate) 
            return;

		// There can be several tasks in one day :
		List<int> listID = new();

        if (TasksList is not null)
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
		if (!DayClick.HasDelegate) 
            return;

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
        if ( !Draggable ) 
            return;

        if ( TaskDragged is null ) 
            return;

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
        
        return $"background:{WeekDaysColor}";
    }
}
