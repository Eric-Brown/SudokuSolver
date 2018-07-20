using System;
using System.IO;
using SudokuSolver;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SodokuSolverTests
{
    [TestClass]
    public class SudokuRootTester
    {
        private const string GOODFILENAME = "C:\\Users\\alexa\\source\\repos\\SudokuSolver\\SodokuSolverTestingProject\\validSudoku.txt";
        private const string GOODSINGLEPUZZLEFILE = "C:\\Users\\alexa\\source\\repos\\SudokuSolver\\SodokuSolverTestingProject\\singlePuzzle.txt";


        [TestMethod]
        public void TestMethod1()
        {
            SudokuPuzzle unsolved = SudokuPuzzle.CreateFromStream(GetStreamFromFileName(GOODSINGLEPUZZLEFILE));
            Assert.IsFalse(unsolved.IsSolved());
            SudokuPuzzle solved = new SudokuPuzzle("Solved1");
            solved.PuzzleMatrix = new int[][]
            {
                new int[] { 4,8,3,9,2,1,6,5,7 },
                new int[] { 9,6,7,3,4,5,8,2,1 },
                new int[] { 2,5,1,8,7,6,4,9,3 },
                new int[] { 5,4,8,1,3,2,9,7,6 },
                new int[] { 7,2,9,5,6,4,1,3,8 },
                new int[] { 1,3,6,7,9,8,2,4,5 },
                new int[] { 3,7,2,6,8,9,5,1,4 },
                new int[] { 8,1,4,2,5,3,7,6,9 },
                new int[] { 6,9,5,4,1,7,3,8,2 },
            };
            SudokuRoot totest =  new SudokuRoot();
            totest.SolveSudoku(unsolved);
            Assert.IsTrue(unsolved.IsSolved());
        }
        private static StreamReader GetStreamFromFileName(string filename)
        {
            return new StreamReader(new BufferedStream(File.OpenRead(filename)));
        }
    }
}
