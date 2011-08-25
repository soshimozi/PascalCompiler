using System;
using System.Collections.Generic;
using System.Text;

namespace PascalCompiler.StatementParser
{
    class ErrorToken : TToken
    {
        public ErrorToken() { code = TTokenCode.Error; }

        public override void Get(IInputBuffer buffer)
        {
            StringBuilder tokenBuilder = new StringBuilder();
            tokenBuilder.Append(buffer.CurrentChar);
            tokenString = tokenBuilder.ToString();
            buffer.GetChar();

            code = TTokenCode.Error;
        }

        public override void Print(IOutputBuffer buffer)
        {
            throw new NotImplementedException();
        }
    }
}
