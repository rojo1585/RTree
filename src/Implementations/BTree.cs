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
        if (_root == null)
        {
            _root = new BTreeNode<T> { IsLeaf = true };
            _root.Keys.Add(value);
            Count= 1;
            Height = 1;
            return;
        }

        BTreeNode<T> r = _root!;

        if (r.Keys.Count == MaxKeys)
        {
            BTreeNode<T> s = new() { IsLeaf = false };
            s.Children.Add(r);
            _root = s;

            SplitChild(s, 0);
            InsertNonFull(s, value);
            Height++;
        }
        else
        {
            InsertNonFull(r, value);
        }

        Count++;
    }

    public void Delete(T value)
    {
        if (_root == null) return;

        if (!Contains(value)) return;

        DeleteRec(_root, value);

        if (_root.Keys.Count == 0)
        {
            if (_root.IsLeaf)
            {
                _root = null;
                Height = 0;
            }
            else
            {
                _root = _root.Children[0]!;
                Height--;
            }
        }
        Count--;
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
        else if (index < node.Children.Count - 1 && node.Children[index + 1]!.Keys.Count > MinKeys)
            BorrowFromNext(node, index);
        else
        {
            // Merge with sibling
            if (index != node.Children.Count - 1)
                Merge(node, index);  // Merge with right sibling
            else if (index > 0)
                Merge(node, index - 1);  // Merge with left sibling
        }
    }

    private void DeleteRec(BTreeNode<T> node, T value)
    {
        // ✅ CAMBIO 1: Usar BinarySearch para O(log MaxKeys)
        int index = node.Keys.BinarySearch(value);

        if (index >= 0)
        {
            // ✅ CAMBIO 2: Encontrado exactamente en la posición 'index'
            if (node.IsLeaf)
            {
                // Case 1: value está en una hoja
                node.Keys.RemoveAt(index);
            }
            else
            {
                // Case 2: value está en un nodo interno
                DeleteInternalNode(node, index, value);
            }
        }
        else if (!node.IsLeaf)
        {
            // ✅ CAMBIO 3: Usar ~index para obtener donde descender
            // BinarySearch devuelve negativo cuando no encuentra
            // ~index nos da la posición donde debería estar
            int childIndex = ~index;

            // ✅ CAMBIO 4: Llenar el hijo ANTES de descender
            if (node.Children[childIndex]!.Keys.Count < MinKeys + 1)
            {
                Fill(node, childIndex);

                // ✅ CAMBIO 5: Recalcular después de Fill
                // La estructura del árbol cambió, recalculamos donde descender
                index = node.Keys.BinarySearch(value);
                childIndex = ~index;
            }

            DeleteRec(node.Children[childIndex]!, value);
        }
    }

    private void DeleteInternalNode(BTreeNode<T> node, int i, T value)
    {
        T key = node.Keys[i];

        if (node.Children[i]!.Keys.Count > MinKeys)
        {
            // Case 2a: El hijo izquierdo tiene más de MinKeys claves
            T pred = GetPredecessor(node, i);
            node.Keys[i] = pred;
            DeleteRec(node.Children[i]!, pred);
        }
        else if (node.Children[i + 1]!.Keys.Count > MinKeys)
        {
            // Case 2b: El hijo derecho tiene más de MinKeys claves
            T succ = GetSuccessor(node, i);
            node.Keys[i] = succ;
            DeleteRec(node.Children[i + 1]!, succ);
        }
        else
        {
            // Case 2c: Ambos hijos tienen MinKeys claves; fusionarlos
            Merge(node, i);
            DeleteRec(node.Children[i]!, value);
        }
    }

   
}
