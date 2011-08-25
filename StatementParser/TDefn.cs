using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PascalCompiler.StatementParser
{
    public class TDefn
    {
        public TDefnCode how;

        // -- Constant
        public TDataValue constantValue = new TDataValue(); // value of constant


        // -- Procedure, function, or standard routine
        public Routine routine = new Routine();

        public int dataOffset;  // vars and parms: sequence count
                                // fiends: byte offset in record


        public TDefn(TDefnCode dc)
        {
            how = dc;
        }
    }

    public struct Routine
    {
        public TRoutineCode which;      // routine code
        public int parmCount;            // count of parameters
        public int totalParmSize;       // total byte size of parms
        public int totalLocalSize;      //  total byte size of locals
        public TLocalIds locals;        // local identifiers
        public Symtab pSymtab;          // ptr to local symtab
        public IntermediateCode pIcode; // ptr to routine's icode
    }

}
