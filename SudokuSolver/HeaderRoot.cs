using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    public abstract class HeaderRoot : ColumnHeader
    {
        public abstract int Count
        {
            get;
        }

        public virtual ColumnHeader this[int index]
        {
            get
            {
                if (index > Count || index < 0) throw new ArgumentOutOfRangeException();
                ColumnHeader toReturn = right;
                for (int i = 0; i < index; i++)
                {
                    toReturn = toReturn.right;
                }
                return toReturn;
            }
        }

        public virtual Stack<Node> Search()
        {
            Stack<Node> result = new Stack<Node>();
            bool isDone = false;
            DLXSearch(result, ref isDone);
            foreach (var node in result)
            {
                node.Unselect();
                node.header.Uncover();
            }
            return result;
        }
        public abstract ColumnHeader CreateHeader();

        public virtual ColumnHeader ChooseColumnHeader()
        {
            ColumnHeader minHeader = right;
            ColumnHeader currHeader = right;
            while (currHeader != this)
            {

                if (currHeader.Size < minHeader.Size)
                {
                    minHeader = currHeader;
                }
                currHeader = currHeader.right;
            }
            return minHeader;
        }
        private void DLXSearch(Stack<Node> solution, ref bool done)
        {
            if (right == this)
            {
                done = true;
                return;
            }
            else
            {
                ColumnHeader chosenColumn = ChooseColumnHeader();
                Node chosenRow = chosenColumn.down;
                chosenColumn.Cover();
                while (!chosenRow.Equals(chosenColumn))
                {
                    solution.Push(chosenRow);
                    chosenRow.Select();
                    DLXSearch(solution, ref done);
                    if (done) return;
                    chosenRow = solution.Pop();
                    chosenColumn = chosenRow.header;
                    chosenRow.Unselect();
                    chosenRow = chosenRow.down;
                }
                chosenColumn.Uncover();
                return;
            }
        }

    }
}
