# Blazor-Calendar
[![NuGet](https://img.shields.io/nuget/v/BlazorCalendar.svg)](https://www.nuget.org/packages/BlazorCalendar/)  ![BlazorCalendar Nuget Package](https://img.shields.io/nuget/dt/BlazorCalendar)
[![GitHub](https://img.shields.io/github/license/tossnet/Blazor-Calendar?color=594ae2&logo=github&style=flat-square)](https://github.com/tossnet/Blazor-Calendar/blob/main/LICENSE)

For Blazor Server or Blazor WebAssembly

## Live demo
Blazor webassembly : https://tossnet.github.io/Blazor-Calendar

![peek_5](https://github.com/user-attachments/assets/a3e3dda4-ecc6-465a-af8f-68cd5a6dfb5b)



![monthlyView](https://user-images.githubusercontent.com/3845786/159467420-8140bf09-b24b-4880-91a2-036c9824336a.gif)

## Installation
Latest version in here: [![NuGet](https://img.shields.io/nuget/v/BlazorCalendar.svg)](https://www.nuget.org/packages/BlazorCalendar/) 

To Install

```
Install-Package BlazorCalendar
```
or
```
dotnet add package BlazorCalendar
```
For client-side and server-side Blazor - add script section to _Layout.cshtml (head section)

```html
 <link href="_content/BlazorCalendar/BlazorCalendar.css" rel="stylesheet" />
```

## Documentation
**📚 [ Doc on Wiki](https://github.com/tossnet/Blazor-Calendar/wiki)**




## <a name="ReleaseNotes"></a>Release Notes

<details open="open"><summary>Version 3.2.1</summary>

> - Add MonthStyle Parameter to customize by example widht of the month column in the AnnualView (min-width, max-width, width)
</details>

<details open="open"><summary>Version 3.2.0</summary>

> - **New Feature** : Add `HighlightedPeriods` support for marking date ranges with custom background colors (vacations, holidays, projects, etc.)
> - Supports both single days (DateEnd optional) and date periods
> - Customizable background and text colors with optional labels
> - Applied to all calendar views (AnnualView, MonthlyView, WeekView)

</details>

<details><summary>Version 3.1.4</summary>

>	- **AnnualView** : change css unit border (px instead of vw) to avoid big border on large screen
</details>

<details><summary>Version 3.1.3</summary>

>- **AnnualView** : improve font-size of task hours 
</details>

<details open="open"><summary>Version 3.1.2</summary>

>- **AnnualView improvement** : Fix task hours visibility logic
</details>

<details><summary>Version 3.1.1</summary>

>- **AnnualView improvement** : display the start and end times of the task if space is available graphically
 - **AnnualView** : fix : if drag and drop is disabled, the Drop event does not check the boolean
</details>

<details><summary>Version 3.1.0</summary>

>- **AnnualView improvement** : Non-overlapping tasks now span the full available width instead of being restricted to a single column, improving calendar readability
</details>

<details><summary>Version 3.0.2</summary>

>- **Performance optimization** : AnnualView now uses Dictionary-based O(1) lookup instead of O(n) linear search for tasks
>- **Smart change detection** : Index is rebuilt only when task content actually changes (using hash comparison), not on every render cycle
>- **WeekView improvement** : Overlapping tasks are now displayed in separate columns for better visibility
</details>

<details><summary>Version 3.0.0</summary>

>- Add .NET10 and remove .NET7
>- Highlight TODAY in AnnualView [Issue #22](https://github.com/tossnet/Blazor-Calendar/issues/22)
>- Custom Styles for DisabledDay [Issue #23](https://github.com/tossnet/Blazor-Calendar/issues/23)
>- Add FontColor property to customize the text color of calendar days
</details>

<details ><summary>Version 2.7.1</summary>

>- In the Weekview, the component did not correctly display the first day of the week according to Culture [Issue #24](https://github.com/tossnet/Blazor-Calendar/issues/24)
</details>

<details><summary>Version 2.7.0</summary>

>- Add .NET9 and remove .NET6.0 
</details>

<details><summary>Version 2.6.5</summary>

>- Add WeekView (thanks [BruderJohn](https://github.com/BruderJohn) )  [Pull #13](https://github.com/tossnet/Blazor-Calendar/pull/13)
</details>

<details><summary>Version 2.6.4</summary>

>- Use task IDs to identify containing div (for JS extensibility)  [Pull #11](https://github.com/tossnet/Blazor-Calendar/pull/11)
</details>

<details><summary>Version 2.6.3</summary>

>- In the monthly view, the calendar displays 3 items  [Issue #8](https://github.com/tossnet/Blazor-Calendar/issues/8)
</details>

<details><summary>Version 2.6.1</summary>

>- MonthlyView : new property HighlightToday (boolean)  [Merge #9](https://github.com/tossnet/Blazor-Calendar/pull/9)
</details>

<details><summary>Version 2.5.3</summary>

>- MonthlyView : fix: duplication of the number of additional tasks [Merge #7](https://github.com/tossnet/Blazor-Calendar/pull/7)
</details>


<details><summary>Version 2.5.2</summary>

>- MonthlyView : return the day on the event ClickEmptyDayParameter. [Merge #5](https://github.com/tossnet/Blazor-Calendar/pull/5)
</details>

<details><summary>Version 2.5.1</summary>
 
>- add new prop named (int) Type  
>- annualView : return the day on the event ClickEmptyDayParameter
</details>

<details><summary>Version 2.5.0</summary>
 
>- new property "FillStyle" (Fill, BackwardDiagonal, ZigZag, Triangles, CrossDots)
</details>

<details><summary>Version 2.4.4</summary>

>- Issue #3
</details>

<details><summary>Version 2.4.3</summary> 

>- Monthly View : we could move a task even if we didn't allow the move
</details>

<details><summary>Version 2.4.2</summary>

>- Issue #2
</details>

<details><summary>Version 2.4.1</summary>
 
>- add white background of headers.
>- AnnualView : lightly rounded edge.
>- In the monthlyview, If a task has a line break (next week) the left edge is not displayed anymore.
</details>

<details><summary>Version 2.4.0</summary>

>- add white background of headers.
>- In the monthlyview, display the start time if it exists.
</details>

<details><summary>Version 2.3.0</summary>

>- improved positioning of tasks in the monthly view.
>- AnnualView : add new event HeaderClick that returns a DateTime (the month clicked).
>- Improvement of the css responsive .
</details>

<details><summary>Version 2.2.0</summary>

>- fix bug.
>- added the NotBeDraggable property.
</details>

<details><summary>Version 2.1.0</summary>   

>- css style improvement.
>- Addition of hatching in the cells at the end of the month.
>- Add a new view called MonthlyView.
</details>

#### ⚠️ Breaking changes ⚠️

<details><summary>Upgrading from 1.0 to 2.0</summary>

* before version 2 :
```html
 <link href="_content/BlazorCalendar/AnnualCalendar.css" rel="stylesheet" />
```

```razor
<AnnualCalendar  FirstDate="today" Months="months"  TasksList="TasksList.ToArray()" />
```

* from version 2 :
```html
 <link href="_content/BlazorCalendar/BlazorCalendar.css" rel="stylesheet" />
```

```razor
<CalendarContainer  FirstDate="today"  TasksList="TasksList.ToArray()" >
   <AnnualView  Months="months" />
</CalendarContainer>
```
   **Reason**
  
  I anticipate creating another monthly view 
</details>

### [RoadMap]

* Add a list of remarkable days (specific background). The user could send the holidays for example

## Weekly time-range selection

`WeekView` can optionally select a time range on the empty grid or hour scale. Enable the interaction and handle `RangeSelected` in the host application:

```razor
<WeekView EnableRangeSelection="true"
          RangeSelected="OnRangeSelected"
          DayHeaderFormat="dddd"
          WeekStartsOn="DayOfWeek.Monday" />

@code {
    private void OnRangeSelected(TimeRangeSelectedParameter range)
    {
        // Open an event editor or update a recurring weekday work-plan draft.
        // range.Start is inclusive; range.End is exclusive.
    }
}
```

Press the primary mouse button on a 15-minute slot, drag within the same day, and release on the last included slot. Dragging upward normalizes the range. For example, selecting 09:00 through the 10:45 slot returns `[09:00, 11:00)`. A single slot returns 15 minutes. The final 23:45 slot ends at midnight on the following day. The selected range is highlighted while dragging.

Escape (with the grid focused), leaving the grid, pointer cancellation or crossing into another day cancels the selection. Releasing outside a selectable slot also cancels it. Existing task click/drag callbacks are retained; selecting a range never inserts, modifies or persists tasks. The host decides what the callback means. Desktop mouse interaction is tested; touch-specific capture/gesture handling is not included.

The feature defaults to disabled and requires a `RangeSelected` delegate. `DayHeaderFormat` defaults to `ddd dd.MM`, preserving the existing dated header; `dddd` shows full localized weekday names without dates. `WeekStartsOn` can explicitly select Monday (or another weekday), independent of the operating system culture; its default is null and retains the culture convention. Times are wall-clock `DateTime` values, with no timezone conversion performed by the component.

### WeekView: sticky headers and off-screen event counts

```razor
<WeekView EnableHourScrolling="true" VisibleStartHour="6" VisibleEndHour="17"
          HeaderBackgroundColor="#d2e4f7" HeaderBorderColor="#9fb3c4"
          ShowOverflowIndicators="true" />
```

Scrolling is opt-in, preserving the previous full-day layout by default. The initial viewport shows 06:00–17:00; all 24 hours remain in the grid and can be reached by scrolling. Headers stay visible. Each day shows `▲ count` on the left and `▼ count` on the right only for nonzero counts; its title stays centered. Counts include events partially clipped at either boundary (a long/all-day event may count in both directions), update after scrolling, resizing and task changes, and are based on rendered event bounds. Click or keyboard-activate a button to scroll to the nearest hidden event edge. A single shared viewport keeps all weekdays aligned.

Optional header colors augment the existing `HeaderStyle`; unset colors retain prior styling. The visible range must satisfy `0 <= VisibleStartHour < VisibleEndHour <= 24`. JS interop runs after rendering, supports independent calendar instances, and disconnects listeners/observers on disposal. The WebAssembly weekly example enables these options. If the host limits the component height, the visible interval shrinks; counters still reflect the actual viewport.

### WebAssembly time-range selection demo

The existing `samples/BlazorWebAssembly` project demonstrates weekly range selection at `/weeklyview` (`Pages/Weekly.razor`). Range mode starts enabled; drag over 15-minute slots in the time scale or empty grid within one weekday. The status area shows the selected date, start, exclusive end and duration. Click **Add demo event** to append a sample task in memory. Repeated selections can create multiple sample events. **Clear demo events** removes only events created by this demo, preserving the original fixtures; reload clears the in-memory changes.

Disable **Select a time range** to use the existing task dragging demo. Changing the displayed week/date or toggling range mode clears the pending range so an old selection cannot be added accidentally. The example wires `EnableRangeSelection`, `RangeSelected` and `WeekStartsOn=Monday`; it does not change the library, use a database or call an external calendar API. It uses the existing component's hour-based task positioning; the exact selected quarter-hour times are retained in the task data and caption.

Run locally from the repository root:

```sh
dotnet run --project samples/BlazorWebAssembly/BlazorWebAssembly.csproj --framework net8.0
```

Open the local URL printed by the development server and navigate to Weekly view.

### WeekView: configurable current-day appearance

```razor
<WeekView HighlightToday="true"
          TodayBackgroundColor="#f5f5f5"
          TodayBorderColor="#ff5b60" />
```

Both new colors are optional and apply only when `HighlightToday` is enabled and today's date is in the rendered week. The background covers the full hour grid, including the time scale and empty overlap columns; task colors and header styles remain unchanged. The border is a 3px inset outline around the whole day column and does not change layout dimensions. Leaving either parameter unset preserves the previous appearance for that part. An explicit today background takes precedence over weekend and highlighted-period backgrounds only for today.
