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

        public int NumLinkedHeaders
        {
            get
            {
                int count = 0;
                ColumnHeader currHeader = right;
                while(!currHeader.Equals(this))
                {
                    ++count;
                    currHeader = currHeader.right;
                }
                return count;
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
            header = this;
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
            Hide();
            //For each row below me, remove it from consideration
            Node currNode = below;

            while (!currNode.Equals(this))
            {
                currNode.HideLinkedNodesFromHeader();
                currNode = currNode.below;
            }
        }

        public void Uncover()
        {
            Node currNode = above;
            while (!currNode.Equals(this))
            {
                currNode.ShowLinkedNodesToHeader();
                currNode = currNode.above;
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
            Search(root, ref result, ref isDone);
            return result;
        }

        private static void Search(ColumnHeader root, ref Stack<Node> solution, ref bool done)
        {
            if (root.right == root)
            {
                done = true;
                return;
            }
            else
            {
                ColumnHeader chosenColumn = ChooseColumnHeader(root);
                Node chosenRow = chosenColumn.below;
                //for each row in the chosen column
                while (!chosenRow.Equals(chosenColumn))
                {
                    //add the row to our solution
                    solution.Push(chosenRow);
                    //for each column our row inhabits
                    Node nextNode = chosenRow;
                    do
                    {
                        //remove the column and the rows it contains from consideration
                        nextNode.header.Cover();
                        //move to the next column
                        nextNode = nextNode.right;
                    } while (!nextNode.Equals(chosenRow));
                    //search here either successful, or our row was wrong
                    Search(root, ref solution, ref done);
                    //Pop data object
                    if (done) return;
                    //remove the row from the solution
                    chosenRow = solution.Pop();
                    chosenColumn = chosenRow.header;
                    nextNode = chosenRow.left;
                    //for each column we covered...uncover it
                    while (!nextNode.Equals(chosenRow))
                    {
                        nextNode.header.Uncover();
                        nextNode = nextNode.left;
                    }
                    chosenRow = chosenRow.below;
                }
                chosenColumn.Uncover();
                return;
            }
        }

        public bool Equals(ColumnHeader other)
        {
            return other.index == index;
        }
    }
}
