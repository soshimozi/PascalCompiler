using System;
using System.Collections.Generic;
using System.Text;

namespace PascalCompiler.StatementParser
{
    public class ReservedWord
    {
        public string TokenString
        {
            get;
            set;
        }

        public TTokenCode Code
        {
            get;
            set;
        }

    }

}
