using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PascalCompiler.StatementParser
{
    class LineNumNode
    {
        private readonly int _lineNumber;

        public LineNumNode(int lineNumber)
        {
            _lineNumber = lineNumber;
        }
    }
}
