namespace BlazorCalendar;

using System.Globalization;

internal sealed class Dates
{
    internal static int GetNumOfDay(int numOfDay)
    {
        // The order of the days of the week is different in each country
        // DayOfWeek : 
        //   .Sunday    = 0
        //   .Monday    = 1
        //   .Tuesday   = 2
        //   .Wednesday = 3
        //   .Thursday  = 4
        //   .Friday    = 5
        //   .Saturday  = 6
        //
        // => if Sunday is the First Day   =>   0  1  2  3  4  5  6
        // => if Monday is the First Day   =>   1  2  3  4  5  6  0
        // => if Saturday is the First Day =>   6  0  1  2  3  4  5

        // To test : 
        //CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("dz-DZ"); // firstDayOfWeek = Saturday
        //CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("fr-FR"); // firstDayOfWeek = Monday
        //CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US"); // firstDayOfWeek = Sunday

        if (numOfDay < 0) return 0;
        if (numOfDay > 6)
        {
            // Whatever the value I transform it in the place from 0 to 6
            numOfDay %= 7;
        }

        var firstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;

        if (firstDayOfWeek == DayOfWeek.Saturday)
        {
            if (numOfDay == 0) numOfDay = 6;
            if (numOfDay == 7) numOfDay = 0;
        }
        else if (firstDayOfWeek == DayOfWeek.Monday)
        {
            if (numOfDay >= 0) numOfDay++;
            if (numOfDay == 7) numOfDay = 0;
        }

        return numOfDay;
    }
}
