using System;
using SudokuSolver;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SodokuSolverTestingProject
{
    [TestClass]
    public class NodeTesting
    {
        [TestMethod]
        public void NodeCanBeConstructedWithDefaultConstructor()
        {
            Node node = new Node();
            Assert.IsTrue(node.above.Equals(node.below));
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
            Assert.IsTrue(node.left.Equals(node.right));
            Assert.IsTrue(node.above.Equals(root));
            Assert.IsTrue(node.below.Equals(root));
        }
    }
}
