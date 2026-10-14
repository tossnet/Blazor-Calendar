using BlazorCalendar.Models;
using BlazorCalendar.Styles;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace BlazorCalendar;

partial class WeekView : CalendarBase, IAsyncDisposable
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

    /// <summary>Optional background of today's entire hour grid, including the time labels.</summary>
    [Parameter] public string? TodayBackgroundColor { get; set; }

    /// <summary>Optional 3px inset outline around today's entire day column.</summary>
    [Parameter] public string? TodayBorderColor { get; set; }

    private string TodayColumnStyle(DateTime day) => HighlightToday && day.Date == DateTime.Today &&
        !string.IsNullOrWhiteSpace(TodayBorderColor)
        ? $"outline:3px solid {TodayBorderColor};outline-offset:-3px;border-radius:4px;" : "";

    private string WeekBackground(DateTime day) => HighlightToday && day.Date == DateTime.Today &&
        !string.IsNullOrWhiteSpace(TodayBackgroundColor)
        ? $"background:{TodayBackgroundColor};color:{FontColor}" : GetBackgroundWithHighlight(day);

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

    /// <summary>Optional background and border colors for each weekday header.</summary>
    [Parameter] public string? HeaderBackgroundColor { get; set; }
    [Parameter] public string? HeaderBorderColor { get; set; }
    /// <summary>Opt-in fixed-height scrolling week with sticky day headers.</summary>
    [Parameter] public bool EnableHourScrolling { get; set; }
    [Parameter] public int VisibleStartHour { get; set; } = 6;
    [Parameter] public int VisibleEndHour { get; set; } = 17;
    /// <summary>Show nonzero counts of events extending above/below the visible viewport.</summary>
    [Parameter] public bool ShowOverflowIndicators { get; set; } = true;
    /// <summary>Optional minimum weekday column width in CSS pixels. Unset retains the existing layout.</summary>
    [Parameter] public int? MinimumDayWidth { get; set; }
    [Inject] private IJSRuntime JS { get; set; } = default!;
    private ElementReference weekElement;
    private IJSObjectReference? scrollModule;
    private bool disposed;
    private string WeekHeaderStyle => $"{HeaderStyle};" +
        (string.IsNullOrWhiteSpace(HeaderBackgroundColor) ? "" : $"background:{HeaderBackgroundColor};") +
        (string.IsNullOrWhiteSpace(HeaderBorderColor) ? "" : $"box-shadow:inset 0 0 0 1px {HeaderBorderColor};");
    private string WeekStyle => $"{Style};--week-visible-hours:{VisibleEndHour-VisibleStartHour};" +
        (MinimumDayWidth is int width ? $"grid-template-columns:repeat(7,minmax({width}px,1fr));" : "");
    private static readonly string JsPath =
       $"./_content/BlazorCalendar/weekView.js?v={typeof(WeekView).Assembly.GetName().Version}";

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (disposed || (!EnableHourScrolling && scrollModule is null)) return;
        scrollModule ??= await JS.InvokeAsync<IJSObjectReference>("import", JsPath);
        if (disposed) return;
        await scrollModule.InvokeVoidAsync("configure", weekElement, EnableHourScrolling,
            VisibleStartHour, ShowOverflowIndicators, FirstDate.ToString("yyyy-MM-dd"));
    }

    public async ValueTask DisposeAsync()
    {
        disposed = true;
        if (scrollModule is null) return;
        try { await scrollModule.InvokeVoidAsync("dispose", weekElement); await scrollModule.DisposeAsync(); }
        catch (JSDisconnectedException) { }
    }

    private DateTime? selectionAnchor;
    private DateTime? selectionLast;
    private long selectionPointer;
    private bool suppressRangeClick;
    private bool CanSelectRange => EnableRangeSelection && RangeSelected.HasDelegate;

    protected override void OnParametersSet()
    {
        if (MinimumDayWidth is <= 0)
            throw new ArgumentOutOfRangeException(nameof(MinimumDayWidth), "The minimum weekday width must be positive.");
        if (EnableHourScrolling && (VisibleStartHour < 0 || VisibleEndHour > 24 || VisibleEndHour <= VisibleStartHour))
            throw new ArgumentOutOfRangeException(nameof(VisibleStartHour), "The visible interval must be inside 00:00–24:00 and have a positive duration.");
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
