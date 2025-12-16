using Xunit;
using RTree.Implementations;
using System.Linq;
using System.Collections.Generic;

namespace RTree.Test;

public class BasicOperationsTests
{
    [Fact]
     public void Insert_SingleItem_CountIsOne()
    {
        var tree = new BTree<int>(3);
        tree.Add(10);

        Assert.Single(tree);
        Assert.Contains(10, tree);
    }

    [Fact]
    public void Insert_MultipleItems_TriggersSplit_AndMaintainsOrder()
    {
        // Usamos orden 3. Max claves por nodo = 2.
        // Al insertar el 3er elemento, debe haber un Split.
        var tree = new BTree<int>(3);

        tree.Add(10);
        tree.Add(20);
        tree.Add(5);  // Esto debería causar split y reordenamiento
        tree.Add(15);

        Assert.Equal(4, tree.Count);

        // Verificamos que al recorrerlo, salgan ordenados
        var sortedList = tree.ToList();
        Assert.Equal(new[] { 5, 10, 15, 20 }, sortedList);
    }

    [Fact]
    public void Delete_LeafNode_DecreasesCount()
    {
        var tree = new BTree<int>(3);
        tree.Add(10);
        tree.Add(20);

        bool removed = tree.Remove(20);

        Assert.True(removed);
        Assert.Single(tree);
        Assert.DoesNotContain(20, tree);
    }

    [Fact]
    public void Delete_RootCausingHeightReduction_Works()
    {
        var tree = new BTree<int>(3);

        int[] inputs = { 10, 20, 30, 40, 50 };
        foreach (var i in inputs) tree.Add(i);

        tree.Remove(10);
        tree.Remove(20);
        tree.Remove(30);
        tree.Remove(40);
        tree.Remove(50);

        Assert.Empty(tree);
        Assert.Equal(0, tree.Height); // Altura base
    }

    [Fact]
    public void Clear_EmptiesTree()
    {
        var tree = new BTree<int>(3);
        for (int i = 0; i < 100; i++) tree.Add(i);

        tree.Clear();

        Assert.Empty(tree);
        Assert.Empty(tree);
    }
}