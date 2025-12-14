using RTree.Interfaces;
using RTree.Literals;
using System.Collections;

namespace RTree.Implementations;

public class BinaryTree<T>(DuplicateHandling _duplicateHandling = DuplicateHandling.None) : IBinaryTree<T> where T : IComparable<T>
{
    private class Node(T value)
    {
        public T Value { get; set; } = value;
        public Node? Left { get; set; }
        public Node? Right { get; set; }
        public int Count { get; set; } = 1;
    }
    private Node? _root;
    public void Insert(T value) => _root = InsertRec(_root, value);

    private Node InsertRec(Node? node, T value)
    {
        if (node == null)
            return new Node(value);

        int comparison = value.CompareTo(node.Value);

        switch (_duplicateHandling)
        {
            case DuplicateHandling.None:
                if (comparison < 0)
                    node.Left = InsertRec(node.Left, value);
                else if (comparison > 0)
                    node.Right = InsertRec(node.Right, value);
                break;

            case DuplicateHandling.Allow:
                if (comparison < 0)
                    node.Left = InsertRec(node.Left, value);
                else
                    node.Right = InsertRec(node.Right, value);
                break;

            case DuplicateHandling.Count:
                if (comparison < 0)
                    node.Left = InsertRec(node.Left, value);
                else if (comparison > 0)
                    node.Right = InsertRec(node.Right, value);
                else
                    node.Count++;
                break;
        }

        return node;
    }

    public bool Contains(T value) => ContainsRec(_root, value);

    private bool ContainsRec(Node? node, T value)
    {
        if (node == null)
            return false;

        if (value.CompareTo(node.Value) == 0)
            return true;

        return value.CompareTo(node.Value) < 0
            ? ContainsRec(node.Left, value)
            : ContainsRec(node.Right, value);
    }

    public void Delete(T value) => _root = DeleteRec(_root, value);

    private Node? DeleteRec(Node? node, T value)
    {
        if (node == null)
            return null;

        int comparison = value.CompareTo(node.Value);

        if (comparison < 0)
            node.Left = DeleteRec(node.Left, value);
        else if (comparison > 0)
            node.Right = DeleteRec(node.Right, value);
        else
        {
            if (_duplicateHandling == DuplicateHandling.Count)
            {
                node.Count--;
                if (node.Count > 0)
                    return node;
            }

            if (node.Left == null)
                return node.Right;
            if (node.Right == null)
                return node.Left;

            node.Value = MinValue(node.Right);
            //Future fix: if the successor node has count > 1, its count should be decremented instead of being completely removed (currently, only the counter is decremented).
            node.Right = DeleteRec(node.Right, node.Value);
        }

        return node;
    }

    private T MinValue(Node node)
    {
        T minValue = node.Value;
        while (node.Left != null)
        {
            minValue = node.Left.Value;
            node = node.Left;
        }
        return minValue;
    }

    public IEnumerable<T> TraverseInOrder() => TraverseInOrderRec(_root);

    private IEnumerable<T> TraverseInOrderRec(Node? node)
    {
        if (node == null)
            yield break;

        foreach (var value in TraverseInOrderRec(node.Left))
            yield return value;

        for (int i = 0; i < (_duplicateHandling == DuplicateHandling.Count ? node.Count : 1); i++)
            yield return node.Value;

        foreach (var value in TraverseInOrderRec(node.Right))
            yield return value;

    }

    public IEnumerable<T> TraversePreOrder() => TraversePreOrderRec(_root);

    private IEnumerable<T> TraversePreOrderRec(Node? node)
    {
        if (node == null)
            yield break;

        yield return node.Value;

        foreach (var value in TraversePreOrderRec(node.Left))
            yield return value;

        foreach (var value in TraversePreOrderRec(node.Right))
            yield return value;
    }

    public IEnumerable<T> TraversePostOrder() => TraversePostOrderRec(_root);

    private IEnumerable<T> TraversePostOrderRec(Node? node)
    {
        if (node == null)
            yield break;

        foreach (var value in TraversePostOrderRec(node.Left))
            yield return value;

        foreach (var value in TraversePostOrderRec(node.Right))
            yield return value;

        yield return node.Value;
    }

    public IEnumerator<T> GetEnumerator()
    {
        return TraverseInOrder().GetEnumerator();
    }

}
