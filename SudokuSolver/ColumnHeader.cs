using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    public class ColumnHeader : Node, IEquatable<ColumnHeader>
    {
        private static int indexNo = 0;

        public new ColumnHeader left;
        public new ColumnHeader right;

        private int index;
        public int Index
        {
            get
            {
                return index;
            }
        }

        private int size = 0;

        public int Size
        {
            get
            {
                return size;
            }
            set
            {
                size = value;
            }
        }
        private bool isCovered = false;
        private int numLinked = 0;
        public int NumLinkedHeaders
        {
            get
            {
                if (header == this) return numLinked;
                if (!isCovered)
                {
                    return header.numLinked;
                }
                else return 0;
            }
        }

        public ColumnHeader()
        {
            index = indexNo++;
            header = this;
            left = this;
            right = this;
        }

        public ColumnHeader(ColumnHeader root)
        {
            index = indexNo++;
            ++root.numLinked;
            header = root;
            ColumnHeader last = root.left;
            last.right = this;
            right = root;
            root.left = this;
            left = last;
        }

        public void Hide()
        {
            --header.numLinked;
            left.right = right;
            right.left = left;
        }

        public void Unhide()
        {
            ++header.numLinked;
            right.left = this;
            left.right = this;
        }

        public void Cover()
        {
            //Prevent covering multiple times
            if (isCovered) return;
            isCovered = true;

            //Remove myself from the header list.
            Hide();
            //For each row I own, remove them from other headers lists
            Node row = down;
            while (!row.Equals(this))
            {
                row.RemoveRow();
                row = row.down;
            }
        }

        public void Uncover()
        {
            //Prevent from uncovering over and over
            if (isCovered == false) return;
            isCovered = false;

            //For each row I own, add them back to other headers lists
            Node row = up;
            while (!row.Equals(this))
            {
                row.AddRow();
                row = row.up;
            }
            Unhide();
        }

        public Node CreateNode()
        {
            ++size;
            return new Node(this);
        }

        public static ColumnHeader ChooseColumnHeader(ColumnHeader root)
        {
            ColumnHeader minHeader = root.right;
            ColumnHeader currHeader = root.right;
            while (!currHeader.Equals(root))
            {

                if (currHeader.size < minHeader.size)
                {
                    minHeader = currHeader;
                }
                currHeader = currHeader.right;
            }
            return minHeader;
        }

        public static Stack<Node> Search(ColumnHeader root)
        {
            Stack<Node> result = new Stack<Node>();
            bool isDone = false;
            Search(root, result, ref isDone);
            foreach (var node in result)
            {
                node.Unselect();
                node.header.Uncover();
            }
            return result;
        }

        private static void Search(ColumnHeader root, Stack<Node> solution, ref bool done)
        {
            if (root.right == root)
            {
                done = true;
                return;
            }
            else
            {
                ColumnHeader chosenColumn = ChooseColumnHeader(root);
                Node chosenRow = chosenColumn.down;
                chosenColumn.Cover();
                while (!chosenRow.Equals(chosenColumn))
                {
                    solution.Push(chosenRow);
                    chosenRow.Select();


                        Search(root, solution, ref done);
                    if (done) return;
                        chosenRow = solution.Pop();
                    chosenColumn = chosenRow.header;
                    chosenRow.Unselect();
                    chosenRow = chosenRow.down;
                }
                chosenColumn.Uncover();
                return;
            }
        }

        public bool Equals(ColumnHeader other)
        {
            return other.index == index;
        }

        // public override void Select()
        // {
        //     down.Select();
        //     Node otherRows = down.down;
        //     while(otherRows != this)
        //     {
        //         otherRows.RemoveRow();
        //     }
        // }
    }
}
