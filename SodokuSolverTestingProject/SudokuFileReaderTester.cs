using Microsoft.VisualStudio.TestTools.UnitTesting;
using SudokuSolver;
using System;
using System.Collections.Generic;
using System.IO;

namespace SudokuSolverTests
{
    [TestClass]
    public class SudokuFileReaderTester
    {
        private const string NONEXISTANTFILENAME = "Doesntexist.txt";
        private const string EMPTYFILENAME = "C:\\Users\\alexa\\source\\repos\\SudokuSolver\\SodokuSolverTestingProject\\Empty.txt";
        private const string WRONGFILEFORMATFILENAME = "C:\\Users\\alexa\\source\\repos\\SudokuSolver\\SodokuSolverTestingProject\\WrongFile.txt";
        private const string GOODFILENAME = "C:\\Users\\alexa\\source\\repos\\SudokuSolver\\SodokuSolverTestingProject\\validSudoku.txt";
        private const string GOODSINGLEPUZZLEFILE = "C:\\Users\\alexa\\source\\repos\\SudokuSolver\\SodokuSolverTestingProject\\singlePuzzle.txt";

        [TestMethod]
        public void ReadingBadFilesThrowsExceptions()
        {
            Assert.ThrowsException<FileNotFoundException>(() => SudokuFile.ReadFile(NONEXISTANTFILENAME),
                "Exception expected for bad filename.");
            Assert.ThrowsException<ArgumentException>(() => SudokuFile.ReadFile(EMPTYFILENAME),
                "Exception expected for file that contains no information.");
            Assert.ThrowsException<ArgumentException>(() => SudokuFile.ReadFile(WRONGFILEFORMATFILENAME),
                "Exception expected for file that contains bad information.");
        }

        [TestMethod]
        public void ReadingGoodFilesDoesNotThrowExceptions()
        {
            try
            {
                SudokuFile.ReadFile(GOODFILENAME);
                SudokuFile.ReadFile(GOODSINGLEPUZZLEFILE);
            }
            catch (Exception)
            {
                Assert.Fail("Good files should not throw exceptions.");
            }
        }

        [TestMethod]
        public void ReadingGoodFilesProducesASudokuPuzzle()
        {
            AssertFileContainsPuzzles(GOODSINGLEPUZZLEFILE, 1);
            AssertFileContainsPuzzles(GOODFILENAME, 50);
        }

        private static void AssertFileContainsPuzzles(string file, int expectedNumber)
        {
            List<SudokuPuzzle> morePuzzles = SudokuFile.ReadFile(file);
            Assert.AreEqual(morePuzzles.Count, expectedNumber);
        }
    }
}