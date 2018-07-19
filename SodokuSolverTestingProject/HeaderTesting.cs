using System;
using SudokuSolver;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SodokuSolverTestingProject
{
    [TestClass]
    public class HeaderTesting
    {
        private const int EXPECTED_LENGTH = 10;

        [TestMethod]
        public void DefaultConstructionLoopsToSelf()
        {
            ColumnHeader root = new ColumnHeader();
            Assert.IsTrue(root.left.Equals(root.right));
            Assert.IsTrue(root.below.Equals(root.above));
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
            Assert.IsFalse(root.Equals(next.above));
            Assert.IsFalse(root.Equals(next.below));
        }

        [TestMethod]
        public void LinkedHeadersCountedCorrectly()
        {
            ColumnHeader root = new ColumnHeader();
            ColumnHeader[] headers =  GetHeaders(root, EXPECTED_LENGTH);
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
            HideUnhideOne(root, headers);
            HideUnhideRandomHeaders(root);
        }

        private void HideUnhideOne(ColumnHeader root, ColumnHeader[] headers)
        {
            ColumnHeader toTest = GetHeaderAt(root, EXPECTED_LENGTH);
            toTest.Hide();
            toTest.Unhide();
            Assert.IsTrue(GetHeaderAt(root, EXPECTED_LENGTH).Equals(toTest));
        }

        private void HideUnhideRandomHeaders(ColumnHeader root)
        {
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
            Assert.IsTrue(root.NumLinkedHeaders == EXPECTED_LENGTH - numToHide);
            foreach (ColumnHeader toHide in hiddenHeaders)
            {
                toHide.Unhide();
            }
            Assert.IsTrue(root.NumLinkedHeaders == EXPECTED_LENGTH);
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
    }
}
