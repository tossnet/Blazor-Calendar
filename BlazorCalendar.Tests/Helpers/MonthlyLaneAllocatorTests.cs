using BlazorCalendar.Helpers;

namespace BlazorCalendar.Tests.Helpers;

public sealed class MonthlyLaneAllocatorTests
{
    [Fact]
    public void Allocate_SameDay_FillsLanesInOrder()
    {
        var allocator = new MonthlyLaneAllocator(4);

        Assert.Equal(0, allocator.Allocate(10, 10));
        Assert.Equal(1, allocator.Allocate(10, 10));
        Assert.Equal(2, allocator.Allocate(10, 10));
        Assert.Equal(3, allocator.Allocate(10, 10));
        Assert.Equal(0, allocator.HiddenOn(10));
    }

    [Fact]
    public void Allocate_DisjointDays_ReuseFirstLane()
    {
        var allocator = new MonthlyLaneAllocator(4);

        Assert.Equal(0, allocator.Allocate(1, 3));
        Assert.Equal(0, allocator.Allocate(4, 6));
    }

    [Fact]
    public void Allocate_MultiDay_SkipsLaneBusyOnALaterDay()
    {
        var allocator = new MonthlyLaneAllocator(4);
        allocator.Allocate(12, 12); // lane 0 busy on day 12 only

        int lane = allocator.Allocate(10, 13);

        Assert.Equal(1, lane);
    }

    [Fact]
    public void Allocate_AllLanesFull_ReturnsMinusOneAndCountsEverySpannedDay()
    {
        var allocator = new MonthlyLaneAllocator(2);
        allocator.Allocate(5, 5);
        allocator.Allocate(5, 5);

        Assert.Equal(-1, allocator.Allocate(4, 6));
        Assert.Equal(-1, allocator.Allocate(5, 5));

        Assert.Equal(1, allocator.HiddenOn(4));
        Assert.Equal(2, allocator.HiddenOn(5));
        Assert.Equal(1, allocator.HiddenOn(6));
        Assert.Equal(0, allocator.HiddenOn(7));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    public void Constructor_LanesBelowOne_ClampsToOne(int lanes)
    {
        var allocator = new MonthlyLaneAllocator(lanes);

        Assert.Equal(1, allocator.Lanes);
        Assert.Equal(0, allocator.Allocate(31, 31));
        Assert.Equal(-1, allocator.Allocate(31, 31));
        Assert.Equal(1, allocator.HiddenOn(31));
    }
}
