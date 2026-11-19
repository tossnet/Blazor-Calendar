using System.Globalization;
using BlazorCalendar.Helpers;

namespace BlazorCalendar.Tests.Helpers;

public class WeekDayTitleTests
{
    private const string DefaultFormat = "ddd dd.MM";
    private static readonly CultureInfo French = CultureInfo.GetCultureInfo("fr-FR");
    private static readonly CultureInfo English = CultureInfo.GetCultureInfo("en-US");
    private static readonly DateTime Thursday = new(2026, 11, 19);

    [Fact]
    public void Create_FrenchThursdayWithDefaultFormat_ReturnsAllVariants()
    {
        WeekDayTitle title = WeekDayTitle.Create(Thursday, DefaultFormat, French);

        Assert.Equal("jeu. 19.11", title.Full);
        Assert.Equal("J 19.11", title.DayAndMonth);
        Assert.Equal("J 19", title.Day);
        Assert.Equal("J", title.Letter);
    }

    [Fact]
    public void Create_EnglishSunday_UsesEnglishDayName()
    {
        WeekDayTitle title = WeekDayTitle.Create(new DateTime(2026, 11, 22), DefaultFormat, English);

        Assert.Equal("Sun 22.11", title.Full);
        Assert.Equal("S 22.11", title.DayAndMonth);
        Assert.Equal("S", title.Letter);
    }

    [Fact]
    public void Create_CustomFormat_OnlyChangesFullVariant()
    {
        WeekDayTitle title = WeekDayTitle.Create(Thursday, "dddd d MMMM", French);

        Assert.Equal("jeudi 19 novembre", title.Full);
        Assert.Equal("J 19.11", title.DayAndMonth);
        Assert.Equal("J 19", title.Day);
    }

    [Theory]
    [InlineData("fr-FR")]
    [InlineData("en-US")]
    [InlineData("de-DE")]
    [InlineData("ja-JP")]
    public void Create_AnyWeekday_LetterIsUppercaseAndPrefixesCompactVariants(string cultureName)
    {
        CultureInfo culture = CultureInfo.GetCultureInfo(cultureName);

        for (int offset = 0; offset < 7; offset++)
        {
            WeekDayTitle title = WeekDayTitle.Create(Thursday.AddDays(offset), DefaultFormat, culture);

            Assert.NotEmpty(title.Letter);
            Assert.Equal(culture.TextInfo.ToUpper(title.Letter), title.Letter);
            Assert.StartsWith(title.Letter + " ", title.Day, StringComparison.Ordinal);
            Assert.StartsWith(title.Letter + " ", title.DayAndMonth, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void IsFor_SameInputs_ReturnsTrue()
    {
        WeekDayTitle title = WeekDayTitle.Create(Thursday, DefaultFormat, French);

        Assert.True(title.IsFor(Thursday, DefaultFormat, French));
    }

    [Fact]
    public void IsFor_DifferentDate_ReturnsFalse()
    {
        WeekDayTitle title = WeekDayTitle.Create(Thursday, DefaultFormat, French);

        Assert.False(title.IsFor(Thursday.AddDays(7), DefaultFormat, French));
    }

    [Fact]
    public void IsFor_DifferentFormat_ReturnsFalse()
    {
        WeekDayTitle title = WeekDayTitle.Create(Thursday, DefaultFormat, French);

        Assert.False(title.IsFor(Thursday, "dddd", French));
    }

    [Fact]
    public void IsFor_DifferentCulture_ReturnsFalse()
    {
        WeekDayTitle title = WeekDayTitle.Create(Thursday, DefaultFormat, French);

        Assert.False(title.IsFor(Thursday, DefaultFormat, English));
    }
}
