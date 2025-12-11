using RTree.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTree.Implementations;

public class BTree<T> : ITree<T> where T : IComparable<T>
{
    private BTreeNode<T>? _root;
    private readonly int _order;

    private int MaxKeys => _order - 1;
    private int MinKeys => (int)Math.Ceiling((double)_order / 2) - 1;
    public BTree(int order = 5)
    {
        if (order < 3)
            throw new ArgumentOutOfRangeException(nameof(order), "The order of the B-Tree must be greater than or equal to 3.");

        _order = order;

        _root = new BTreeNode<T> { IsLeaf = true };
    }

    public void Insert(T value)
    {
        throw new NotImplementedException();
    }

    public bool Contains(T value)
    {
        return Search(_root, value);
    }

    private bool Search(BTreeNode<T>? node, T value)
    {
        if (node == null)
            return false;

        int index = node.FindKeyIndex(value);

        if (index >= 0)
        {
            return true;
        }

        if (!node.IsLeaf)
        {
            // The branch index to follow is ~index (bitwise complement)
            int childIndex = ~index;
            return Search(node.Children[childIndex], value);
        }

        return false; 
    }

    public void Delete(T value)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<T> TraverseInOrder()
    {
        return TraverseInOrderRec(_root);
    }

    private IEnumerable<T> TraverseInOrderRec(BTreeNode<T>? node)
    {
        if (node == null) yield break;

        for (int i = 0; i < node.Keys.Count; i++)
        {
            if (!node.IsLeaf)
            {
                foreach (var val in TraverseInOrderRec(node.Children[i]))
                    yield return val;
            }

            yield return node.Keys[i];
        }

        if (!node.IsLeaf)
        {
            foreach (var val in TraverseInOrderRec(node.Children[node.Keys.Count]))
                yield return val;
        }
    }

    private void SplitChild(BTreeNode<T> parent, int childIndex)
    {
        BTreeNode<T> y = parent.Children[childIndex]!;
        BTreeNode<T> z = new() { IsLeaf = y.IsLeaf };

        int medianIndex = MaxKeys / 2;

        z.Keys.AddRange(y.Keys.GetRange(medianIndex + 1, MaxKeys - medianIndex - 1));

        if (!y.IsLeaf)
            z.Children.AddRange(y.Children.GetRange(medianIndex + 1, _order - (medianIndex + 1)));

        T medianKey = y.Keys[medianIndex];
        parent.Keys.Insert(childIndex, medianKey); 

        parent.Children.Insert(childIndex + 1, z);

        y.Keys.RemoveRange(medianIndex, y.Keys.Count - medianIndex);
        if (!y.IsLeaf)
            y.Children.RemoveRange(medianIndex + 1, y.Children.Count - (medianIndex + 1));

    }
    public IEnumerator<T> GetEnumerator() => TraverseInOrder().GetEnumerator();
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
}
