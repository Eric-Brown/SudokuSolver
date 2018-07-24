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


        [TestMethod]
        public void DefaultConstructionLoopsToSelf()
        {
            ColumnHeader defaultHeader = new ColumnHeader();
            Assert.IsTrue(defaultHeader.left.Equals(defaultHeader.right));
            Assert.IsTrue(defaultHeader.down.Equals(defaultHeader.up));
            Assert.IsTrue(defaultHeader.header.Equals(defaultHeader));
        }
        [TestMethod]
        public void HeaderAsConstructionParameterLinksHeadersTogether()
        {
            HeaderRoot root = GetRoot();
            ColumnHeader next = new ColumnHeader(root);
            Assert.IsTrue(root == next.left);
            Assert.IsTrue(next.right==root);
            Assert.IsFalse(root.Equals(next.up));
            Assert.IsFalse(root.Equals(next.down));
        }

        private static HeaderRoot GetRoot()
        {
            return new MockHeaderRoot();
        }

        [TestMethod]
        public void LinkedHeadersCountedCorrectly()
        {
            HeaderRoot root = GetRoot();
            ColumnHeader[] headers = GetHeaders(root, EXPECTED_LENGTH);
            Assert.IsTrue(root.Count == EXPECTED_LENGTH);
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

        [TestMethod]
        public void HidingRemovesHeaderFromLinkedList()
        {
            HeaderRoot root = GetRoot();
            ColumnHeader next = new ColumnHeader(root);
            next.Hide();
            Assert.IsTrue(root.Count == 0);
        }

        [TestMethod]
        public void UnhidingAddsHeaderToLinkedListInOriginalPosition()
        {
            HeaderRoot root = GetRoot();
            ColumnHeader[] headers = GetHeaders(root, EXPECTED_LENGTH);
            ColumnHeader toTest = GetHeaderAt(root, EXPECTED_LENGTH);
            toTest.Hide();
            toTest.Unhide();
            Assert.IsTrue(GetHeaderAt(root, EXPECTED_LENGTH).Equals(toTest));
        }

        [TestMethod]
        public void UnhidingManyAddsHeaderToLinkedListInOriginalPosition()
        {
            HeaderRoot root = GetRoot();
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
            HeaderRoot root = GetRoot();
            ColumnHeader[] headers = GetHeaders(root, EXPECTED_LENGTH);
            headers[0].Cover();
            Assert.IsTrue(root.Count == EXPECTED_LENGTH - 1);
        }

        [TestMethod]
        public void UncoverWorksWithNoNodes()
        {
            HeaderRoot root = GetRoot();
            ColumnHeader[] headers = GetHeaders(root, EXPECTED_LENGTH);
            headers[0].Cover();
            headers[0].Uncover();
            Assert.IsTrue(root.Count == EXPECTED_LENGTH);
        }
        [TestMethod]
        public void UncoverRestoresRowsToOtherHeaders()
        {
            HeaderRoot root = GetRoot();
            ColumnHeader[] headers = GetHeaders(root, EXPECTED_LENGTH);
            CreateAndLinkNodesToRandomHeaders(root, EXPECTED_LENGTH/2);
            CreateAndLinkNodesToRandomHeaders(root, EXPECTED_LENGTH/2);
            CreateAndLinkNodesToRandomHeaders(root, EXPECTED_LENGTH/2);
            CreateAndLinkNodesToRandomHeaders(root, EXPECTED_LENGTH/2);
            CreateAndLinkNodesToRandomHeaders(root, EXPECTED_LENGTH/2);
            CreateAndLinkNodesToRandomHeaders(root, EXPECTED_LENGTH / 2);
            headers[0].Cover();
            headers[0].Uncover();
            int sum = 0;
            for(int i = 0; i < EXPECTED_LENGTH; ++i)
            {
                sum+= headers[i].Size;
            }
            Assert.IsTrue(sum == EXPECTED_LENGTH * 3);
        }
        [TestMethod]
        public void CoveringAllHeadersWillLeaveRootLinkingToSelf()
        {
            HeaderRoot root = GetRoot();
            ColumnHeader[] headers = GetHeaders(root, EXPECTED_LENGTH);
            for (int i = 0; i < headers.Length; i++)
            {
                headers[i].Cover();
            }
            Assert.IsTrue(root.right == root);
            Assert.IsTrue(root.left == root);
        }

        [TestMethod]
        public void NodesCanBeAddedToHeaders()
        {
            HeaderRoot root = GetRoot();
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

        private ColumnHeader GetHeaderAt(HeaderRoot root, int index)
        {
            ColumnHeader currHeader = root.right;
            for (int i = 0; i < index; i++)
            {
                currHeader = currHeader.right;
            }
            return currHeader;
        }

        [TestMethod]
        public void ChooseColumnWillReturnMinColumn()
        {
            HeaderRoot root = GetRoot();
            ColumnHeader[] headers = GetHeaders(root, EXPECTED_LENGTH);
            for (int i = 0; i < EXPECTED_LENGTH - 1; i++)
            {
                headers[i].CreateNode();
            }
            ColumnHeader chosen = root.ChooseColumnHeader();
            Assert.AreEqual(headers[EXPECTED_LENGTH - 1], chosen);
        }

        [TestMethod]
        public void CoverWillRemoveRowsFromAllHeaders()
        {
            HeaderRoot root = GetRoot();
            ColumnHeader[] headers = GetHeadersWithLinkedNodes(root);
            headers[0].Cover();
            int summedsizes = 0;
            for (int i = 1; i < EXPECTED_LENGTH; i++)
            {
                summedsizes += headers[i].Size;
            }
            Assert.IsTrue(summedsizes == 0);
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

        [TestMethod]
        public void CoverWillOnlyRemoveRowsFromLinkedColumns()
        {
            HeaderRoot root = GetRoot();
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
            HeaderRoot root = GetRoot();
            ColumnHeader[] headers = GetHeadersWithLinkedNodes(root);
            headers[0].Cover();
            int sum = 0;
            ColumnHeader currheader = root.right;
            while (currheader != root)
            {
                sum += currheader.Size;
                currheader = currheader.right;
            }
            Assert.IsTrue(sum == 0);
        }

        [TestMethod]
        public void CoverWillCorrectlySetOtherHeadersHeights()
        {
            HeaderRoot root = GetRoot();
            ColumnHeader[] headers = GetHeadersWithLinkedNodes(root);
            headers[0].Cover();
            Random rand = new Random();
            int numToLinkA = rand.Next(1, EXPECTED_LENGTH - 1);
            int numToLinkB = rand.Next(1, EXPECTED_LENGTH - 1);
            //Header 0 will always only have the one linked row;
            CreateAndLinkNodesToRandomHeaders(root, numToLinkA);
            CreateAndLinkNodesToRandomHeaders(root, numToLinkB);

            int sum = 0;
            ColumnHeader currHeader = root.right;
            while (currHeader != root)
            {
                sum += currHeader.Size;
                currHeader = currHeader.right;
            }
            Assert.IsTrue(sum == numToLinkA + numToLinkB);
        }



        /// <summary>
        /// TODO: FIX THIS
        /// </summary>
        /// <param name="root"></param>
        /// <param name="numToLink"></param>
        private void CreateAndLinkNodesToRandomHeaders(HeaderRoot root, int numToLink)
        {
            int AvailableHeaderCount = root.Count;
            List<int> workingIndex = new List<int>();
            for (int i = 0; i < AvailableHeaderCount; i++)
            {
                workingIndex.Add(i);
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





    }
}
