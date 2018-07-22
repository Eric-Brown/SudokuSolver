using System;
using System.Collections.Generic;
using SudokuSolver;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SudokuSolverTests
{
    [TestClass]
    public class ProgramTester
    {
        private const string GOODFILENAME = "C:\\Users\\alexa\\source\\repos\\SudokuSolver\\SodokuSolverTestingProject\\validSudoku.txt";

        [TestMethod]
        public void SolvesAllTheThings()
        {

            List<SudokuPuzzle> puzzles = SudokuFileReader.ReadFile(GOODFILENAME);
            SudokuRoot Solver = new SudokuRoot();
            foreach (var puzzle in puzzles)
            {
                Solver.SolveSudoku(puzzle);
            }
            foreach (var puz in puzzles)
            {
                Assert.IsTrue(puz.IsSolved());
            }
        }
    }
}
