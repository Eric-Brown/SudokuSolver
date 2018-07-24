using System;
using SudokuSolver;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SudokuSolverTests
{
    [TestClass]
    public class NodeTesting
    {
        [TestMethod]
        public void NodeCanBeConstructedWithDefaultConstructor()
        {
            
            Node node = new Node();
            Assert.IsTrue(node.up.Equals(node.down));
            Assert.IsTrue(node.right.Equals(node.left));
            Assert.IsTrue(node.right.Equals(node));
            Assert.IsTrue(node.header == null);
        }

        [TestMethod]
        public void NodeCanBeConstructedWithHeader()
        {
            ColumnHeader header = new ColumnHeader();
            Node node = new Node(header);
            Assert.IsTrue(node.header.Equals(header));
            Assert.AreEqual(header, node.header);
            Assert.IsTrue(node.left.Equals(node.right));
            Assert.IsTrue(node.up.Equals(header));
            Assert.IsTrue(node.down.Equals(header));
        }
        
        [TestMethod]
        public void NodeCanGetLinkedHeaders()
        {
            HeaderRoot root = GetRoot();
            ColumnHeader headerSingle = new ColumnHeader(root);
            Node single = headerSingle.CreateNode();
            ColumnHeader headerSingleMultiRow = new ColumnHeader(root);
            Node oneLinked = headerSingleMultiRow.CreateNode();
            Node twoLinked1 = headerSingleMultiRow.CreateNode();
            Node twoLinked2 = headerSingle.CreateNode();
            Node.LinkNodes(twoLinked1, twoLinked2);
            Assert.IsTrue(single.GetLinkedHeaders().Count == 1);
            Assert.IsTrue(oneLinked.GetLinkedHeaders().Count == 1);
            Assert.IsTrue(twoLinked1.GetLinkedHeaders().Count == 2);
        }

        [TestMethod]
        public void NodeCanRemoveRow()
        {
            HeaderRoot root = GetRoot();
            ColumnHeader headerSingle = new ColumnHeader(root);
            ColumnHeader otherHeader = new ColumnHeader(root);
            Node toRemove = headerSingle.CreateNode();
            Node linked = otherHeader.CreateNode();
            Node.LinkNodes(toRemove, linked);
            toRemove.RemoveRow();
            Assert.IsTrue(headerSingle.Size == 1);
            Assert.IsTrue(otherHeader.Size == 0);
        }

        [TestMethod]
        public void RemoveRowSetsOtherHeadersHeightsCorrectly()
        {
            HeaderRoot root = GetRoot();
            ColumnHeader headerSingle = new ColumnHeader(root);
            ColumnHeader otherHeader = new ColumnHeader(root);
            ColumnHeader otherHeader2 = new ColumnHeader(root);
            Node toRemove = headerSingle.CreateNode();
            Node linked = otherHeader.CreateNode();

            Node linkedH2 = otherHeader2.CreateNode();
            Node.LinkNodes(toRemove, linked, linkedH2);


            Node linked2 = otherHeader.CreateNode();
            Node toRemove2 = otherHeader2.CreateNode();
            Node.LinkNodes(toRemove2, linked2);

            otherHeader.CreateNode();
            otherHeader.CreateNode();

            toRemove.RemoveRow();
            toRemove2.RemoveRow();
            Assert.IsTrue(headerSingle.Size == 1);
            Assert.IsTrue(otherHeader.Size == 2);
            Assert.IsTrue(otherHeader2.Size == 1);
        }

        [TestMethod]
        public void GetLinkedHeadersReturnsAccurateHeaders()
        {
            HeaderRoot root = GetRoot();
            ColumnHeader[] headers = new ColumnHeader[2];
            Node[] nodes = new Node[2];
            for (int i = 0; i < 2; i++)
            {
                headers[i] = root.CreateHeader();
                nodes[i] = headers[i].CreateNode();
            }
            Node.LinkNodes(nodes);
            Assert.IsTrue(root[0].down.GetLinkedHeaders().Count == 2);
        }

        private HeaderRoot GetRoot()
        {
            return new MockHeaderRoot();
        }
    }
}
