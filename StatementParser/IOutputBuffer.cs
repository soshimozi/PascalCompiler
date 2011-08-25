using System;
using System.Collections.Generic;
using System.Text;

namespace PascalCompiler.StatementParser
{
    public interface IOutputBuffer
    {
        void PutLine();
        void PutLine(string line);
    }
}
