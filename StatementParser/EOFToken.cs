using System;
using System.Collections.Generic;
using System.Text;

namespace PascalCompiler.StatementParser
{
    public class EOFToken : TToken
    {
        public override void Get(IInputBuffer buffer)
        {
            code = TTokenCode.EndOfFile;
        }

        public override void Print(IOutputBuffer buffer)
        {
            throw new NotImplementedException();
        }
    }
}
