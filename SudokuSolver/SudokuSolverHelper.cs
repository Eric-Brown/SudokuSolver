using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    public static class SudokuSolverHelper
    {
        private const int SUDOKU_DIMENSION = 9;
        private const int NUM_COLUMN_HEADERS = 324;
        private const int NUM_HEADERS_PER_CATEGORY = 81;
        private const int BOX_DIMENSION = 3;
        static ColumnHeader root;
        static ColumnHeader[] ColumnHeaders;
        private class Node : IEquatable<Node>
        {
            private static int nextRowID = 0;
            protected Node below;
            protected Node above;
            private Node left;
            private Node right;
            private ColumnHeader header;
            private readonly int rowID;

            //Assumed that this constructor is ONLY used by ColumnHeader
            protected Node()
            {
                rowID = nextRowID++;
                below = this;
                above = this;
            }
            public Node(ColumnHeader column)
            {
                header = column;
                column.Add(this);
                rowID = nextRowID++;
                Node last = column.below;
                column.below = this;
                last.above = this;
                above = column;
                below = last;
            }

            public void Hide()
            {
                above.below = below;
                below.above = above;
            }

            public void Unhide()
            {
                above.below = this;
                below.above = this;
            }

            public void Select()
            {
                Node currNode = below;
                while (!currNode.Equals(this))
                {
                    currNode.RemoveLinkedNodesFromHeader();
                    currNode = currNode.below;
                }
            }

            private void RemoveLinkedNodesFromHeader()
            {
                Node linkedNode = right;
                while (!linkedNode.Equals(this))
                {
                    linkedNode.header.RemoveNode(linkedNode);
                    linkedNode = linkedNode.right;
                }
            }

            protected void UnhideRows()
            {
                Node currNode = above;
                while (!currNode.Equals(this))
                {
                    Node linkedNode = currNode.left;
                    while (!linkedNode.Equals(currNode))
                    {
                        ++linkedNode.size;
                        linkedNode.above.below = linkedNode;
                        linkedNode.below.above = linkedNode;
                        linkedNode = linkedNode.left;
                    }
                    currNode = currNode.above;
                }
            }

            public bool Equals(Node other)
            {
                return other.rowID == rowID;
            }
        }
        private class ColumnHeader : Node
        {
            private ColumnHeader PreviousHeader;
            private ColumnHeader NextHeader;
            private int size = 0;
            public int Size
            {
                get
                {
                    return size;
                }
            }
            public ColumnHeader()
            {
                header = this;
                PreviousHeader = this;
                NextHeader = this;
            }

            public ColumnHeader(ColumnHeader root)
            {
                ColumnHeader last = root.PreviousHeader;
                last.NextHeader = this;
                root.PreviousHeader = this;
                PreviousHeader = last;
                NextHeader = root;
            }

            public void InsertBehind(ColumnHeader column)
            {
                ColumnHeader temp = PreviousHeader;
                PreviousHeader = column;
                column.NextHeader = this;
                column.PreviousHeader = temp;
                temp.NextHeader = column;
            }

            public void Cover()
            {
                PreviousHeader.NextHeader = NextHeader;
                NextHeader.PreviousHeader = PreviousHeader;

            }

            public void Uncover()
            {
                UnhideRows();
                NextHeader.PreviousHeader = this;
                PreviousHeader.NextHeader = this;
            }

            public void Add(Node node)
            {
                ++size;
            }

            public void RemoveNode(Node toRemove)
            {
                toRemove.Hide();
                --size;
            }
        }


        static SudokuSolverHelper()
        {
            //create sudoku sparse matrix
            root = new ColumnHeader();

        }

        private enum HeaderArrayAccess
        {
            Cell = 0,
            Row = 1,
            Column = 2,
            Box = 3
        }
        
        private static void CreateSudokuConstraintMatrix()
        {
            //Stored in an array for O(1) access, but are still linked together for algorithm
            ColumnHeaders = CreateColumnHeaders();

            //729 Rows needed
            for (int row = 0; row < SUDOKU_DIMENSION; row++)
            {
                for (int col = 0; col < SUDOKU_DIMENSION; col++)
                {
                    //For each new cell in G, we create 4 data objects and link them together
                    //then these data objects are linked to their respective column header and inserted
                    for (int num = 0; num < SUDOKU_DIMENSION; num++)
                    {
                        //Each cell has 9 candidates
                        //This means that each cell has 9 rows
                        Node cellNode = new Node(LookUpColumnHeader(HeaderArrayAccess.Cell, row, col));
                        Node rowNode = new Node(LookUpColumnHeader(HeaderArrayAccess.Row, row, num));
                        Node colNode = new Node(LookUpColumnHeader(HeaderArrayAccess.Column, col, num));
                        Node boxNode = new Node(LookUpColumnHeader(HeaderArrayAccess.Box, GetBoxIndex(row, col), num));
                        LinkNodes(cellNode, rowNode, colNode, boxNode);
                    }
                }
            }
        }

        private static void LinkNodes(params Node[] nodes)
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                int rightIndex = (i + 1) % nodes.Length;
                int leftIndex = (i - 1) < 0 ? nodes.Length - 1 : i - 1;
                nodes[i].right = nodes[rightIndex];
                nodes[i].left = nodes[leftIndex];
            }
        }

        private static int GetBoxIndex(int row, int column)
        {
            return (row / BOX_DIMENSION) * BOX_DIMENSION + (column / BOX_DIMENSION);
        }

        private static ColumnHeader LookUpColumnHeader(HeaderArrayAccess category, int indexer, int offset)
        {
            return ColumnHeaders[(NUM_HEADERS_PER_CATEGORY * (int)category) + 
                (indexer * SUDOKU_DIMENSION) + 
                offset];
        }
        private static ColumnHeader[] CreateColumnHeaders()
        {
            //324 column objects
            //Cell Constraints 81 count
            //      Each cell (81 cells) must have only 1 value
            //Row Constraints 81 count
            //      Each row (9 rows) must have each number (9)
            //Column Constraints 81 count
            //      Each column (9 columns) must have each number (9)
            //Box Constraints 81 count
            //      Each box (9 boxes) must have each number (9)
            ColumnHeader[] headers = new ColumnHeader[NUM_COLUMN_HEADERS];
            for (int i = 0; i < NUM_COLUMN_HEADERS; i++)
            {
                headers[i] = new ColumnHeader(root);
            }
            return headers;
        }

        public static void Solve(SudokuPuzzle puzzle)
        {

        }
        private static void DLX()
        {
            List<Node> solution = new List<Node>();
            Search(root, 0, ref solution);
        }

        private static void Search(ColumnHeader root, int depth, ref List<Node> solution)
        {
            if(root.NextHeader == root)
            {
                //we have solution here
                return;
            }
            else
            {
                ColumnHeader chosenColumn = ChooseColumn(root);
                Node chosenNode = chosenColumn.above;
                while(!chosenNode.Equals(chosenColumn))
                {
                    solution.Add(chosenNode);
                    Node nextNode = chosenNode.right;
                    while(!nextNode.Equals(chosenNode))
                    {
                        nextNode.Column.Cover();
                        nextNode = nextNode.right;
                    }
                    Search(root, depth + 1, ref solution);
                    //Pop data object
                    chosenNode = solution.ElementAt(depth);
                    chosenColumn = chosenNode.header;
                    nextNode = chosenNode.left;
                    while (!nextNode.Equals(chosenNode))
                    {
                        nextNode.Column.Uncover();
                        nextNode = nextNode.left;
                    }
                    chosenNode = chosenNode.above;
                }
                chosenColumn.Uncover();
                return;
            }
        }

        private static ColumnHeader ChooseColumn(ColumnHeader root)
        {
            ColumnHeader minHeader = root.NextHeader;
            ColumnHeader currHeader = root.NextHeader;
            while(!currHeader.Equals(root))
            {
                currHeader = currHeader.NextHeader;
                if(currHeader.size < minHeader.size)
                {
                    minHeader = currHeader;
                }
            }
            return minHeader;
        }



        
    }
}