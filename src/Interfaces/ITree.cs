namespace RTree.Interfaces;

public interface ITree<T> where T : IComparable<T>
{
    bool Contains(T value);
    IEnumerable<T> TraverseInOrder();
}
