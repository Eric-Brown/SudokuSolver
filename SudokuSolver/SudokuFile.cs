using System;
using System.Collections.Generic;
using System.IO;

namespace SudokuSolver
{
    public static class SudokuFile
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

        public static void SaveFile(List<SudokuPuzzle> puzzles, string fileName)
        {
            using (StreamWriter stream = new StreamWriter(new BufferedStream(File.OpenWrite(fileName))))
            {
                OutputToStream(puzzles, stream);
            }
        }
        public static void OutputToStream(List<SudokuPuzzle> puzzles, TextWriter stream)
        {

                foreach (var puzzle in puzzles)
                {
                    puzzle.WriteToStream(stream);
                }
        }

    }
}