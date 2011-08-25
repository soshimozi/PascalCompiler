using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PascalCompiler.StatementParser
{
    public struct TLocalIds
    {
        public SymtabNode pParmIds;         // ptr to local parm id list
        public SymtabNode pConstantIds;     // ptr to local constant id list
        public SymtabNode pTypeIds;         // ptr to local type idlist
        public SymtabNode pVariableIds;     // ptr to local variable id list
        public SymtabNode pRoutineIds;      // ptr to local proc and func id list
    }
}
