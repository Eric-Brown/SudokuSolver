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
            ColumnHeader root = new ColumnHeader();
            Node node = new Node(root);
            Assert.IsTrue(node.header.Equals(root));
            Assert.AreEqual(root, node.header);
            Assert.IsTrue(node.left.Equals(node.right));
            Assert.IsTrue(node.up.Equals(root));
            Assert.IsTrue(node.down.Equals(root));
        }
        
        [TestMethod]
        public void NodeCanGetLinkedHeaders()
        {
            ColumnHeader root = new ColumnHeader();
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
            ColumnHeader root = new ColumnHeader();
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
            ColumnHeader root = new ColumnHeader();
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
    }
}
