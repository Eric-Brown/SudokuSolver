using System;
using System.Collections.Generic;
using SudokuSolver;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SudokuSolverTests
{
    [TestClass]
    public class HeaderRootTester
    {
        private const int NUM_HEADERS_FOR_KNOWN_MATRIX = 7;
        private const int SolutionNodeCount = 3;
        private const int EXPECTED_LENGTH = 10;

        [TestMethod]
        public void SearchWillReturnACorrectNodeSolutionStack()
        {
            HeaderRoot root = GetRoot();
            ColumnHeader[] headers = GetHeadersWithLinkedNodes(root);
            Stack<Node> solution = root.Search();
            Assert.IsTrue(solution.Count == 1);
        }

        [TestMethod]
        public void SearchWillReturnACorrectNodeSolutionStackForComplicatedMatrix()
        {
            HeaderRoot root = GetRoot();
            ColumnHeader[] headers = GetKnownProblemHeaders(root);

            Stack<Node> solutionActual = root.Search();
            List<ColumnHeader> retrievedHeaders = new List<ColumnHeader>();
            for (int i = 0; i < solutionActual.Count; i++)
            {
                retrievedHeaders.AddRange(solutionActual.ToArray()[i].GetLinkedHeaders());
            }
            Assert.IsTrue(retrievedHeaders.Count == root.Count);
            Assert.IsTrue(solutionActual.Count == 3);


        }
        [TestMethod]
        public void CoverWillCorrectlySetOtherKnownHeadersHights()
        {
            HeaderRoot root = GetRoot();
            ColumnHeader[] known = GetKnownProblemHeaders(root);
            int[] expectedRemainingSize = GetKnownProblemHeaderCoverResults();
            for (int i = 0; i < known.Length; i++)
            {
                known[i].Cover();
                int sum = 0;
                ColumnHeader temp = root.right;
                while (temp != root)
                {
                    sum += temp.Size;
                    temp = temp.right;
                }
                Assert.IsTrue(sum == expectedRemainingSize[i]);
                known[i].Uncover();
            }
        }

        private int[] GetKnownProblemHeaderCoverResults()
        {
            return new int[NUM_HEADERS_FOR_KNOWN_MATRIX]
            {
                12,
                11,
                10,
                9,
                11,
                10,
                5
            };
        }
        private Node[] GetKnownProblemSolutionNodes(HeaderRoot root)
        {
            Node[] solutionSet = new Node[SolutionNodeCount];
            solutionSet[0] = root[0].down.down;
            solutionSet[1] = root[4].down.down;
            solutionSet[2] = root[1].down.down;
            return solutionSet;
        }
        private ColumnHeader[] GetHeaders(HeaderRoot root, int numHeaders)
        {
            ColumnHeader[] newHeaders = new ColumnHeader[numHeaders];
            for (int i = 0; i < numHeaders; i++)
            {
                newHeaders[i] = root.CreateHeader();
            }
            return newHeaders;
        }
        /// <summary>
        /// Creates and links nodes that exist at the given header indexes.
        /// </summary>
        /// <param name="root"></param>
        /// <param name="headerIndexes">These are expected to be 1 indexed.</param>
        private void CreateRowForHeaders(HeaderRoot root, params int[] headerIndexes)
        {
            Node[] toLink = new Node[headerIndexes.Length];
            for (int i = 0; i < headerIndexes.Length; i++)
            {
                toLink[i] = root[headerIndexes[i] -1].CreateNode();
            }
            Node.LinkNodes(toLink);
        }
        private ColumnHeader[] GetKnownProblemHeaders(HeaderRoot root)
        {
            ColumnHeader[] headers = GetHeaders(root, NUM_HEADERS_FOR_KNOWN_MATRIX);
            CreateRowForHeaders(root, 1, 4, 7);
            CreateRowForHeaders(root, 1, 4);
            CreateRowForHeaders(root, 4, 5, 7);
            CreateRowForHeaders(root, 3, 5, 6);
            CreateRowForHeaders(root, 2, 3, 6, 7);
            CreateRowForHeaders(root, 2, 7);
            return headers;
        }
        private static HeaderRoot GetRoot()
        {
            return new MockHeaderRoot();
        }
        private ColumnHeader[] GetHeadersWithLinkedNodes(HeaderRoot root)
        {
            ColumnHeader[] headers = GetHeaders(root, EXPECTED_LENGTH);
            Node[] nodes = new Node[EXPECTED_LENGTH];
            for (int i = 0; i < EXPECTED_LENGTH; i++)
            {
                nodes[i] = headers[i].CreateNode();
            }
            Node.LinkNodes(nodes);
            return headers;
        }
    }
}
