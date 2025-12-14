
using System.Xml.Linq;

namespace RTree.Implementations;

public partial class BTree<T>
{ 
    public int Count { get; private set; }
    public int Height { get; private set; }

    public T? Min => _root == null || Count == 0 ? default! : GetMin(_root);
    public T? Max => _root == null || Count == 0 ? default! : GetMax(_root);

    private T GetMax(BTreeNode<T> node)
    {
        while (!node.IsLeaf)
        {
            node = node.Children[^1]!;
        } 
        return node.Keys[^1];
    }

    private T GetMin(BTreeNode<T> node)
    {
        while (!node.IsLeaf)
        {
            node = node.Children[0]!;
        }
        return node.Keys[0];
    }
}
