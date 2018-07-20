using System;
using SudokuSolver;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SudokuSolverTests
{
    [TestClass]
    public class HeaderTesting
    {
        private const int EXPECTED_LENGTH = 10;
        private const int NUM_HEADERS_FOR_KNOWN_MATRIX = 7;
        private const int SolutionNodeCount = 3;

        [TestMethod]
        public void DefaultConstructionLoopsToSelf()
        {
            ColumnHeader root = new ColumnHeader();
            Assert.IsTrue(root.left.Equals(root.right));
            Assert.IsTrue(root.down.Equals(root.up));
            Assert.IsTrue(root.header.Equals(root));
        }
        [TestMethod]
        public void HeaderAsConstructionParameterLinksHeadersTogether()
        {
            ColumnHeader root = new ColumnHeader();
            ColumnHeader next = new ColumnHeader(root);
            Assert.IsTrue(root.Equals(next.left));
            Assert.IsTrue(next.right.Equals(root),
                "Next -> Right ID: " + next.right.rowID + "\nRoot ID: " + root.rowID);
            Assert.IsFalse(root.Equals(next.up));
            Assert.IsFalse(root.Equals(next.down));
        }

        [TestMethod]
        public void LinkedHeadersCountedCorrectly()
        {
            ColumnHeader root = new ColumnHeader();
            ColumnHeader[] headers = GetHeaders(root, EXPECTED_LENGTH);
            Assert.IsTrue(root.NumLinkedHeaders == EXPECTED_LENGTH);
        }

        private ColumnHeader[] GetHeaders(ColumnHeader root, int numHeaders)
        {
            ColumnHeader[] newHeaders = new ColumnHeader[numHeaders];
            for (int i = 0; i < numHeaders; i++)
            {
                newHeaders[i] = new ColumnHeader(root);
            }
            return newHeaders;
        }

        [TestMethod]
        public void HidingRemovesHeaderFromLinkedList()
        {
            ColumnHeader root = new ColumnHeader();
            ColumnHeader next = new ColumnHeader(root);
            next.Hide();
            Assert.IsTrue(root.NumLinkedHeaders == 0);
        }

        [TestMethod]
        public void UnhidingAddsHeaderToLinkedListInOriginalPosition()
        {
            ColumnHeader root = new ColumnHeader();
            ColumnHeader[] headers = GetHeaders(root, EXPECTED_LENGTH);
            ColumnHeader toTest = GetHeaderAt(root, EXPECTED_LENGTH);
            toTest.Hide();
            toTest.Unhide();
            Assert.IsTrue(GetHeaderAt(root, EXPECTED_LENGTH).Equals(toTest));
        }

        [TestMethod]
        public void UnhidingManyAddsHeaderToLinkedListInOriginalPosition()
        {
            ColumnHeader root = new ColumnHeader();
            ColumnHeader[] headers = GetHeaders(root, EXPECTED_LENGTH);
            Random random = new Random();
            int numToHide = random.Next(0, EXPECTED_LENGTH);
            int[] indexes = new int[numToHide];
            List<ColumnHeader> hiddenHeaders = new List<ColumnHeader>();
            List<int> numbers = new List<int>();
            for (int i = 0; i < EXPECTED_LENGTH; i++)
            {
                numbers.Add(i);
            }
            for (int i = 0; i < numToHide; i++)
            {
                int numbersIndex = random.Next(0, numbers.Count);
                indexes[i] = numbers[numbersIndex];
                hiddenHeaders.Add(GetHeaderAt(root, numbers[numbersIndex]));
                numbers.RemoveAt(numbersIndex);
            }
            foreach (ColumnHeader toHide in hiddenHeaders)
            {
                toHide.Hide();
            }
            foreach (ColumnHeader toHide in hiddenHeaders)
            {
                toHide.Unhide();
            }
            for (int i = 0; i < numToHide; i++)
            {
                Assert.IsTrue(hiddenHeaders[i].Equals(GetHeaderAt(root, indexes[i])));
            }
        }

        [TestMethod]
        public void CoverWorksWithNoNodes()
        {
            ColumnHeader root = new ColumnHeader();
            ColumnHeader[] headers = GetHeaders(root, EXPECTED_LENGTH);
            headers[0].Cover();
            Assert.IsTrue(root.NumLinkedHeaders == EXPECTED_LENGTH - 1);
        }

        [TestMethod]
        public void UncoverWorksWithNoNodes()
        {
            ColumnHeader root = new ColumnHeader();
            ColumnHeader[] headers = GetHeaders(root, EXPECTED_LENGTH);
            headers[0].Cover();
            headers[0].Uncover();
            Assert.IsTrue(root.NumLinkedHeaders == EXPECTED_LENGTH);
        }

        [TestMethod]
        public void CoveringAllHeadersWillLeaveRootLinkingToSelf()
        {
            ColumnHeader root = new ColumnHeader();
            ColumnHeader[] headers = GetHeaders(root, EXPECTED_LENGTH);
            for (int i = 0; i < headers.Length; i++)
            {
                headers[i].Cover();
            }
            Assert.IsTrue(root.right.Equals(root));
            Assert.IsTrue(root.left.Equals(root));
        }

        [TestMethod]
        public void NodesCanBeAddedToHeaders()
        {
            ColumnHeader root = new ColumnHeader();
            ColumnHeader[] headers = GetHeaders(root, EXPECTED_LENGTH);
            for (int i = 0; i < headers.Length; i++)
            {
                headers[i].CreateNode();
            }
            for (int i = 0; i < headers.Length; i++)
            {
                Assert.IsTrue(headers[i].Size == 1);
            }
        }

        private ColumnHeader GetHeaderAt(ColumnHeader root, int index)
        {
            ColumnHeader currHeader = root;
            for (int i = 0; i < index; i++)
            {
                currHeader = currHeader.right;
            }
            return currHeader;
        }

        [TestMethod]
        public void ChooseColumnWillReturnMinColumn()
        {
            ColumnHeader root = new ColumnHeader();
            ColumnHeader[] headers = GetHeaders(root, EXPECTED_LENGTH);
            for (int i = 0; i < EXPECTED_LENGTH - 1; i++)
            {
                headers[i].CreateNode();
            }
            ColumnHeader chosen = ColumnHeader.ChooseColumnHeader(root);
            Assert.AreEqual(headers[EXPECTED_LENGTH - 1], chosen);
        }

        [TestMethod]
        public void CoverWillRemoveRowsFromAllHeaders()
        {
            ColumnHeader root = new ColumnHeader();
            ColumnHeader[] headers = GetHeadersWithLinkedNodes(root);
            headers[0].Cover();
            int summedsizes = 0;
            for (int i = 1; i < EXPECTED_LENGTH; i++)
            {
                summedsizes += headers[i].Size;
            }
            Assert.IsTrue(summedsizes == 0);
        }

        private ColumnHeader[] GetHeadersWithLinkedNodes(ColumnHeader root)
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

        [TestMethod]
        public void CoverWillOnlyRemoveRowsFromLinkedColumns()
        {
            ColumnHeader root = new ColumnHeader();
            ColumnHeader[] headers = GetHeaders(root, EXPECTED_LENGTH);
            Node[] nodes = new Node[EXPECTED_LENGTH / 2];
            for (int i = 0; i < EXPECTED_LENGTH; i++)
            {
                Node created = headers[i].CreateNode();
                if (i % 2 == 0)
                    nodes[i / 2] = created;
            }
            Node.LinkNodes(nodes);
            headers[0].Cover();
            int sum = 0;
            ColumnHeader next = root.right;
            while (next != root)
            {
                sum += next.Size;
                next = next.right;
            }
            Assert.IsTrue(sum == EXPECTED_LENGTH / 2);
        }

        [TestMethod]
        public void CoverWillSetOtherHeadersHeights()
        {
            ColumnHeader root = new ColumnHeader();
            ColumnHeader[] headers = GetHeadersWithLinkedNodes(root);
            headers[0].Cover();
            int sum = 0;
            ColumnHeader currheader = root.right;
            while(currheader != root)
            {
                sum += currheader.Size;
                currheader = currheader.right;
            }
            Assert.IsTrue(sum == 0);
        }

        [TestMethod]
        public void CoverWillCorrectlySetOtherHeadersHeights()
        {
            ColumnHeader root = new ColumnHeader();
            ColumnHeader[] headers = GetHeadersWithLinkedNodes(root);
            Random rand = new Random();
            int numToLinkA = rand.Next(1, EXPECTED_LENGTH - 1);
            int numToLinkB = rand.Next(1, EXPECTED_LENGTH - 1);
            //Header 0 will always only have the one linked row;
            CreateAndLinkNodesToRandomHeaders(headers[0], numToLinkA);
            CreateAndLinkNodesToRandomHeaders(headers[0], numToLinkB);

            headers[0].Cover();
            int sum = 0;
            ColumnHeader currHeader = root.right;
            while (currHeader != root)
            {
                sum += currHeader.Size;
                currHeader = currHeader.right;
            }
            Assert.IsTrue(sum == numToLinkA + numToLinkB);
        }

        [TestMethod]
        public void CoverWillCorrectlySetOtherKnownHeadersHights()
        {
            ColumnHeader root = new ColumnHeader();
            ColumnHeader[] known = GetKnownProblemHeaders(root);
            int[] expectedRemainingSize = GetKnownProblemHeaderCoverResults();
            for (int i = 0; i < known.Length; i++)
            {
                known[i].Cover();
                int sum = 0;
                ColumnHeader temp = root.right;
                while(temp != root)
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

        private void CreateAndLinkNodesToRandomHeaders(ColumnHeader root, int numToLink)
        {
            int AvailableHeaderCount = root.NumLinkedHeaders - 1;
            List<int> workingIndex = new List<int>();
            for (int i = 0; i < AvailableHeaderCount; i++)
            {
                workingIndex.Add(i+1);
            }
            Random rand = new Random();
            Node[] toLink = new Node[numToLink];

            for (int i = 0; i < numToLink; i++)
            {
                int chooseIndex = rand.Next(0, workingIndex.Count);
                toLink[i] = GetHeaderAt(root, workingIndex[chooseIndex]).CreateNode();
                workingIndex.RemoveAt(chooseIndex);
            }
            Node.LinkNodes(toLink);
        }

        [TestMethod]
        public void SearchWillReturnACorrectNodeSolutionStack()
        {
            ColumnHeader root = new ColumnHeader();
            ColumnHeader[] headers = GetHeadersWithLinkedNodes(root);
            Stack<Node> solution = ColumnHeader.Search(root);
            Assert.IsTrue(solution.Count == 1);
        }

        [TestMethod]
        public void SearchWillReturnACorrectNodeSolutionStackForComplicatedMatrix()
        {
            ColumnHeader root = new ColumnHeader();
            ColumnHeader[] headers = GetKnownProblemHeaders(root);
            Node[] solutionNodes = GetKnownProblemSolutionNodes(root);
            Stack<Node> solutionActual = ColumnHeader.Search(root);
            List<ColumnHeader> retrievedHeaders = new List<ColumnHeader>();
            foreach (var solutionnode in solutionNodes)
            {
                retrievedHeaders.AddRange(solutionnode.GetLinkedHeaders());
            }
            Assert.IsTrue(retrievedHeaders.Count == root.NumLinkedHeaders);
            Assert.IsTrue(solutionActual.Count == solutionNodes.Length);

        }

        private Node[] GetKnownProblemSolutionNodes(ColumnHeader root)
        {
            Node[] solutionSet = new Node[SolutionNodeCount];
            solutionSet[0] = GetHeaderAt(root, 1).down.down;
            solutionSet[1] = GetHeaderAt(root, 5).down.down;
            solutionSet[2] = GetHeaderAt(root, 2).down.down;
            return solutionSet;
        }

        private ColumnHeader[] GetKnownProblemHeaders(ColumnHeader root)
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

        private void CreateRowForHeaders(ColumnHeader root, params int[] headerIndexes)
        {
            Node[] toLink = new Node[headerIndexes.Length];
            for (int i = 0; i < headerIndexes.Length; i++)
            {
                toLink[i] = GetHeaderAt(root, headerIndexes[i]).CreateNode();
            }
            Node.LinkNodes(toLink);
        }
    }
}
