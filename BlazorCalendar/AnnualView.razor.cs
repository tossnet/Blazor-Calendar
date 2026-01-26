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

    /// <summary>
    /// Gets tasks that start on a specific date or continue from a previous month.
    /// Returns tasks with their calculated span within the current month.
    /// </summary>
    private List<(Tasks Task, int StartRow, int Span)> GetTasksForMonth(DateTime monthStart)
    {
        if (TasksList is null)
            return [];

        var result = new List<(Tasks Task, int StartRow, int Span)>();
        var lastDayOfMonth = new DateTime(monthStart.Year, monthStart.Month, DateTime.DaysInMonth(monthStart.Year, monthStart.Month));

        foreach (var task in TasksList)
        {
            // Check if task overlaps with this month
            if (task.DateEnd.Date < monthStart.Date || task.DateStart.Date > lastDayOfMonth.Date)
                continue;

            // Calculate effective start within the month
            var effectiveStart = task.DateStart.Date < monthStart.Date ? monthStart.Date : task.DateStart.Date;
            // Calculate effective end within the month
            var effectiveEnd = task.DateEnd.Date > lastDayOfMonth.Date ? lastDayOfMonth.Date : task.DateEnd.Date;

            // StartRow is 1-based (row 1 is header, row 2 is day 1)
            int startRow = effectiveStart.Day + 1;
            int span = (effectiveEnd - effectiveStart).Days + 1;

            result.Add((task, startRow, span));
        }

        // Sort by start date, then by duration (longer tasks first for overlap handling)
        return result.OrderBy(x => x.StartRow).ThenByDescending(x => x.Span).ToList();
    }

    /// <summary>
    /// Assigns columns to tasks to handle overlapping tasks within a month.
    /// Also calculates column span so non-overlapping tasks can use full width.
    /// </summary>
    private List<(Tasks Task, int StartRow, int Span, int Column, int ColSpan)> AssignTaskColumns(List<(Tasks Task, int StartRow, int Span)> tasks)
    {
        var result = new List<(Tasks Task, int StartRow, int Span, int Column, int ColSpan)>();
        var columnEndRows = new List<int>(); // Track the end row of each column

        // First pass: assign columns
        var taskAssignments = new List<(Tasks Task, int StartRow, int Span, int Column, int EndRow)>();

        foreach (var (task, startRow, span) in tasks)
        {
            int endRow = startRow + span - 1;
            int assignedColumn = -1;

            // Find first available column
            for (int c = 0; c < columnEndRows.Count; c++)
            {
                if (columnEndRows[c] < startRow)
                {
                    assignedColumn = c + 1; // Columns are 1-based
                    columnEndRows[c] = endRow;
                    break;
                }
            }

            // If no column available, create a new one
            if (assignedColumn == -1)
            {
                columnEndRows.Add(endRow);
                assignedColumn = columnEndRows.Count;
            }

            taskAssignments.Add((task, startRow, span, assignedColumn, endRow));
        }

        int totalColumns = columnEndRows.Count > 0 ? columnEndRows.Count : 1;

        // Second pass: calculate column spans for each task
        foreach (var (task, startRow, span, column, endRow) in taskAssignments)
        {
            int colSpan = 1;

            // Check how many consecutive columns to the right are free during this task's time
            for (int nextCol = column + 1; nextCol <= totalColumns; nextCol++)
            {
                bool canExtend = true;

                // Check if any other task uses nextCol and overlaps with this task's rows
                foreach (var other in taskAssignments)
                {
                    if (other.Column == nextCol)
                    {
                        int otherEndRow = other.StartRow + other.Span - 1;
                        // Check for overlap in rows
                        if (!(other.StartRow > endRow || otherEndRow < startRow))
                        {
                            canExtend = false;
                            break;
                        }
                    }
                }

                if (canExtend)
                    colSpan++;
                else
                    break;
            }

            result.Add((task, startRow, span, column, colSpan));
        }

        return result;
    }

    /// <summary>
    /// Gets the maximum number of task columns needed for a month.
    /// </summary>
    private int GetMaxTaskColumns(List<(Tasks Task, int StartRow, int Span, int Column, int ColSpan)> tasksWithColumns)
    {
        if (tasksWithColumns.Count == 0)
            return 1;
        return tasksWithColumns.Max(x => x.Column);
    }

    private async Task ClickInternal(MouseEventArgs e, DateTime day,int? taskId = null)
    {
        if (taskId.HasValue)
        {
            // Clicked on a specific task
            if (TaskClick.HasDelegate)
            {
                ClickTaskParameter clickTaskParameter = new()
                {
                    IDList = new List<int> { taskId.Value },
                    X = e.ClientX,
                    Y = e.ClientY,
                    Day = day
                };
                await TaskClick.InvokeAsync(clickTaskParameter);
            }
            return;
        }

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
