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
        public Node down;
        public Node up;
        public ColumnHeader header;
        public readonly int rowID;

        public Node()
        {
            rowID = nextRowID++;
            up = this;
            down = this;
            left = this;
            right = this;
        }

        public Node(ColumnHeader column)
        {
            header = column;
            rowID = nextRowID++;
            Node last = column.up;
            column.up = this;
            up = last;
            down = column;
            last.down = this;
            left = this;
            right = this;
        }



        public void RemoveRow()
        {
            //For each linked column
            Node linkedNode = right;
            while (!linkedNode.Equals(this))
            {
                //Remove the row from its list
                linkedNode.up.down = linkedNode.down;
                linkedNode.down.up = linkedNode.up;
                //Adjust the lists size
                --linkedNode.header.Size;
                //Go to the next node
                linkedNode = linkedNode.right;
            }
        }

        public void AddRow()
        {
            //For each column in this row
            Node linkedNode = left;
            while (!linkedNode.Equals(this))
            {
                if (linkedNode.up.down == this) continue;
                //Add the node back to the list
                linkedNode.up.down = linkedNode;
                if (linkedNode.down.up == this) continue;
                linkedNode.down.up = linkedNode;
                //Adjust the headers size
                ++linkedNode.header.Size;
                //Go to the next node
                linkedNode = linkedNode.left;
            }
        }

        public List<ColumnHeader> GetLinkedHeaders()
        {
            List<ColumnHeader> results = new List<ColumnHeader>();
            results.Add(header);
            Node linked = right;
            while(linked != this)
            {
                results.Add(linked.header);
                linked = linked.right;
            }
            return results;
        }

        public virtual void Select()
        {
            //When I am selected, I:
            // Go through my linked nodes and ask their category to cover themselves
            Node linked = right;
            while (linked!= this)
            {
                linked.header.Cover();
                linked = linked.right;
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

        internal void Unselect()
        {
            Node linked = left;
            while (linked!= this)
            {
                linked.header.Uncover();
                linked = linked.left;
            }
        }
    }

}
