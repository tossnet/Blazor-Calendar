using AngleSharp.Dom;
using BlazorCalendar.Models;
using Bunit;

namespace BlazorCalendar.Tests.Components;

public sealed class WeekViewDragTests : BunitContext
{
    private static readonly DateTime Thursday = new(2026, 11, 19);

    public WeekViewDragTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    [Fact]
    public void Render_HoursContainer_UsesCustomDragImage()
    {
        IRenderedComponent<WeekView> cut = RenderWeek();

        IElement hours = cut.FindAll(".hours")[0];

        Assert.Contains("setDragImage", hours.GetAttribute("ondragstart"));
        Assert.Contains("dragging", hours.GetAttribute("ondragend"));
    }

    [Fact]
    public void Render_Task_HasNoInlineDragEndHandler()
    {
        IRenderedComponent<WeekView> cut = RenderWeek();

        IElement task = cut.Find(".hour-task");

        Assert.Null(task.GetAttribute("ondragend"));
        Assert.Equal("True", task.GetAttribute("draggable"));
    }

    private IRenderedComponent<WeekView> RenderWeek() =>
        Render<WeekView>(ps => ps
            .AddCascadingValue("FirstDate", Thursday)
            .Add(p => p.WeekStartsOn, DayOfWeek.Monday)
            .Add(p => p.Draggable, true)
            .Add(p => p.TasksList, new[]
            {
                new Tasks { ID = 1, Code = "A", Caption = "A", Color = "#f00",
                            DateStart = Thursday.AddHours(7), DateEnd = Thursday.AddHours(10) }
            }));
}
