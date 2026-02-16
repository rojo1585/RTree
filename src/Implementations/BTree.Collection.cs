using System.Collections;

namespace RTree.Implementations;

public partial class BTree<T> : ICollection<T>
{

    public bool IsReadOnly => false;
  

    public void Add(T item) => Insert(item);

    public void Clear()
    {
        _root = null;
        Count = 0;
        Height = 0;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        ArgumentNullException.ThrowIfNull(array);
        ArgumentOutOfRangeException.ThrowIfNegative(arrayIndex);
        if (array.Length - arrayIndex < Count) throw new ArgumentException("The target array is too small.");

        foreach (var item in this)
            array[arrayIndex++] = item;
    }

    public bool Remove(T item)
    {
        if (!Contains(item)) return false;

        Delete(item);
        return true;
    }

    public IEnumerator<T> GetEnumerator() => TraverseInOrder().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
