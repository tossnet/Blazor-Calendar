using AngleSharp.Dom;
using BlazorCalendar.Models;
using Bunit;

namespace BlazorCalendar.Tests.Components;

public sealed class MonthlyViewLanesTests : BunitContext
{
    private static readonly DateTime Day = new(2026, 11, 19);
    private readonly CultureScope culture = new("fr-FR");

    public MonthlyViewLanesTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    [Fact]
    public void Render_Default_ShowsFourTasksAndOneBadge()
    {
        IRenderedComponent<MonthlyView> cut = RenderMonth(TasksOn(Day, 5));

        Assert.Equal(4, cut.FindAll(".monthly-task").Count);
        IElement badge = Assert.Single(cut.FindAll(".monthly-more-tasks"));
        Assert.Equal("+1", badge.TextContent.Trim());
    }

    [Fact]
    public void Render_MaxTasksPerDayThree_ShowsThreeTasksAndHiddenCount()
    {
        IRenderedComponent<MonthlyView> cut = RenderMonth(TasksOn(Day, 5), maxTasksPerDay: 3);

        Assert.Equal(3, cut.FindAll(".monthly-task").Count);
        Assert.Equal("+2", Assert.Single(cut.FindAll(".monthly-more-tasks")).TextContent.Trim());
    }

    [Fact]
    public void Render_TasksFitInLanes_NoBadge()
    {
        IRenderedComponent<MonthlyView> cut = RenderMonth(TasksOn(Day, 4));

        Assert.Equal(4, cut.FindAll(".monthly-task").Count);
        Assert.Empty(cut.FindAll(".monthly-more-tasks"));
    }

    [Fact]
    public void Render_TasksGetDistinctLanes()
    {
        IRenderedComponent<MonthlyView> cut = RenderMonth(TasksOn(Day, 4));

        string[] styles = cut.FindAll(".monthly-task").Select(e => e.GetAttribute("style")!).ToArray();

        for (int lane = 0; lane < 4; lane++)
        {
            Assert.Contains(styles, s => s.Contains($"--lane:{lane};"));
        }
    }

    [Fact]
    public void Render_HiddenMultiDayTask_HasBadgeOnEachSpannedDay()
    {
        Tasks[] tasks = TasksOn(Day, 1)
            .Append(new Tasks { ID = 99, DateStart = Day.AddDays(-1), DateEnd = Day.AddDays(1), Code = "LONG" })
            .ToArray();

        IRenderedComponent<MonthlyView> cut = RenderMonth(tasks, maxTasksPerDay: 1);

        // The long task starts first (sorted by date), so it takes the lane; the one-day task is hidden.
        Assert.Single(cut.FindAll(".monthly-task"));
        Assert.Equal("+1", Assert.Single(cut.FindAll(".monthly-more-tasks")).TextContent.Trim());
    }

    [Fact]
    public void Render_ContainerExposesLaneCount()
    {
        IRenderedComponent<MonthlyView> cut = RenderMonth(TasksOn(Day, 1), maxTasksPerDay: 6);

        Assert.Contains("--monthly-lanes:6;", cut.Find(".monthly-calendar").GetAttribute("style"));
    }

    [Theory]
    [InlineData(11, 23, "grid-column:1 / span 1; grid-row:6;")]
    [InlineData(12, 2, "grid-column:1 / span 7; grid-row:6;|grid-column:1 / span 1; grid-row:7;")]
    public void Render_MultiDayTaskWrapsAtWeekBoundary(int endMonth, int endDay, string followingSegments)
    {
        // Sunday is the last column in the French-culture month view.
        Tasks task = new()
        {
            ID = 99, DateStart = new DateTime(2026, 11, 22), DateEnd = new DateTime(2026, endMonth, endDay),
            Code = "WRAP"
        };
        IRenderedComponent<MonthlyView> cut = RenderMonth([task]);
        var expected = new[] { "grid-column:7 / span 1; grid-row:5;" }.Concat(followingSegments.Split('|')).ToArray();
        var segments = cut.FindAll(".monthly-task");
        Assert.Equal(expected.Length, segments.Count);
        for (int i = 0; i < expected.Length; i++)
            Assert.StartsWith(expected[i], segments[i].GetAttribute("style"));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) culture.Dispose();
        base.Dispose(disposing);
    }

    private static Tasks[] TasksOn(DateTime day, int count) =>
        Enumerable.Range(1, count)
                  .Select(i => new Tasks { ID = i, DateStart = day, DateEnd = day, Code = $"T{i}" })
                  .ToArray();

    private IRenderedComponent<MonthlyView> RenderMonth(Tasks[] tasks, int? maxTasksPerDay = null) =>
        Render<MonthlyView>(ps =>
        {
            ps.AddCascadingValue("FirstDate", Day)
              .AddCascadingValue("TasksList", tasks);
            if (maxTasksPerDay is int max)
                ps.Add(p => p.MaxTasksPerDay, max);
        });
}
