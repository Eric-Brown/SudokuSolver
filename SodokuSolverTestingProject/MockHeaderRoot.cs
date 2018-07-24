using System;
using SudokuSolver;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolverTests
{
    class MockHeaderRoot : HeaderRoot
    {
        private int nextKey = 0;
        public override int Count
        {
            get
            {
                int result = 0;
                ColumnHeader header = right;
                while(header != this)
                {
                    result++;
                    header = header.right;
                }
                return result;
            }
        }

        public override ColumnHeader CreateHeader()
        {
            return new ColumnHeader(this, nextKey++);
        }
    }
}
