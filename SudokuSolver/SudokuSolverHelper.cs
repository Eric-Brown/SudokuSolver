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
        static ColumnHeader root;

        private class Node
        {
            public Node Left;
            public Node Right;
            public Node Up;
            public Node Down;
            public ColumnHeader Column;
            public int rowID;
            int nodeCount;
        }
        private class ColumnHeader : Node
        {
            public new ColumnHeader Left;
            public new ColumnHeader Right;
            public int Size;
            public int ColumnID;
        }

        static SudokuSolverHelper()
        {
            //create sudoku sparse matrix
            root = new ColumnHeader();

        }
        
        private static void CreateSudokuConstraintMatrix()
        {
            //Cell Constraints 81 count
            //Row Constraints 81 count
            //Column Constraints 81 count
            //Box Constraints 81 count
            for (int i = 0; i < SUDOKU_DIMENSION; i++)
            {
                for (int j = 0; j < SUDOKU_DIMENSION; j++)
                {
                    //a 1 in cell 0,0 is also a 1 in that row, and a 1 in that col, and a 1 in that box
                }
            }
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
            if(root.Right == root)
            {
                //we have solution here
                return;
            }
            else
            {
                ColumnHeader column = ChooseColumn(root);
                Node row = column.Down;
                while(row != column)
                {
                    solution.Add(row);
                    Node j = row.Right;
                    while(j != row)
                    {
                        Cover(j.Column);
                        j = j.Right;
                    }
                    Search(root, depth + 1, ref solution);
                    //Pop data object
                    row = solution.ElementAt(depth);
                    column = row.Column;
                    j = row.Left;
                    while (j != row)
                    {
                        Uncover(j.Column);
                        j = j.Left;
                    }
                    row = row.Down;
                }
                Uncover(column);
                return;
            }
        }

        private static ColumnHeader ChooseColumn(ColumnHeader root)
        {
            ColumnHeader minHeader = root.Right;
            ColumnHeader currHeader = root.Right;
            while(currHeader != root)
            {
                currHeader = currHeader.Right;
                if(currHeader.Size < minHeader.Size)
                {
                    minHeader = currHeader;
                }
            }
            return minHeader;
        }

        private static void Cover(ColumnHeader toCover)
        {
            toCover.Right.Left = toCover.Left;
            toCover.Left.Right = toCover.Right;
            Node row = toCover.Down;
            while(row != toCover)
            {
                Node j = row.Right;
                while(j != row)
                {
                    j.Down.Up = j.Up;
                    j.Up.Down = j.Down;
                    --j.Column.Size;
                    j = j.Right;
                }
                row = row.Down;
            }
        }
        private static void Uncover(ColumnHeader toUncover)
        {
            Node row = toUncover.Up;
            while(row != toUncover)
            {
                Node j = row.Left;
                while (j != row)
                {
                    ++j.Column.Size;
                    j.Down.Up = j;
                    j.Up.Down = j;
                    j = j.Left;
                }
                row = row.Up;
            }
            toUncover.Right.Left = toUncover;
            toUncover.Left.Right = toUncover;
        }
    }
}
