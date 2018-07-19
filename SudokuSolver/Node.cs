using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{

    public class Node : IEquatable<Node>
    {
        private static int nextRowID = 0;
        public Node left;
        public Node right;
        public Node below;
        public Node above;
        public ColumnHeader header;
        public readonly int rowID;

        public Node()
        {
            rowID = nextRowID++;
            above = this;
            below = this;
            left = this;
            right = this;
        }

        public Node(ColumnHeader column)
        {
            header = column;
            rowID = nextRowID++;
            Node last = column.above;
            column.above = this;
            above = last;
            below = column;
            last.below = this;
            left = this;
            right = this;
        }



        public void HideLinkedNodesFromHeader()
        {
            Node linkedNode = right;
            while (!linkedNode.Equals(this))
            {
                above.below = below;
                below.above = above;
                --linkedNode.header.Size;
                linkedNode = linkedNode.right;
            }
        }

        public void ShowLinkedNodesToHeader()
        {
            Node linkedNode = left;
            while (!linkedNode.Equals(this))
            {
                above.below = this;
                below.above = this;
                ++linkedNode.header.Size;
                linkedNode = linkedNode.left;
            }
        }

        public static void LinkNodes(params Node[] nodes)
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                int rightIndex = (i + 1) % nodes.Length;
                int leftIndex = (i - 1) < 0 ? nodes.Length - 1 : i - 1;
                nodes[i].right = nodes[rightIndex];
                nodes[i].left = nodes[leftIndex];
            }
        }

        public bool Equals(Node other)
        {
            return other.rowID == rowID;
        }
    }

}
