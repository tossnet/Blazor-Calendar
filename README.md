# Blazor-Calendar
[![NuGet](https://img.shields.io/nuget/v/BlazorCalendar.svg)](https://www.nuget.org/packages/BlazorCalendar/)  ![BlazorCalendar Nuget Package](https://img.shields.io/nuget/dt/BlazorCalendar)
[![GitHub](https://img.shields.io/github/license/tossnet/Blazor-Calendar?color=594ae2&logo=github&style=flat-square)](https://github.com/tossnet/Blazor-Calendar/blob/main/LICENSE)

For Blazor Server or Blazor WebAssembly

## Live demo
Blazor webassembly : https://tossnet.github.io/Blazor-Calendar/monthlyview

![blazorcalendar201](https://user-images.githubusercontent.com/3845786/158783479-35e614fe-fcca-4162-8e64-b5b33338251d.gif)

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
https://github.com/tossnet/Blazor-Calendar/wiki



## <a name="ReleaseNotes"></a>Release Notes

Blazor-Calendar/pull/13
<details open="open"><summary>Version 2.6.5</summary>

>- Add WeekView (thanks [BruderJohn](https://github.com/BruderJohn) )  [Pull #11](https://github.com/tossnet/Blazor-Calendar/pull/13)
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

* set a customizable background color for the current day
* Add a list of remarkable days (specific background). The user could send the holidays for example
