using Xunit;
using RTree.Implementations;
using System.Linq;

namespace RTree.Test;

public class FeatureTests
{
    [Fact]
    public void RangeSearch_ReturnsCorrectElements()
    {
        var tree = new BTree<int>(5); 
        for (int i = 0; i < 100; i += 10) tree.Add(i);

        var range = tree.RangeSearch(25, 75).ToList();

        Assert.Equal(5, range.Count);
        Assert.Equal(30, range.First());
        Assert.Equal(70, range.Last());
        Assert.DoesNotContain(20, range);
        Assert.DoesNotContain(80, range);
    }

    [Fact]
    public void Linq_Integration_Works()
    {
        var tree = new BTree<int>(5) { 5, 3, 8, 1, 9, 2 };

        var pares = tree.Where(x => x % 2 == 0).ToList();

        Assert.Equal(2, pares.Count);
        Assert.Contains(2, pares);
        Assert.Contains(8, pares);
    }

    [Fact]
    public void TryGetValue_RetrievesActualObject()
    {

        var tree = new BTree<string>(3)
        {
            "Manzana",
            "Pera"
        };

        bool found = tree.TryGetValue("Pera", out string? val);
        Assert.True(found);
        Assert.Equal("Pera", val);

        bool notFound = tree.TryGetValue("Uva", out _);
        Assert.False(notFound);
    }
}
