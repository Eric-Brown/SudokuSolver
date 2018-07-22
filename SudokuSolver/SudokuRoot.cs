using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    public class SudokuRoot : ColumnHeader
    {
        private const int SUDOKU_DIMENSION = 9;
        private const int NUM_COLUMN_HEADERS = 324;
        private const int NUM_HEADERS_PER_CATEGORY = 81;
        private const int BOX_DIMENSION = 3;
        private ColumnHeader root;
        private ColumnHeader[] ColumnHeaders;
        private List<Node> clueHeaders = new List<Node>();
        //Cell Constraints 81 count
        //      Each cell (81 cells) must have only 1 value
        //Row Constraints 81 count
        //      Each row (9 rows) must have each number (9)
        //Column Constraints 81 count
        //      Each column (9 columns) must have each number (9)
        //Box Constraints 81 count
        //      Each box (9 boxes) must have each number (9)
        private enum HeaderArrayAccess
        {
            Cell = 0,
            Row = 1,
            Column = 2,
            Box = 3
        }

        public SudokuRoot()
        {
            InitializeRootAndColumnHeaders();
            CreateSudokuConstraintMatrix();
        }

        private void InitializeRootAndColumnHeaders()
        {
            root = new ColumnHeader();
            CreateColumnHeaders();

        }
        private void CreateColumnHeaders()
        {

            ColumnHeaders = new ColumnHeader[NUM_COLUMN_HEADERS];
            for (int i = 0; i < NUM_COLUMN_HEADERS; i++)
            {
                ColumnHeaders[i] = new ColumnHeader(root);
            }
        }
        private void CreateSudokuConstraintMatrix()
        {
            //729 Rows needed
            for (int row = 0; row < SUDOKU_DIMENSION; row++)
            {
                for (int col = 0; col < SUDOKU_DIMENSION; col++)
                {
                    //For each new cell in G, we create 4 data objects and link them together
                    //then these data objects are linked to their respective column header and inserted
                    for (int num = 0; num < SUDOKU_DIMENSION; num++)
                    {
                        CreateSudokuRow(row, col, num);
                    }
                }
            }
        }

        private void CreateSudokuRow(int row, int col, int num)
        {
            Node cellNode = LookUpColumnHeader(HeaderArrayAccess.Cell, row, col).CreateNode();
            Node rowNode = LookUpColumnHeader(HeaderArrayAccess.Row, row, num).CreateNode();
            Node colNode = LookUpColumnHeader(HeaderArrayAccess.Column, col, num).CreateNode();
            Node boxNode = LookUpColumnHeader(HeaderArrayAccess.Box, GetBoxIndex(row, col), num).CreateNode();
            Node.LinkNodes(cellNode, rowNode, colNode, boxNode);
        }

        private int GetBoxIndex(int row, int column)
        {
            return (row / BOX_DIMENSION) * BOX_DIMENSION + (column / BOX_DIMENSION);
        }

        private ColumnHeader LookUpColumnHeader(HeaderArrayAccess category, int indexer, int offset)
        {
            return ColumnHeaders[(NUM_HEADERS_PER_CATEGORY * (int)category) +
                (indexer * SUDOKU_DIMENSION) +
                offset];
        }

        public void SolveSudoku(SudokuPuzzle puzzle)
        {
            ReadInClues(puzzle);
            Stack<Node> solution = Search(root);
            EnterSolution(solution, puzzle);
            RemoveClues();
        }

        private void RemoveClues()
        {
            clueHeaders.Reverse();
            foreach(var clue in clueHeaders)
            {
                ColumnHeader header = clue.header;
                clue.Unselect();
                header.Uncover();
            }
            clueHeaders.Clear();
        }

        private void EnterSolution(Stack<Node> solution, SudokuPuzzle puzzle)
        {
            while (solution.Count > 0)
            {
                List<ColumnHeader> rowHeaders = solution.Pop().GetLinkedHeaders();
                InterpretRow(rowHeaders, puzzle.PuzzleMatrix);
            }
        }

        private void InterpretRow(List<ColumnHeader> rowHeaders, int[][] puzzleMatrix)
        {
            rowHeaders.Sort((a, b) => a.Index.CompareTo(b.Index));
            int row = (rowHeaders[0].Index - 2) / SUDOKU_DIMENSION;
            int col = (rowHeaders[0].Index - 2) % SUDOKU_DIMENSION;
            int num = (rowHeaders[1].Index - 2) - NUM_HEADERS_PER_CATEGORY - (row * SUDOKU_DIMENSION) + 1;
            puzzleMatrix[row][col] = num;
        }

        private void ReadInClues(SudokuPuzzle puzzle)
        {
            for (int i = 0; i < SUDOKU_DIMENSION; i++)
            {
                for (int j = 0; j < SUDOKU_DIMENSION; j++)
                {
                    if (puzzle.PuzzleMatrix[i][j] != 0)
                    {
                        ColumnHeader cell = LookUpColumnHeader(HeaderArrayAccess.Cell, i, j);
                        ColumnHeader row = LookUpColumnHeader(HeaderArrayAccess.Row, i, puzzle.PuzzleMatrix[i][j] - 1);
                        AddClues(cell, row);
                    }
                }
            }
        }
        private void AddClues(ColumnHeader a, ColumnHeader b)
        {
            a.Cover();
            Node currNode = a.down;
            while (!currNode.GetLinkedHeaders().Contains(b))
            {
                currNode = currNode.down;
            }
            currNode.Select();
            clueHeaders.Add(currNode);
        }
    }
}
