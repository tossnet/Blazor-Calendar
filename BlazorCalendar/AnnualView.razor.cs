using BlazorCalendar.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorCalendar;

partial class AnnualView : CalendarBase
{
    [CascadingParameter(Name = "SelectedView")]
    public DisplayedView DisplayedView { get; set; } = DisplayedView.Annual;

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
    public EventCallback<ClickTaskParameter> TaskClick { get; set; }

    [Parameter]
    public EventCallback<ClickEmptyDayParameter> EmptyDayClick { get; set; }

    [Parameter]
    public EventCallback<DragDropParameter> DragStart { get; set; }

    [Parameter]
    public EventCallback<DragDropParameter> DropTask { get; set; }

    [Parameter]
    public EventCallback<DateTime> HeaderClick { get; set; }

    /// <summary>
    /// When set to <c>true</c>, displays a red border around today's date for better visibility.
    /// </summary>
    [Parameter]
    public bool HighlightToday { get; set; } = false;

    private DateTime m = DateTime.Today;
    private Tasks? TaskDragged;

    // Task index for O(1) lookup by date
    private Dictionary<DateTime, List<Tasks>>? _tasksByDate;
    private int _lastTasksHash;
    private int _lastTasksLength;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        // Rebuild index only when TasksList content actually changes
        if (HasTasksListChanged())
        {
            BuildTaskIndex();
            _lastTasksLength = TasksList?.Length ?? 0;
            _lastTasksHash = ComputeTasksHash();
        }
    }

    /// <summary>
    /// Detects if TasksList content has changed without requiring reference equality.
    /// Uses O(1) length check + O(n) hash only when length matches.
    /// </summary>
    private bool HasTasksListChanged()
    {
        int currentLength = TasksList?.Length ?? 0;

        // Quick O(1) check: different length = definitely changed
        if (currentLength != _lastTasksLength)
            return true;

        // Both empty = no change
        if (currentLength == 0)
            return false;

        // Same length: compare hash to detect content changes
        return ComputeTasksHash() != _lastTasksHash;
    }

    /// <summary>
    /// Computes a simple hash based on task IDs and dates for change detection.
    /// </summary>
    private int ComputeTasksHash()
    {
        if (TasksList is null || TasksList.Length == 0)
            return 0;

        unchecked
        {
            int hash = 17;
            foreach (var task in TasksList)
            {
                hash = hash * 31 + task.ID;
                hash = hash * 31 + task.DateStart.GetHashCode();
                hash = hash * 31 + task.DateEnd.GetHashCode();
            }
            return hash;
        }
    }

    /// <summary>
    /// Builds an index of tasks by date for O(1) lookup during rendering.
    /// </summary>
    private void BuildTaskIndex()
    {
        _tasksByDate = new Dictionary<DateTime, List<Tasks>>();

        if (TasksList is null)
            return;

        foreach (var task in TasksList)
        {
            for (var date = task.DateStart.Date; date <= task.DateEnd.Date; date = date.AddDays(1))
            {
                if (!_tasksByDate.TryGetValue(date, out var list))
                {
                    list = new List<Tasks>(4);
                    _tasksByDate[date] = list;
                }
                list.Add(task);
            }
        }
    }

    /// <summary>
    /// Gets tasks for a specific date using O(1) dictionary lookup.
    /// </summary>
    private List<Tasks> GetTasksForDate(DateTime date)
    {
        if (_tasksByDate is null || !_tasksByDate.TryGetValue(date.Date, out var tasks))
            return [];

        return tasks;
    }

    private async Task ClickInternal(MouseEventArgs e, DateTime day)
    {
        if (day == default) 
            return;

        // Early exit if no delegates registered
        if (!TaskClick.HasDelegate && !EmptyDayClick.HasDelegate)
            return;

        // Use indexed lookup instead of linear search
        var tasksForDay = GetTasksForDate(day);

        if (tasksForDay.Count > 0)
        {
            if (TaskClick.HasDelegate)
            {
                var listID = new List<int>(tasksForDay.Count);
                foreach (var t in tasksForDay)
                {
                    listID.Add(t.ID);
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
        else
        {
            if (EmptyDayClick.HasDelegate)
            {
                ClickEmptyDayParameter clickEmptyDayParameter = new()
                {
                    Day = day,
                    X = e.ClientX,
                    Y = e.ClientY
                };

                await EmptyDayClick.InvokeAsync(clickEmptyDayParameter);
            }
        }
    }

    private async Task HandleDragStart(DateTime day, int taskID) 
    {
        if (taskID < 0) 
            return;

        TaskDragged = new Tasks()
        {
            ID = taskID
        };

        DragDropParameter dragDropParameter = new()
        {
            Day = day,
            taskID = TaskDragged.ID
        };

        await DragStart.InvokeAsync(dragDropParameter);
    }

    private async Task HandleDayOnDrop(DateTime day)
    {
        if (TaskDragged is null) 
            return;

        DragDropParameter dragDropParameter = new()
        {
            Day = day,
            taskID = TaskDragged.ID
        };

        await DropTask.InvokeAsync(dragDropParameter);

        TaskDragged = null;
    }

    private async Task HandleHeaderClick(DateTime month)
    {
        if (!HeaderClick.HasDelegate) 
            return;

        await HeaderClick.InvokeAsync(month);
    }

    /// <summary>
    /// Generates the background style for disabled days with a hatched pattern
    /// </summary>
    private string? GetDisabledBackground(bool isDisabled)
    {
        if (!isDisabled)
            return null;

        return $"background: linear-gradient(-45deg, {DisabledDayColor} 25%, transparent 25%, transparent 50%, {DisabledDayColor} 50%, {DisabledDayColor} 75%, transparent 75%, transparent); background-size: 8px 8px;";
    }
}
