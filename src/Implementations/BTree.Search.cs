using RTree.Interfaces;

namespace RTree.Implementations;

public partial class BTree<T>
{
    public bool TryGetValue(T value, out T? currentValue) => TryGetValueRec(_root, value, out currentValue);

    private bool TryGetValueRec(BTreeNode<T>? node, T value, out T? currentValue)
    {
        currentValue = default;

        if (node == null) return false;

        int index = node.FindKeyIndex(value);

        if (index >= 0)
        {
            currentValue = node.Keys[index];
            return true;
        }

        if (node.IsLeaf) return false;

        int childIndex = ~index;
        return TryGetValueRec(node.Children[childIndex], value, out currentValue);
    }

    public IEnumerable<T> RangeSearch(T min, T max) => RangeSearchRec(_root, min, max);

    private IEnumerable<T> RangeSearchRec(BTreeNode<T>? node, T min, T max)
    {
        if (node == null) yield break;

        int i = 0;

        while (i < node.Keys.Count && node.Keys[i].CompareTo(min) < 0)
        {
            i++;
        }

        while (i < node.Keys.Count)
        {
            if (node.Keys[i].CompareTo(max) > 0)
            {
                if (!node.IsLeaf)
                {
                    foreach (var item in RangeSearchRec(node.Children[i], min, max))
                        yield return item;
                }
                yield break;
            }

            if (!node.IsLeaf)
            {
                foreach (var item in RangeSearchRec(node.Children[i], min, max))
                    yield return item;
            }

            yield return node.Keys[i];
            i++;
        }

        if (!node.IsLeaf)
        {
            foreach (var item in RangeSearchRec(node.Children[i], min, max))
                yield return item;
        }
    }
}
