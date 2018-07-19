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
                        Node cellNode = LookUpColumnHeader(HeaderArrayAccess.Cell, row, col).CreateNode();
                        Node rowNode = LookUpColumnHeader(HeaderArrayAccess.Row, row, num).CreateNode();
                        Node colNode = LookUpColumnHeader(HeaderArrayAccess.Column, col, num).CreateNode();
                        Node boxNode = LookUpColumnHeader(HeaderArrayAccess.Box, GetBoxIndex(row, col), num).CreateNode();
                        Node.LinkNodes(cellNode, rowNode, colNode, boxNode);
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
            Stack<Node> solution = ColumnHeader.Search(root);
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

        private static void InsertSolution(Stack<Node> solution, SudokuPuzzle puzzle)
        {
            while (solution.Count > 0)
            {
                InterpretIndex(solution.Peek(), out int row, out int col, out int num);
                puzzle.PuzzleMatrix[row][col] = num;
                solution.Pop().header.Uncover();
            }
        }

        private static void InterpretIndex(Node solutionPart, out int row, out int col, out int num)
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
                        ColumnHeader cell = LookUpColumnHeader(HeaderArrayAccess.Cell, i, j);
                        ColumnHeader row = LookUpColumnHeader(HeaderArrayAccess.Row, i, puzzle.PuzzleMatrix[i][j] - 1);
                        ColumnHeader column = LookUpColumnHeader(HeaderArrayAccess.Column, j, puzzle.PuzzleMatrix[i][j] - 1);
                        ColumnHeader box = LookUpColumnHeader(HeaderArrayAccess.Box, GetBoxIndex(i, j), puzzle.PuzzleMatrix[i][j] - 1);
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

    }
}
