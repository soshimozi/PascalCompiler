using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PascalCompiler.StatementParser
{
    static class SharedProperties
    {
        public static int AsmLableIndex = 0;
        public static int CurrentNestingLevel = 0;
        //public static short SymbolTableCount = 0;
        public static Symtab pSymtabList = null;
        public static Symtab globalSymtab = null;

    }
}
