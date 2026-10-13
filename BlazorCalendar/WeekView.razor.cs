using BlazorCalendar.Models;
using BlazorCalendar.Styles;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorCalendar;

partial class WeekView : CalendarBase
{
    [CascadingParameter(Name = "SelectedView")]
    public DisplayedView DisplayedView { get; set; } = DisplayedView.Weekly;

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
    public EventCallback<ClickEmptyDayParameter> EmptyDayClick { get; set; }

    [Parameter]
    public EventCallback<ClickTaskParameter> TaskClick { get; set; }

    [Parameter]
    public EventCallback<DragDropParameter> DragStart { get; set; }

    [Parameter]
    public EventCallback<DragDropParameter> DropTask { get; set; }

    /// <summary>Explicit week start; null retains the current culture convention.</summary>
    [Parameter] public DayOfWeek? WeekStartsOn { get; set; }

    /// <summary>Optional weekday header format. The default preserves the existing abbreviated date header.</summary>
    [Parameter] public string DayHeaderFormat { get; set; } = "ddd dd.MM";

    /// <summary>Enables pointer selection on the empty weekly grid. Disabled by default.</summary>
    [Parameter] public bool EnableRangeSelection { get; set; }

    /// <summary>A normalized same-day range with an exclusive end. Does not create or mutate tasks.</summary>
    [Parameter] public EventCallback<TimeRangeSelectedParameter> RangeSelected { get; set; }

    private DateTime? selectionAnchor;
    private DateTime? selectionLast;
    private long selectionPointer;
    private bool suppressRangeClick;
    private bool CanSelectRange => EnableRangeSelection && RangeSelected.HasDelegate;

    protected override void OnParametersSet()
    {
        if (!CanSelectRange || DisplayedView != DisplayedView.Weekly ||
            (selectionAnchor is DateTime anchor && (anchor.Date < FirstDate.AddDays(-6) || anchor.Date > FirstDate.AddDays(6))))
            CancelRange();
    }

    private void StartRange(PointerEventArgs e, DateTime slot)
    {
        if (!CanSelectRange || e.Button != 0 || !e.IsPrimary) return;
        selectionAnchor = selectionLast = slot;
        selectionPointer = e.PointerId;
        suppressRangeClick = false;
    }

    private void ExtendRange(PointerEventArgs e, DateTime slot)
    {
        if (selectionAnchor is not DateTime anchor || e.PointerId != selectionPointer) return;
        if ((e.Buttons & 1) == 0 || slot.Date != anchor.Date) { CancelRange(); return; }
        selectionLast = slot;
    }

    private async Task FinishRange(PointerEventArgs e, DateTime slot)
    {
        if (!CanSelectRange || selectionAnchor is not DateTime anchor || e.PointerId != selectionPointer) return;
        if (slot.Date != anchor.Date) { CancelRange(); return; }
        var start = anchor < slot ? anchor : slot;
        var end = (anchor > slot ? anchor : slot).AddMinutes(15);
        CancelRange();
        suppressRangeClick = true;
        await RangeSelected.InvokeAsync(new TimeRangeSelectedParameter { Start = start, End = end });
    }

    private bool IsRangeSlot(DateTime slot) => selectionAnchor is DateTime a && selectionLast is DateTime b &&
        slot.Date == a.Date && slot >= (a < b ? a : b) && slot <= (a > b ? a : b);

    private void CancelRange() { selectionAnchor = selectionLast = null; }
    private void RangeKeyDown(KeyboardEventArgs e) { if (e.Key == "Escape") CancelRange(); }

    private Tasks? TaskDragged;
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
        if (!Draggable) 
            return;

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

    private async Task ClickDayInternal(MouseEventArgs e, DateTime day)
    {
        if (suppressRangeClick) { suppressRangeClick = false; return; }
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

    private string GetTaskStyle(Tasks task)
    {
        string style = Colors.GetHatching(task.FillStyle, task.Color);
        return $"{style};color:{task.ForeColor}";
    }
}
