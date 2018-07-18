using Microsoft.VisualStudio.TestTools.UnitTesting;
using SudokuSolver;
using System.IO;

namespace SudokuSolverTests
{
    [TestClass]
    public class SudokuSolverTester
    {
        private const string GOODFILENAME = "C:\\Users\\alexa\\source\\repos\\SudokuSolver\\SodokuSolverTestingProject\\validSudoku.txt";
        private const string GOODSINGLEPUZZLEFILE = "C:\\Users\\alexa\\source\\repos\\SudokuSolver\\SodokuSolverTestingProject\\singlePuzzle.txt";

        [TestMethod]
        public void SolutionIsCorrect()
        {
            SudokuPuzzle puzzle = SudokuPuzzle.CreateFromStream(GetStreamFromFileName(GOODSINGLEPUZZLEFILE));
            SudokuSolverHelper.Solve(puzzle);
            Assert.IsTrue(puzzle.IsSolved());
        }

        [TestMethod]
        public void AllSudokusHaveSolutions()
        {
        }

        private static StreamReader GetStreamFromFileName(string filename)
        {
            return new StreamReader(new BufferedStream(File.OpenRead(filename)));
        }
    }
}