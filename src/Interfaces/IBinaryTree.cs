namespace RTree.Interfaces;

public interface IBinaryTree<T> : ITree<T> where T : IComparable<T>
{
    void Insert(T value);
    void Delete(T value);
    IEnumerable<T> TraversePreOrder();
    IEnumerable<T> TraversePostOrder();
}