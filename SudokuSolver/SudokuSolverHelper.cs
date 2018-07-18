using System;
using System.Collections.Generic;

namespace SudokuSolver
{
    public static class SudokuSolverHelper
    {
        private const int SUDOKU_DIMENSION = 9;
        private const int NUM_COLUMN_HEADERS = 324;
        private const int NUM_HEADERS_PER_CATEGORY = 81;
        private const int BOX_DIMENSION = 3;
        private static ColumnHeader root;
        private static ColumnHeader[] ColumnHeaders;

        private class ColumnHeader
        {
            private static int indexNo = 0;
            public class Node : IEquatable<Node>
            {
                private static int nextRowID = 0;
                public Node left;
                public Node right;
                public Node below;
                public Node above;
                public ColumnHeader header;
                private readonly int rowID;

                public Node(ColumnHeader column)
                {
                    header = column;
                    rowID = nextRowID++;
                    if (column.front == null)
                    {
                        column.front = this;
                        column.back = this;
                        above = this;
                        below = this;
                    }
                    else
                    {
                        Node last = column.back;
                        last.below = this;
                        above = last;
                        below = column.front;
                        column.back = this;
                    }
                }

                public void HideLinkedNodesFromHeader()
                {
                    Node linkedNode = right;
                    while (!linkedNode.Equals(this))
                    {
                        linkedNode.Hide();
                        linkedNode.header.Remove();
                        linkedNode = linkedNode.right;
                    }
                }

                private void Hide()
                {
                    above.below = below;
                    below.above = above;
                }

                public void ShowLinkedNodesToHeader()
                {
                    Node linkedNode = left;
                    while (!linkedNode.Equals(this))
                    {
                        Unhide();
                        linkedNode.header.Add();
                        linkedNode = linkedNode.left;
                    }
                }

                private void Unhide()
                {
                    above.below = this;
                    below.above = this;
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
            private int index;
            public int Index
            {
                get
                {
                    return index;
                }
            }
            private Node back;
            private Node front;
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
                PreviousHeader = this;
                NextHeader = this;
            }

            public ColumnHeader(ColumnHeader root)
            {
                index = indexNo++;
                ColumnHeader last = root.PreviousHeader;
                last.NextHeader = this;
                root.PreviousHeader = this;
                PreviousHeader = last;
                NextHeader = root;
            }

            public void Hide()
            {
                NextHeader.PreviousHeader = PreviousHeader;
                PreviousHeader.NextHeader = NextHeader;
            }

            public void Unhide()
            {
                NextHeader.PreviousHeader = this;
                PreviousHeader.NextHeader = this;
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
                Node currNode = back;
                for (int i = 0; i < size; i++)
                {
                    currNode.HideLinkedNodesFromHeader();
                    currNode = currNode.below;
                }
            }

            public void Uncover()
            {
                Node currNode = front;
                for (int i = 0; i < size; i++)
                {
                    currNode.ShowLinkedNodesToHeader();
                    currNode = currNode.above;
                }
                NextHeader.PreviousHeader = this;
                PreviousHeader.NextHeader = this;
            }

            private void Add()
            {
                ++size;
            }

            private void Remove()
            {
                --size;
            }

            public Node CreateNode()
            {
                return new Node(this);
            }

            public static ColumnHeader ChooseColumnHeader(ColumnHeader root)
            {
                ColumnHeader minHeader = root.NextHeader;
                ColumnHeader currHeader = root.NextHeader;
                while (!currHeader.Equals(root))
                {
                    currHeader = currHeader.NextHeader;
                    if (currHeader.size < minHeader.size)
                    {
                        minHeader = currHeader;
                    }
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
                if (root.NextHeader == root)
                {
                    done = true;
                    return;
                }
                else
                {
                    ColumnHeader chosenColumn = ChooseColumnHeader(root);
                    Node chosenNode = chosenColumn.front;
                    //for each node in the chosen column
                    do
                    {
                        solution.Push(chosenNode);
                        Node nextNode = chosenNode.right;
                        while (!nextNode.Equals(chosenNode))
                        {
                            nextNode.header.Cover();
                            nextNode = nextNode.right;
                        }
                        Search(root, ref solution, ref done);
                        //Pop data object
                        if (done) return;
                        chosenNode = solution.Pop();
                        chosenColumn = chosenNode.header;
                        nextNode = chosenNode.left;
                        while (!nextNode.Equals(chosenNode))
                        {
                            nextNode.header.Uncover();
                            nextNode = nextNode.left;
                        }
                        chosenNode = chosenNode.above;
                    }
                    while (!chosenNode.Equals(chosenColumn.front));
                    chosenColumn.Uncover();
                    return;
                }
            }
        }

        private static List<ColumnHeader> clues = new List<ColumnHeader>();

        static SudokuSolverHelper()
        {
            root = new ColumnHeader();
            CreateSudokuConstraintMatrix();
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
                        ColumnHeader.Node cellNode = LookUpColumnHeader(HeaderArrayAccess.Cell, row, col).CreateNode();
                        ColumnHeader.Node rowNode = LookUpColumnHeader(HeaderArrayAccess.Row, row, num).CreateNode();
                        ColumnHeader.Node colNode = LookUpColumnHeader(HeaderArrayAccess.Column, col, num).CreateNode();
                        ColumnHeader.Node boxNode = LookUpColumnHeader(HeaderArrayAccess.Box, GetBoxIndex(row, col), num).CreateNode();
                        ColumnHeader.Node.LinkNodes(cellNode, rowNode, colNode, boxNode);
                    }
                }
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
            ReadInClues(puzzle);
            Stack<ColumnHeader.Node> solution = ColumnHeader.Search(root);
            InsertSolution(solution, puzzle);
            UnhideClues();
        }

        private static void UnhideClues()
        {
            foreach (var header in clues)
            {
                header.Unhide();
            }
        }

        private static void InsertSolution(Stack<ColumnHeader.Node> solution, SudokuPuzzle puzzle)
        {
            while (solution.Count > 0)
            {
                InterpretIndex(solution.Peek(), out int row, out int col, out int num);
                puzzle.PuzzleMatrix[row][col] = num;
                solution.Pop().header.Uncover();
            }
        }

        private static void InterpretIndex(ColumnHeader.Node solutionPart, out int row, out int col, out int num)
        {
            int index1 = solutionPart.header.Index;
            int index2 = solutionPart.right.header.Index;
            int throwaway = 0;
            row = 0;
            col = 0;
            num = 0;
            HeaderArrayAccess indx1Cat = CategoryFromIndex(index1);
            HeaderArrayAccess indx2Cat = CategoryFromIndex(index2);
            switch (indx1Cat)
            {
                case HeaderArrayAccess.Cell:
                    IndexerAndOffsetFromIndex(index1, out row, out col);
                    break;
                case HeaderArrayAccess.Row:
                    IndexerAndOffsetFromIndex(index1, out row, out num);
                    break;
                case HeaderArrayAccess.Column:
                    IndexerAndOffsetFromIndex(index1, out col, out num);
                    break;
                case HeaderArrayAccess.Box:
                    IndexerAndOffsetFromIndex(index1, out throwaway, out num);
                    break;
            }
            switch (indx2Cat)
            {
                case HeaderArrayAccess.Cell:
                    IndexerAndOffsetFromIndex(index1, out row, out col);
                    break;
                case HeaderArrayAccess.Row:
                    IndexerAndOffsetFromIndex(index1, out row, out num);
                    break;
                case HeaderArrayAccess.Column:
                    IndexerAndOffsetFromIndex(index1, out col, out num);
                    break;
                case HeaderArrayAccess.Box:
                    IndexerAndOffsetFromIndex(index1, out throwaway, out num);
                    break;
            }
        }

        private static void IndexerAndOffsetFromIndex(int index, out int indexer, out int offset)
        {
            index = index - ((index / NUM_HEADERS_PER_CATEGORY) * NUM_HEADERS_PER_CATEGORY);
            indexer = index / SUDOKU_DIMENSION;
            offset = index % SUDOKU_DIMENSION;
        }


        private static HeaderArrayAccess CategoryFromIndex(int index)
        {
            return (HeaderArrayAccess)(index / NUM_HEADERS_PER_CATEGORY);
        }

        private static void ReadInClues(SudokuPuzzle puzzle)
        {
            for (int i = 0; i < SUDOKU_DIMENSION; i++)
            {
                for (int j = 0; j < SUDOKU_DIMENSION; j++)
                {
                    if(puzzle.PuzzleMatrix[i][j] != 0)
                    {
                        ColumnHeader cell = GetHeader(HeaderArrayAccess.Cell, i, j);
                        ColumnHeader row = GetHeader(HeaderArrayAccess.Row, i, puzzle.PuzzleMatrix[i][j]);
                        ColumnHeader column = GetHeader(HeaderArrayAccess.Column, j, puzzle.PuzzleMatrix[i][j]);
                        ColumnHeader box = GetHeader(HeaderArrayAccess.Box, GetBoxIndex(i, j), puzzle.PuzzleMatrix[i][j]);
                        HideColumnHeaders(cell, row, column, box);
                    }
                }
            }
        }

        private static void HideColumnHeaders(params ColumnHeader[] headers)
        {
            for (int i = 0; i < headers.Length; i++)
            {
                headers[i].Hide();
                clues.Add(headers[i]);
            }
        }

        private static ColumnHeader GetHeader(HeaderArrayAccess category, int indexer, int offset)
        {
            return ColumnHeaders[(int)category * NUM_HEADERS_PER_CATEGORY + indexer * SUDOKU_DIMENSION + offset];
        }
    }
}
