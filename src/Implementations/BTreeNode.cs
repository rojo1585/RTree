using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTree.Implementations;
/// <summary>
/// Represents a node in the B-Tree.
/// </summary>
internal class BTreeNode<T> where T : IComparable<T>
{

    public List<T> Keys { get; set; } = [];
    public List<BTreeNode<T>?> Children { get; set; } = [];
    public bool IsLeaf { get; set; } = true;

    /// <summary>
    /// Utiliza búsqueda binaria para encontrar la posición de una clave dada.
    /// </summary>
    /// <param name="value">La clave a buscar.</param>
    /// <returns>
    /// Si la clave se encuentra, retorna su índice (>= 0).
    /// </returns>
    public int FindKeyIndex(T value)=> Keys.BinarySearch(value);

}
