using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    public class SudokuRoot : HeaderRoot
    {
        private const int SUDOKU_DIMENSION = 9;
        private const int NUM_COLUMN_HEADERS = 324;
        private const int NUM_HEADERS_PER_CATEGORY = 81;
        private const int BOX_DIMENSION = 3;
        private ColumnHeader[] headerCache;
        private List<Node> clueHeaders = new List<Node>();

        private int count;
        public override int Count
        {
            get
            {
                return count;
            }
        }

        public override ColumnHeader this[int index]
        {
            get
            {
                if (count == NUM_COLUMN_HEADERS) return headerCache[index];
                else
                    return base[index];
            }
        }
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
            CreateHeaders();
            CreateSudokuConstraintMatrix();
        }

        private void CreateHeaders()
        {
            headerCache = new ColumnHeader[NUM_COLUMN_HEADERS];
            for (int i = 0; i < NUM_COLUMN_HEADERS; i++)
            {
                headerCache[i] = new ColumnHeader(this, i);
            }
            count = NUM_COLUMN_HEADERS;

        }
        private void CreateSudokuConstraintMatrix()
        {
            for (int row = 0; row < SUDOKU_DIMENSION; row++)
            {
                for (int col = 0; col < SUDOKU_DIMENSION; col++)
                {
                    for (int num = 0; num < SUDOKU_DIMENSION; num++)
                    {
                        CreateSudokuRow(row, col, num);
                    }
                }
            }
        }

        private void CreateSudokuRow(int row, int col, int num)
        {
            Node cellNode = RetrieveHeader(HeaderArrayAccess.Cell, row, col).CreateNode();
            Node rowNode = RetrieveHeader(HeaderArrayAccess.Row, row, num).CreateNode();
            Node colNode = RetrieveHeader(HeaderArrayAccess.Column, col, num).CreateNode();
            Node boxNode = RetrieveHeader(HeaderArrayAccess.Box, GetBoxIndex(row, col), num).CreateNode();
            Node.LinkNodes(cellNode, rowNode, colNode, boxNode);
        }

        private int GetBoxIndex(int row, int column)
        {
            return (row / BOX_DIMENSION) * BOX_DIMENSION + (column / BOX_DIMENSION);
        }

        private ColumnHeader RetrieveHeader(HeaderArrayAccess category, int indexer, int offset)
        {
            return headerCache[(NUM_HEADERS_PER_CATEGORY * (int)category) +
                (indexer * SUDOKU_DIMENSION) +
                offset];
        }

        public void SolveSudoku(SudokuPuzzle puzzle)
        {
            ReadInClues(puzzle);
            Stack<Node> solution = Search();
            EnterSolution(solution, puzzle);
            RemoveClues();
        }

        private void RemoveClues()
        {
            clueHeaders.Reverse();
            foreach (var clue in clueHeaders)
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
            rowHeaders.Sort((a, b) => a.Key.CompareTo(b.Key));
            int row = (rowHeaders[0].Key) / SUDOKU_DIMENSION;
            int col = (rowHeaders[0].Key) % SUDOKU_DIMENSION;
            int num = (rowHeaders[1].Key) - NUM_HEADERS_PER_CATEGORY - (row * SUDOKU_DIMENSION) + 1;
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
                        ColumnHeader cell = RetrieveHeader(HeaderArrayAccess.Cell, i, j);
                        ColumnHeader row = RetrieveHeader(HeaderArrayAccess.Row, i, puzzle.PuzzleMatrix[i][j] - 1);
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

        public override ColumnHeader CreateHeader()
        {
            return new ColumnHeader(this, count++);
        }
    }
}
