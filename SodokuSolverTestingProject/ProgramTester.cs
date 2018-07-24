using System;
using System.Collections.Generic;
using SudokuSolver;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SudokuSolverTests
{
    [TestClass]
    public class ProgramTester
    {
        private const string GOODFILENAME = "C:\\Users\\alexa\\source\\repos\\SudokuSolver\\SodokuSolverTestingProject\\validSudoku.txt";
        private const string TESTFILENAMEABS = @"C:\Users\alexa\source\repos\SudokuSolver\SodokuSolverTestingProject\test.txt";
        private const string TESTFILENAME = "output.txt";
        private const string NONEXISTANTFILENAME = "Doesntexist.txt";
        private const string EMPTYFILENAME = "C:\\Users\\alexa\\source\\repos\\SudokuSolver\\SodokuSolverTestingProject\\Empty.txt";
        private const string WRONGFILEFORMATFILENAME = "C:\\Users\\alexa\\source\\repos\\SudokuSolver\\SodokuSolverTestingProject\\WrongFile.txt";
        private const string OUTPUTABSPATH = @"C:\Users\alexa\source\repos\SudokuSolver\SodokuSolverTestingProject\output.txt";


        [TestMethod]
        public void SolvesAllTheThings()
        {

            List<SudokuPuzzle> puzzles = SudokuFile.ReadFile(GOODFILENAME);
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
        [TestMethod]
        public void SavesAllTheThings()
        {
            List<SudokuPuzzle> puzzles = SudokuFile.ReadFile(GOODFILENAME);
            SudokuFile.SaveFile(puzzles, TESTFILENAMEABS);
            List<SudokuPuzzle> puzzlesRead = SudokuFile.ReadFile(TESTFILENAMEABS);
            Assert.IsTrue(puzzles.Count == puzzlesRead.Count);
            for (int i = 0; i < puzzles.Count; i++)
            {
                Assert.IsTrue(puzzles[i].Equals(puzzlesRead[i]));
            }
        }
        
        [TestMethod]
        public void ProgramDoesNotThrowOnBadFileName()
        {
            Program.ReadInput(NONEXISTANTFILENAME);
            Program.ReadInput(EMPTYFILENAME);
            Program.ReadInput(WRONGFILEFORMATFILENAME);
        }

        [TestMethod]
        public void ProgramReadsAndSolvesPuzzlesInArgumentName()
        {
            StringWriter writer = new StringWriter();
            System.Console.SetOut(writer);
            Program.ReadInput(GOODFILENAME);
            Assert.IsFalse(string.IsNullOrEmpty(writer.ToString()));
        }
        [TestMethod]
        public void ProgramOutputsSolutionsToProvidedFileName()
        {
            Program.ReadInput(GOODFILENAME, OUTPUTABSPATH);
            Assert.IsTrue(File.Exists(OUTPUTABSPATH));
            List<SudokuPuzzle> solvedPuzzles = SudokuFile.ReadFile(OUTPUTABSPATH);
            foreach (var puzzle in solvedPuzzles)
            {
                Assert.IsTrue(puzzle.IsSolved());
            }
        }
        [TestMethod]
        public void ProgramOutputsSolutionsToDefaultFileNameIfAsked()
        {
            Program.ReadInput(GOODFILENAME, "-f");
            Assert.IsTrue(File.Exists(TESTFILENAME));
            List<SudokuPuzzle> solvedPuzzles = SudokuFile.ReadFile(TESTFILENAME);
            foreach (var puzzle in solvedPuzzles)
            {
                Assert.IsTrue(puzzle.IsSolved());
            }

        }
    }
}
