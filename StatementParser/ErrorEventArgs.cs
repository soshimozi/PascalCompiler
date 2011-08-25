using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PascalCompiler.StatementParser
{
    class ErrorEventArgs : EventArgs
    {
        public TErrorCode ErrorCode { get; set; }

        public int CharacterPosition { get; set; }

        public int CurrentLine { get; set; }
    }
}
