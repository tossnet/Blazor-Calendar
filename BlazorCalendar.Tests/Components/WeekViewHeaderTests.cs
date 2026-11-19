using AngleSharp.Dom;
using Bunit;

namespace BlazorCalendar.Tests.Components;

public sealed class WeekViewHeaderTests : BunitContext
{
    private static readonly DateTime Thursday = new(2026, 11, 19);
    private readonly CultureScope culture = new("fr-FR");

    public WeekViewHeaderTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    [Fact]
    public void Render_EachDay_HasAllTitleVariants()
    {
        IRenderedComponent<WeekView> cut = RenderWeek(Thursday);

        IReadOnlyList<IElement> titles = cut.FindAll(".week-day-title");

        Assert.Equal(7, titles.Count);
        Assert.All(titles, title =>
        {
            Assert.NotNull(title.QuerySelector(".wdt-full"));
            Assert.NotNull(title.QuerySelector(".wdt-dm"));
            Assert.NotNull(title.QuerySelector(".wdt-d"));
            Assert.NotNull(title.QuerySelector(".wdt-l"));
        });
    }

    [Fact]
    public void Render_FirstColumn_ShowsMondayVariantsAndTooltip()
    {
        IRenderedComponent<WeekView> cut = RenderWeek(Thursday);

        IElement monday = cut.FindAll(".week-day-title")[0];

        Assert.Equal("lun. 16.11", monday.GetAttribute("title"));
        Assert.Equal("lun. 16.11", monday.QuerySelector(".wdt-full")!.TextContent);
        Assert.Equal("L 16.11", monday.QuerySelector(".wdt-dm")!.TextContent);
        Assert.Equal("L 16", monday.QuerySelector(".wdt-d")!.TextContent);
        Assert.Equal("L", monday.QuerySelector(".wdt-l")!.TextContent);
    }

    [Fact]
    public void Render_DayHeaderFormatChanged_UpdatesFullTitle()
    {
        IRenderedComponent<WeekView> cut = RenderWeek(Thursday);

        cut.Render(ps => ps.Add(p => p.DayHeaderFormat, "dddd"));

        Assert.Equal("lundi", cut.FindAll(".wdt-full")[0].TextContent);
    }

    [Fact]
    public void Render_NextWeek_UpdatesTitles()
    {
        RenderWeek(Thursday);

        IRenderedComponent<WeekView> nextWeek = RenderWeek(Thursday.AddDays(7));

        Assert.Equal("L 23", nextWeek.FindAll(".wdt-d")[0].TextContent);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) culture.Dispose();
        base.Dispose(disposing);
    }

    private IRenderedComponent<WeekView> RenderWeek(DateTime firstDate) =>
        Render<WeekView>(ps => ps
            .AddCascadingValue("FirstDate", firstDate)
            .Add(p => p.WeekStartsOn, DayOfWeek.Monday));
}
