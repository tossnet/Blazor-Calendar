using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Microsoft.VSDiagnostics;

namespace BlazorCalendar.Benchmarks;

/// <summary>
/// Benchmark comparing task lookup strategies for AnnualView calendar rendering.
/// Original approach: O(days × tasks) - linear search for each day
/// Dictionary approach: O(days + tasks) - pre-indexed lookup
/// </summary>
[MemoryDiagnoser]
[CPUUsageDiagnoser]
public class TaskLookupBenchmarks
{
    private TaskData[] _tasks = null!;
    private DateTime _startDate;
    private Dictionary<DateTime, List<TaskData>>? _tasksByDate;

    // Simule 12 mois × 31 jours = 372 cellules (vue annuelle)
    private const int DaysCount = 372;

    [Params(10, 100, 500, 1000)]
    public int TaskCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _startDate = new DateTime(2025, 1, 1);
        var random = new Random(42); // Seed fixe pour reproductibilité

        _tasks = new TaskData[TaskCount];
        for (int i = 0; i < TaskCount; i++)
        {
            var startOffset = random.Next(0, 365);
            var duration = random.Next(1, 8); // Tâches de 1 à 7 jours

            _tasks[i] = new TaskData
            {
                ID = i,
                DateStart = _startDate.AddDays(startOffset),
                DateEnd = _startDate.AddDays(startOffset + duration),
                Code = $"TASK-{i}",
                Caption = $"Task {i}",
                Color = "#FF5733"
            };
        }

        // Pré-construction de l'index pour le benchmark Dictionary
        BuildTaskIndex();
    }

    private void BuildTaskIndex()
    {
        _tasksByDate = new Dictionary<DateTime, List<TaskData>>();

        foreach (var task in _tasks)
        {
            for (var date = task.DateStart.Date; date <= task.DateEnd.Date; date = date.AddDays(1))
            {
                if (!_tasksByDate.TryGetValue(date, out var list))
                {
                    list = new List<TaskData>(4);
                    _tasksByDate[date] = list;
                }
                list.Add(task);
            }
        }
    }

    /// <summary>
    /// Approche actuelle : pour chaque jour, parcourir TOUTES les tâches
    /// Complexité : O(jours × tâches)
    /// </summary>
    [Benchmark(Baseline = true)]
    public int OriginalLinearSearch()
    {
        int tasksFound = 0;
        string borderStyle;

        for (int dayIndex = 0; dayIndex < DaysCount; dayIndex++)
        {
            var currentDate = _startDate.AddDays(dayIndex);
            int tasksCounter = 0;
            int taskID = -1;
            borderStyle = string.Empty;

            // Simulation de la boucle originale
            for (int k = 0; k < _tasks.Length; k++)
            {
                var t = _tasks[k];

                if (t.DateStart.Date <= currentDate.Date && currentDate.Date <= t.DateEnd.Date)
                {
                    taskID = t.ID;
                    tasksCounter++;

                    // Simulation du calcul de borderStyle
                    if (t.DateStart.Date == currentDate.Date && t.DateEnd.Date != currentDate.Date)
                        borderStyle = "border-top";
                    else if (t.DateEnd.Date == currentDate.Date && t.DateStart.Date != currentDate.Date)
                        borderStyle = "border-bottom";
                    else if (t.DateEnd.Date == currentDate.Date && t.DateStart.Date == currentDate.Date)
                        borderStyle = "border-top border-bottom";
                }
            }

            if (tasksCounter >= 1)
                tasksFound++;
        }

        return tasksFound;
    }

    /// <summary>
    /// Approche optimisée : pré-indexation + lookup O(1)
    /// Complexité : O(jours + tâches) pour la construction, O(1) par lookup
    /// </summary>
    [Benchmark]
    public int DictionaryLookup()
    {
        int tasksFound = 0;
        string borderStyle;

        for (int dayIndex = 0; dayIndex < DaysCount; dayIndex++)
        {
            var currentDate = _startDate.AddDays(dayIndex);
            int tasksCounter = 0;
            int taskID = -1;
            borderStyle = string.Empty;

            // Lookup O(1)
            if (_tasksByDate!.TryGetValue(currentDate.Date, out var tasksForDay))
            {
                tasksCounter = tasksForDay.Count;

                foreach (var t in tasksForDay)
                {
                    taskID = t.ID;

                    // Simulation du calcul de borderStyle
                    if (t.DateStart.Date == currentDate.Date && t.DateEnd.Date != currentDate.Date)
                        borderStyle = "border-top";
                    else if (t.DateEnd.Date == currentDate.Date && t.DateStart.Date != currentDate.Date)
                        borderStyle = "border-bottom";
                    else if (t.DateEnd.Date == currentDate.Date && t.DateStart.Date == currentDate.Date)
                        borderStyle = "border-top border-bottom";
                }
            }

            if (tasksCounter >= 1)
                tasksFound++;
        }

        return tasksFound;
    }

    /// <summary>
    /// Benchmark incluant la construction de l'index (cas réaliste lors d'un changement de TasksList)
    /// </summary>
    [Benchmark]
    public int DictionaryWithIndexBuild()
    {
        // Reconstruction de l'index (simule OnParametersSet)
        var tasksByDate = new Dictionary<DateTime, List<TaskData>>();

        foreach (var task in _tasks)
        {
            for (var date = task.DateStart.Date; date <= task.DateEnd.Date; date = date.AddDays(1))
            {
                if (!tasksByDate.TryGetValue(date, out var list))
                {
                    list = new List<TaskData>(4);
                    tasksByDate[date] = list;
                }
                list.Add(task);
            }
        }

        // Puis lookup
        int tasksFound = 0;
        string borderStyle;

        for (int dayIndex = 0; dayIndex < DaysCount; dayIndex++)
        {
            var currentDate = _startDate.AddDays(dayIndex);
            int tasksCounter = 0;
            int taskID = -1;
            borderStyle = string.Empty;

            if (tasksByDate.TryGetValue(currentDate.Date, out var tasksForDay))
            {
                tasksCounter = tasksForDay.Count;

                foreach (var t in tasksForDay)
                {
                    taskID = t.ID;

                    if (t.DateStart.Date == currentDate.Date && t.DateEnd.Date != currentDate.Date)
                        borderStyle = "border-top";
                    else if (t.DateEnd.Date == currentDate.Date && t.DateStart.Date != currentDate.Date)
                        borderStyle = "border-bottom";
                    else if (t.DateEnd.Date == currentDate.Date && t.DateStart.Date == currentDate.Date)
                        borderStyle = "border-top border-bottom";
                }
            }

            if (tasksCounter >= 1)
                tasksFound++;
        }

        return tasksFound;
    }
}

/// <summary>
/// Simplified task model for benchmarking (mirrors BlazorCalendar.Models.Tasks)
/// </summary>
public sealed class TaskData
{
    public int ID { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Caption { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public DateTime DateStart { get; set; }
    public DateTime DateEnd { get; set; }
}
