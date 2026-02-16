using RTree.Implementations;
namespace RTree.Test;

public class StressTests
{
    [Fact]
    public void StressTest_RandomData_10000_Items()
    {
        int amount = 10000000;
        var tree = new BTree<int>(5);
        var random = new Random();

        var data = Enumerable.Range(0, amount)
                             .Select(x => random.Next(int.MinValue, int.MaxValue))
                             .Distinct()
                             .ToList();

        foreach (var num in data)
        {
            tree.Add(num);
        }

        Assert.Equal(data.Count, tree.Count);
        Assert.True(IsSorted(tree), "The tree did not maintain order after mass insertion.");

        foreach (var num in data)
        {
            Assert.True(tree.Contains(num), $"Failure to find the number {num}");
        }
        var a = data.Last();

        foreach (var num in data)
        {
            bool removed = tree.Remove(num);

            Assert.True(removed, $"Error deleting the number {num}");

        }

        Assert.Empty(tree);
    }

    [Fact]
    public void StressTest_SortedData_WorstCase_5000_Items()
    {

        int amount = 5000;
        var tree = new BTree<int>(4);

        for (int i = 0; i < amount; i++)
        {
            tree.Add(i);
        }

        Assert.Equal(amount, tree.Count);
        Assert.True(IsSorted(tree));

        for (int i = amount - 1; i >= 0; i--)
        {
            tree.Remove(i);
        }

        Assert.Empty(tree);
    }


    private bool IsSorted(IEnumerable<int> collection)
    {
        int? previous = null;
        foreach (var current in collection)
        {
            if (previous.HasValue && previous.Value > current)
                return false;
            previous = current;
        }
        return true;
    }
}
