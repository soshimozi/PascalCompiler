using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PascalCompiler.StatementParser
{
    public class Symtab : AATree<string, SymtabNode>
    {
        SymtabNode[] vpNodes;
        int _xSymtab;
        short nodeCount;
        public Symtab next;

        private static int tableCount = 0;
        public static Symtab CreateTable()
        {
            return new Symtab(tableCount++);
        }

        protected Symtab(int tableIndex)
        {
            _xSymtab = tableIndex;
            vpNodes = null;

            next = SharedProperties.pSymtabList;
            SharedProperties.pSymtabList = this;
        }

        public SymtabNode Enter(String pString, TDefnCode dc = TDefnCode.dcUndefined)
        {
            SymtabNode pNode = this[pString];
            if (pNode != null) return pNode;

            pNode = new SymtabNode(pString, dc);
            pNode.SymtabIndex = _xSymtab;
            pNode.NodeIndex = nodeCount++;

            Add(pString, pNode);
            return pNode;
        }

        public SymtabNode EnterNew(String pString, TDefnCode dc = TDefnCode.dcUndefined)
        {
            SymtabNode pNode = this[pString];
            if (pNode == null) pNode = Enter(pString, dc);
            else throw new IdentifierRedefinedException();

            return pNode;
        }

        public void Convert(Symtab[] vpSymtabs)
        {
            vpSymtabs[this._xSymtab] = this;
            vpNodes = this.Values;
        }

        public SymtabNode Search(string pString)
        {
            return this[pString];
        }
    }
}
