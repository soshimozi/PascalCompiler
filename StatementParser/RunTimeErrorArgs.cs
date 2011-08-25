using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PascalCompiler.StatementParser
{
    public class RuntimeErrorArgs : EventArgs
    {
        public TRuntimeError RuntimeError
        {
            get;
            set;
        }

        public int LineNumber { get; set; }
    }

    public enum TRuntimeError
    {
        rteNone,
        rteStackOverflow,
        rteValueOutOfRane,
        rteInvalidCaseValue,
        rteDivisionByZero,
        rteInvalidFunctionArgument,
        rteInvalidUserInput,
        rteUnimplementedRuntimeFeature
    }


}
