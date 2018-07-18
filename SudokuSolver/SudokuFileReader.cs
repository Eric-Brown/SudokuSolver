using System;
using System.Collections.Generic;
using System.IO;

namespace SudokuSolver
{
    public static class SudokuFileReader
    {
        private const string BAD_FILE_ERRMSG = "Given filename is either empty or cannot be read as a sudoku puzzle.";

        public static List<SudokuPuzzle> ReadFile(String filename)
        {
            List<SudokuPuzzle> puzzles = new List<SudokuPuzzle>();
            using (StreamReader stream = new StreamReader(new BufferedStream(File.OpenRead(filename))))
            {
                if (stream.EndOfStream)
                {
                    throw new ArgumentException(BAD_FILE_ERRMSG);
                }
                while (!stream.EndOfStream)
                {
                    puzzles.Add(SudokuPuzzle.CreateFromStream(stream));
                }
            }
            return puzzles;
        }
    }
}