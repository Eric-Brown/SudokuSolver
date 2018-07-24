using System.Collections.Generic;
using System.IO;
namespace SudokuSolver
{
    public static class Program
    {
        private const string DEFAULT_OUTPUT_FILENAME = "output.txt";

        private static void Main(string[] args)
        {
            ReadInput(args);
        }
        public static void ReadInput(params string[] args)
        {
            if (!File.Exists(args[0]))
            {
                System.Console.WriteLine($"Provided file name: {args[0]}, does not exist.");
                return;
            }
            try
            {
                List<SudokuPuzzle> puzzles = SudokuFile.ReadFile(args[0]);
                SudokuRoot solver = new SudokuRoot();
                foreach (var puzzle in puzzles)
                {
                    solver.SolveSudoku(puzzle);
                }
                if (args.Length > 1)
                {
                    string outputFileName = DEFAULT_OUTPUT_FILENAME;
                    if (args[1] == "-f")
                    {
                        if (args.Length > 2)
                        {
                            outputFileName = args[2];
                        }
                    }
                    else
                    {
                        outputFileName = args[1];
                    }
                    SudokuFile.SaveFile(puzzles, outputFileName);
                }
                else
                {
                    SudokuFile.OutputToStream(puzzles, System.Console.Out);
                    long sum = 0;
                    foreach (var puzzle in puzzles)
                    {
                        sum += puzzle.PuzzleMatrix[0][0] * 100 + puzzle.PuzzleMatrix[0][1] * 10 + puzzle.PuzzleMatrix[0][2];
                    }
                    System.Console.WriteLine(sum);
                }
            }
            catch (System.ArgumentException ex)
            {
                System.Console.WriteLine(ex.Message);
            }
        }
    }
}