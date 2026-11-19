namespace BlazorCalendar.Helpers;

/// <summary>
/// Assigns month tasks to vertical lanes inside the day cells of the MonthlyView.
/// A task gets the first lane that is free on every day it spans; when none is free,
/// it is hidden and counted on each of those days (rendered as a "+N" badge).
/// </summary>
internal sealed class MonthlyLaneAllocator
{
    // Index 0 unused: days of month are 1..31.
    private const int DaySlots = 32;

    private readonly bool[,] occupied;
    private readonly int[] hidden = new int[DaySlots];

    public MonthlyLaneAllocator(int lanes)
    {
        Lanes = Math.Max(1, lanes);
        occupied = new bool[Lanes, DaySlots];
    }

    public int Lanes { get; }

    /// <summary>
    /// Reserves the first lane free from <paramref name="startDay"/> to <paramref name="endDay"/> (inclusive).
    /// Returns the lane index, or -1 when the task cannot be displayed.
    /// </summary>
    public int Allocate(int startDay, int endDay)
    {
        for (int lane = 0; lane < Lanes; lane++)
        {
            if (!IsFree(lane, startDay, endDay))
                continue;

            for (int d = startDay; d <= endDay; d++)
            {
                occupied[lane, d] = true;
            }
            return lane;
        }

        for (int d = startDay; d <= endDay; d++)
        {
            hidden[d]++;
        }
        return -1;
    }

    /// <summary>Number of tasks that could not be displayed on this day.</summary>
    public int HiddenOn(int day) => hidden[day];

    private bool IsFree(int lane, int startDay, int endDay)
    {
        for (int d = startDay; d <= endDay; d++)
        {
            if (occupied[lane, d])
                return false;
        }
        return true;
    }
}
