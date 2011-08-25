using System;
using System.Collections.Generic;
using System.Text;

namespace PascalCompiler.StatementParser
{
    public interface IScanner
    {
        TToken Get();
    }
}
