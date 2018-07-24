using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    public class ColumnHeader : Node
    {

        public new ColumnHeader left;
        public new ColumnHeader right;

        private readonly int key;
        public int Key
        {
            get
            {
                return key;
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

        public ColumnHeader()
        {
            header = this;
            left = this;
            right = this;
        }

        public ColumnHeader(HeaderRoot root, int givenKey = 0)
        {
            key = givenKey;
            header = root;
            ColumnHeader last = root.left;
            last.right = this;
            right = root;
            root.left = this;
            left = last;
        }


        public void Hide()
        {

            left.right = right;
            right.left = left;
        }

        public void Unhide()
        {

            right.left = this;
            left.right = this;
        }

        public void Cover()
        {
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

    }
}
