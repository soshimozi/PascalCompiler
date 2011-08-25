using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PascalCompiler.StatementParser
{
    public class SymtabNode
    {
        //SymtabNode left, right;
        string pString;
        int xSymtab;
        int xNode;

        int nestingLevel;
        int labelIndex;

        public TType pType;
        public TDefn defn;

        public int NestingLevel
        {
            get { return nestingLevel; }
            set { nestingLevel = value; }
        }

        public int LabelIndex
        {
            get { return labelIndex; }
            set { labelIndex = value; }
        }

        public SymtabNode Next { get; set; }

        public String String { get { return pString; } set { pString = value; } }
        public int SymtabIndex { get { return xSymtab; } set { xSymtab = value; } }
        public int NodeIndex { get { return xNode; } set { xNode = value; } }

        //public DefnCode Dc { get; set; }

        public SymtabNode(String pString, TDefnCode dc = TDefnCode.dcUndefined)
        {
            this.pString = pString;
            defn = new TDefn(dc);

            xNode = 0;
            nestingLevel = SharedProperties.CurrentNestingLevel;
            labelIndex = SharedProperties.AsmLableIndex++;
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public float Value { get; set; }
    }
}
