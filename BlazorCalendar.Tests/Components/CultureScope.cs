using System.Globalization;

namespace BlazorCalendar.Tests.Components;

/// <summary>Sets the current culture for the duration of a test and restores it afterwards.</summary>
internal sealed class CultureScope : IDisposable
{
    private readonly CultureInfo previousCulture = CultureInfo.CurrentCulture;
    private readonly CultureInfo previousUICulture = CultureInfo.CurrentUICulture;

    public CultureScope(string cultureName)
    {
        CultureInfo culture = CultureInfo.GetCultureInfo(cultureName);
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
    }

    public void Dispose()
    {
        CultureInfo.CurrentCulture = previousCulture;
        CultureInfo.CurrentUICulture = previousUICulture;
    }
}
