using Microsoft.VisualStudio.TestTools.UnitTesting;
using SudokuSolver;
using System.IO;

namespace SudokuSolverTests
{
    [TestClass]
    public class SudokuPuzzleTester
    {
        private const string NONEXISTANTFILENAME = "Doesntexist.txt";
        private const string EMPTYFILENAME = "C:\\Users\\alexa\\source\\repos\\SudokuSolver\\SodokuSolverTestingProject\\Empty.txt";
        private const string WRONGFILEFORMATFILENAME = "C:\\Users\\alexa\\source\\repos\\SudokuSolver\\SodokuSolverTestingProject\\WrongFile.txt";
        private const string GOODFILENAME = "C:\\Users\\alexa\\source\\repos\\SudokuSolver\\SodokuSolverTestingProject\\validSudoku.txt";
        private const string GOODSINGLEPUZZLEFILE = "C:\\Users\\alexa\\source\\repos\\SudokuSolver\\SodokuSolverTestingProject\\singlePuzzle.txt";

        [TestMethod]
        public void CanBeCreatedFromStream()
        {
            StreamReader stream = GetStreamFromFileName(GOODSINGLEPUZZLEFILE);
            SudokuPuzzle created = SudokuPuzzle.CreateFromStream(stream);
            Assert.IsNotNull(created);
        }

        [TestMethod]
        public void CreatedPuzzlesAreCorrect()
        {
            SudokuPuzzle expected = new SudokuPuzzle("Grid 01")
            {
                PuzzleMatrix = new int[][]
            {
                new int[] { 0,0,3,0,2,0,6,0,0 },
                new int[] { 9,0,0,3,0,5,0,0,1 },
                new int[] { 0,0,1,8,0,6,4,0,0 },
                new int[] { 0,0,8,1,0,2,9,0,0 },
                new int[] { 7,0,0,0,0,0,0,0,8 },
                new int[] { 0,0,6,7,0,8,2,0,0 },
                new int[] { 0,0,2,6,0,9,5,0,0 },
                new int[] { 8,0,0,2,0,3,0,0,9 },
                new int[] { 0,0,5,0,1,0,3,0,0 },
            }
            };
            SudokuPuzzle actual = SudokuPuzzle.CreateFromStream(GetStreamFromFileName(GOODSINGLEPUZZLEFILE));

            Assert.IsTrue(actual.Equals(expected));
        }

        private static StreamReader GetStreamFromFileName(string filename)
        {
            return new StreamReader(new BufferedStream(File.OpenRead(filename)));
        }

        [TestMethod]
        public void IsSolvedIsCorrect()
        {
            SudokuPuzzle unsolved = SudokuPuzzle.CreateFromStream(GetStreamFromFileName(GOODSINGLEPUZZLEFILE));
            Assert.IsFalse(unsolved.IsSolved());
            SudokuPuzzle solved = new SudokuPuzzle("Solved1")
            {
                PuzzleMatrix = new int[][]
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
            }
            };
            Assert.IsTrue(solved.IsSolved());
        }
    }
}