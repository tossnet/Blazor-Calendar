# Blazor-Calendar
[![NuGet](https://img.shields.io/nuget/v/BlazorCalendar.svg)](https://www.nuget.org/packages/BlazorCalendar/)  ![BlazorCalendar Nuget Package](https://img.shields.io/nuget/dt/BlazorCalendar)


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

### Version 2.2.0

- fix bug
- added the NotBeDraggable property

### Version 2.1.0

- css style improvement
- Addition of hatching in the cells at the end of the month
- Add a new view called MonthlyView

### Version 2.0.0
⚠️Breaking


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

### [RoadMap]

* Add a second monthly view
