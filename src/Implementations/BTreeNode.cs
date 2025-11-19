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
    /// Si la clave NO se encuentra, retorna el índice donde debería insertarse, representado como el complemento bit a bit (~índice). 
    /// Este valor también indica el índice del puntero al nodo hijo que debemos seguir.
    /// </returns>
    public int FindKeyIndex(T value)
    {
        int index = Keys.BinarySearch(value);

        if (index >= 0)
        {
            return index;
        }

        return ~index;
    }
}
