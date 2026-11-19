using AngleSharp.Dom;
using BlazorCalendar.Models;
using Bunit;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorCalendar.Tests.Components;

public sealed class WeekViewRangeDragTests : BunitContext
{
    private static readonly DateTime Thursday = new(2026, 11, 19);

    private DragDropParameter? dropped;
    private TimeRangeSelectedParameter? selected;

    public WeekViewRangeDragTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    [Fact]
    public void RangeEnabled_Task_StaysDraggable()
    {
        IRenderedComponent<WeekView> cut = RenderWeek();

        Assert.Equal("True", cut.Find(".hour-task").GetAttribute("draggable"));
    }

    [Fact]
    public void RangeEnabled_DragStartThenDropOnHour_RaisesDropTask()
    {
        IRenderedComponent<WeekView> cut = RenderWeek();

        cut.Find(".hour-task").DragStart();
        DropZone(cut, Thursday.AddHours(14)).Drop();

        Assert.NotNull(dropped);
        Assert.Equal(1, dropped.taskID);
        Assert.Equal(Thursday.AddHours(14), dropped.Day);
    }

    [Fact]
    public void PointerEnter_WithoutSelection_DoesNotRerender()
    {
        IRenderedComponent<WeekView> cut = RenderWeek();
        int renders = cut.RenderCount;

        Slot(cut, Thursday.AddHours(9)).TriggerEvent("onpointerenter", Pointer(buttons: 0));

        Assert.Equal(renders, cut.RenderCount);
    }

    [Fact]
    public void PointerLeave_WithoutSelection_DoesNotRerender()
    {
        IRenderedComponent<WeekView> cut = RenderWeek();
        int renders = cut.RenderCount;

        cut.Find(".weekly-calendar").TriggerEvent("onpointerleave", Pointer(buttons: 0));

        Assert.Equal(renders, cut.RenderCount);
    }

    [Fact]
    public void RangeSelection_StillWorks()
    {
        IRenderedComponent<WeekView> cut = RenderWeek();
        DateTime first = Thursday.AddHours(14), last = first.AddMinutes(15);

        Slot(cut, first).TriggerEvent("onpointerdown", Pointer(buttons: 1));
        Slot(cut, last).TriggerEvent("onpointerenter", Pointer(buttons: 1));

        Assert.Contains("range-selected", Slot(cut, last).ClassName);

        Slot(cut, last).TriggerEvent("onpointerup", Pointer(buttons: 0));

        Assert.NotNull(selected);
        Assert.Equal(first, selected.Start);
        Assert.Equal(last.AddMinutes(15), selected.End);
    }

    private static PointerEventArgs Pointer(long buttons) =>
        new() { PointerId = 1, IsPrimary = true, Button = 0, Buttons = buttons };

    // Each slot exists twice (time label column and drop zone); target the drop zone one.
    private static IElement Slot(IRenderedComponent<WeekView> cut, DateTime slot) =>
        cut.Find($".hour-dropzone [data-range-slot='{slot:yyyy-MM-ddTHH:mm}']");

    private static IElement DropZone(IRenderedComponent<WeekView> cut, DateTime hour) =>
        Slot(cut, hour).ParentElement!;

    private IRenderedComponent<WeekView> RenderWeek() =>
        Render<WeekView>(ps => ps
            .AddCascadingValue("FirstDate", Thursday)
            .Add(p => p.WeekStartsOn, DayOfWeek.Monday)
            .Add(p => p.Draggable, true)
            .Add(p => p.EnableRangeSelection, true)
            .Add(p => p.RangeSelected, r => selected = r)
            .Add(p => p.DropTask, d => dropped = d)
            .Add(p => p.TasksList, new[]
            {
                new Tasks { ID = 1, Code = "A", Caption = "A", Color = "#f00",
                            DateStart = Thursday.AddHours(7), DateEnd = Thursday.AddHours(10) }
            }));
}
