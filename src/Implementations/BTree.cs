using RTree.Interfaces;

namespace RTree.Implementations;

public partial class BTree<T> : ITree<T> where T : IComparable<T>
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
    public bool Contains(T value) => TryGetValue(value, out _);
    public void Insert(T value)
    {
        // fix if root is null, create a new root

        BTreeNode<T> r = _root!;

        if (r.Keys.Count == MaxKeys)
        {
            BTreeNode<T> s = new() { IsLeaf = false };
            s.Children.Add(r);
            _root = s;

            SplitChild(s, 0);

            InsertNonFull(s, value);
        }
        else
        {
            InsertNonFull(r, value);
        }
    }

    public void Delete(T value)
    {
        if (_root == null) return;

        if (!Contains(value)) return;

        DeleteRec(_root, value);

        if (_root.Keys.Count == 0)
        {
            if (_root.IsLeaf)
                _root = null;
            else
                _root = _root.Children.FirstOrDefault();

        }
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

    private void InsertNonFull(BTreeNode<T> node, T value)
    {
        int i = node.Keys.Count - 1;

        if (node.IsLeaf)
        {
            while (i >= 0 && value.CompareTo(node.Keys[i]) < 0)
                i--;

            node.Keys.Insert(i + 1, value);
        }
        else
        {
            while (i >= 0 && value.CompareTo(node.Keys[i]) < 0)
                i--;

            int childIndex = i + 1;

            BTreeNode<T> child = node.Children[childIndex]!;

            if (child.Keys.Count == MaxKeys)
            {
                SplitChild(node, childIndex);

                if (value.CompareTo(node.Keys[childIndex]) > 0)
                    childIndex++;
            }

            InsertNonFull(node.Children[childIndex]!, value);
        }
    }
    public IEnumerator<T> GetEnumerator() => TraverseInOrder().GetEnumerator();
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

    private T GetPredecessor(BTreeNode<T> node, int index)
    {
        BTreeNode<T> current = node.Children[index]!;

        while (!current.IsLeaf)
            current = current.Children[current.Keys.Count]!;

        return current.Keys.Last();
    }

    private T GetSuccessor(BTreeNode<T> node, int index)
    {
        BTreeNode<T> current = node.Children[index + 1]!;

        while (!current.IsLeaf)
            current = current.Children[0]!;

        return current.Keys.First();
    }

    private void BorrowFromPrevious(BTreeNode<T> node, int index)
    {
        BTreeNode<T> child = node.Children[index]!;
        BTreeNode<T> sibling = node.Children[index - 1]!;

        child.Keys.Insert(0, node.Keys[index - 1]);

        node.Keys[index - 1] = sibling.Keys.Last();

        if (!child.IsLeaf)
        {
            child.Children.Insert(0, sibling.Children.Last());
            sibling.Children.RemoveAt(sibling.Children.Count - 1);
        }

        sibling.Keys.RemoveAt(sibling.Keys.Count - 1);
    }

    private void BorrowFromNext(BTreeNode<T> node, int index)
    {
        BTreeNode<T> child = node.Children[index]!;
        BTreeNode<T> sibling = node.Children[index + 1]!;

        child.Keys.Add(node.Keys[index]);

        node.Keys[index] = sibling.Keys.First();

        if (!child.IsLeaf)
        {
            child.Children.Add(sibling.Children.First());
            sibling.Children.RemoveAt(0);
        }

        sibling.Keys.RemoveAt(0);
    }

    private void Merge(BTreeNode<T> node, int index)
    {
        BTreeNode<T> child = node.Children[index]!;
        BTreeNode<T> sibling = node.Children[index + 1]!;

        child.Keys.Add(node.Keys[index]);

        child.Keys.AddRange(sibling.Keys);

        if (!child.IsLeaf)
            child.Children.AddRange(sibling.Children);

        node.Keys.RemoveAt(index);

        node.Children.RemoveAt(index + 1);
    }

    private void Fill(BTreeNode<T> node, int index)
    {
        if (index != 0 && node.Children[index - 1]!.Keys.Count > MinKeys)
            BorrowFromPrevious(node, index);
        else if (index != node.Keys.Count && node.Children[index + 1]!.Keys.Count > MinKeys)
            BorrowFromNext(node, index);
        else
        {
            if (index != node.Keys.Count)
                Merge(node, index);
            else
                Merge(node, index - 1);
        }
    }

    private void DeleteRec(BTreeNode<T> node, T value)
    {
        int index = node.FindKeyIndex(value);

        if (index >= 0)
        {
            if (node.IsLeaf)
                node.Keys.RemoveAt(index);
            else
            {
                if (node.Children[index]!.Keys.Count > MinKeys)
                {
                    T pred = GetPredecessor(node, index);
                    node.Keys[index] = pred;
                    DeleteRec(node.Children[index]!, pred);
                }
                else if (node.Children[index + 1]!.Keys.Count > MinKeys)
                {
                    T succ = GetSuccessor(node, index);
                    node.Keys[index] = succ;
                    DeleteRec(node.Children[index + 1]!, succ);
                }
                else
                {
                    Merge(node, index);
                    DeleteRec(node.Children[index]!, value);
                }
            }
        }
        else
        {
            int childIndex = ~index;

            if (node.IsLeaf)
                return;

            if (node.Children[childIndex]!.Keys.Count == MinKeys)
                Fill(node, childIndex);

            index = node.FindKeyIndex(value);
            childIndex = ~index;
            DeleteRec(node.Children[childIndex]!, value);
        }
    }

   
}
