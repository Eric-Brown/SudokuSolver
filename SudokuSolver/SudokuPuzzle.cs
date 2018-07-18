using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace SudokuSolver
{
    public class SudokuPuzzle : IEquatable<SudokuPuzzle>
    {
        private const int SUDOKU_DIMENSIONS = 9;
        private const string BAD_FILE_ERRMSG = "Line not properly formatted in given file.";
        private const int BOX_DIMENSIONS = 3;
        public readonly string PuzzleName;
        public int[][] PuzzleMatrix = new int[SUDOKU_DIMENSIONS][];

        public SudokuPuzzle(string name)
        {
            PuzzleName = name;
            for (int i = 0; i < SUDOKU_DIMENSIONS; i++)
            {
                PuzzleMatrix[i] = new int[SUDOKU_DIMENSIONS];
            }
        }

        public SudokuPuzzle(string name, int[][] matrix)
        {
            PuzzleName = name;
            PuzzleMatrix = matrix;
        }

        public static SudokuPuzzle CreateFromStream(TextReader stream)
        {
            string name = stream.ReadLine();
            string currentLine;
            int[][] matrix = new int[SUDOKU_DIMENSIONS][];
            for (int i = 0; i < SUDOKU_DIMENSIONS; i++)
            {
                currentLine = stream.ReadLine();
                if (string.IsNullOrEmpty(currentLine) || currentLine.Length < SUDOKU_DIMENSIONS || Regex.IsMatch(currentLine, ".*\\D.*"))
                {
                    throw new ArgumentException(BAD_FILE_ERRMSG);
                }
                matrix[i] = new int[SUDOKU_DIMENSIONS];
                for (int j = 0; j < SUDOKU_DIMENSIONS; j++)
                {
                    matrix[i][j] = (int)char.GetNumericValue(currentLine[j]);
                }
            }

            return new SudokuPuzzle(name, matrix);
        }

        public bool Equals(SudokuPuzzle other)
        {
            bool matrixesEqual = true;
            for (int i = 0; i < SUDOKU_DIMENSIONS; i++)
            {
                for (int j = 0; j < SUDOKU_DIMENSIONS; j++)
                {
                    if (!matrixesEqual) break;
                    if(PuzzleMatrix[i][j] != other.PuzzleMatrix[i][j])
                    {
                        matrixesEqual = false;
                    }
                }
            }
            return string.Equals(PuzzleName, other.PuzzleName) && matrixesEqual;
        }

        public bool IsSolved()
        {
            bool result = true;
            for (int i = 0; i < SUDOKU_DIMENSIONS; i++)
            {
                if (result == false) break;
                result = StripIsValid(PuzzleMatrix[i]) && ColumnIsValid(i) && BoxIsValid(i);
            }
            return result;
        }

        private bool BoxIsValid(int boxNum)
        {
            //Boxes are 3x3, start at 0, and increment as they go left to right and top to bottom
            int[] strippedBox = new int[SUDOKU_DIMENSIONS];
            for (int i = 0; i < BOX_DIMENSIONS; i++)
            {
                for (int j = 0; j < BOX_DIMENSIONS; j++)
                {
                    strippedBox[i * BOX_DIMENSIONS + j] = PuzzleMatrix[boxNum / BOX_DIMENSIONS + i][boxNum % BOX_DIMENSIONS + j];
                }
            }
            return StripIsValid(strippedBox);
        }

        private bool ColumnIsValid(int col)
        {
            int[] columnStrip = new int[SUDOKU_DIMENSIONS];
            for (int i = 0; i < SUDOKU_DIMENSIONS; i++)
            {
                columnStrip[i] = PuzzleMatrix[i][col];
            }
            return StripIsValid(columnStrip);
        }

        private bool StripIsValid(int[] strip)
        {
            int sum = 0;
            HashSet<int> found = new HashSet<int>();
            for (int i = 0; i < SUDOKU_DIMENSIONS; i++)
            {
                found.Add(strip[i]);
                sum += strip[i];
            }
            return sum == 45 && found.Count == 9;
        }
    }
}
