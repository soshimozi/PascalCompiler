using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PascalCompiler.StatementParser
{
    public class SymtabStack
    {
        private const int maxNestingLevel = 32;

        Symtab[] pSymtabs = new Symtab[maxNestingLevel];

        public SymtabStack(Symtab globalSymbolTable)
        {
            SharedProperties.CurrentNestingLevel = 0;
            for (int i = 1; i < maxNestingLevel; ++i) pSymtabs[i] = null;

            // -- Initialize the global nesting level.
            pSymtabs[0] = globalSymbolTable;

            TType.InitializePredefinedTypes(globalSymbolTable);
            InitializeStandardRoutines(globalSymbolTable);

        }

        struct TStdRtn {
            public string         pName;
            public TRoutineCode  rc;
            public TDefnCode     dc;
        };

        TStdRtn[] stdRtnList = new TStdRtn[] {
            new TStdRtn() {pName = "read",    rc = TRoutineCode.rcRead,    dc = TDefnCode.dcProdecure},
            new TStdRtn() {pName = "readln",  rc = TRoutineCode.rcReadln,  dc = TDefnCode.dcProdecure},
            new TStdRtn() {pName ="write",   rc = TRoutineCode.rcWrite,   dc = TDefnCode.dcProdecure},
            new TStdRtn() {pName = "writeln", rc = TRoutineCode.rcWriteln, dc = TDefnCode.dcProdecure},
            new TStdRtn() {pName = "abs",     rc = TRoutineCode.rcAbs,     dc = TDefnCode.dcFunction},
            new TStdRtn() {pName = "arctan",  rc = TRoutineCode.rcArctan,  dc = TDefnCode.dcFunction},
            new TStdRtn() {pName = "chr",     rc = TRoutineCode.rcChr,     dc = TDefnCode.dcFunction},
            new TStdRtn() {pName = "cos",     rc = TRoutineCode.rcCos,     dc = TDefnCode.dcFunction},
            new TStdRtn() {pName = "eof",     rc = TRoutineCode.rcEof,     dc = TDefnCode.dcFunction},
            new TStdRtn() {pName = "eoln",    rc = TRoutineCode.rcEoln,    dc = TDefnCode.dcFunction},
            new TStdRtn() {pName = "exp",     rc = TRoutineCode.rcExp,     dc = TDefnCode.dcFunction},
            new TStdRtn() {pName = "ln",      rc = TRoutineCode.rcLn,      dc = TDefnCode.dcFunction},
            new TStdRtn() {pName = "odd",     rc = TRoutineCode.rcOdd,     dc = TDefnCode.dcFunction},
            new TStdRtn() {pName = "ord",     rc = TRoutineCode.rcOrd,     dc = TDefnCode.dcFunction},
            new TStdRtn() {pName = "pred",    rc = TRoutineCode.rcPred,    dc = TDefnCode.dcFunction},
            new TStdRtn() {pName = "round",   rc = TRoutineCode.rcRound,   dc = TDefnCode.dcFunction},
            new TStdRtn() {pName = "sin",     rc = TRoutineCode.rcSin,     dc = TDefnCode.dcFunction},
            new TStdRtn() {pName = "sqr",     rc = TRoutineCode.rcSqr,     dc = TDefnCode.dcFunction},
            new TStdRtn() {pName = "sqrt",    rc = TRoutineCode.rcSqrt,    dc = TDefnCode.dcFunction},
            new TStdRtn() {pName = "succ",    rc = TRoutineCode.rcSucc,    dc = TDefnCode.dcFunction},
            new TStdRtn() {pName = "trunc",   rc = TRoutineCode.rcTrunc,   dc = TDefnCode.dcFunction}
        };


        private void InitializeStandardRoutines(Symtab pSymtab)
        {
            foreach (TStdRtn pSR in stdRtnList)
            {
                SymtabNode pRoutineId = pSymtab.Enter(pSR.pName, pSR.dc);

                pRoutineId.defn.routine.which = pSR.rc;
                pRoutineId.defn.routine.parmCount = 0;
                pRoutineId.defn.routine.totalParmSize = 0;
                pRoutineId.defn.routine.totalLocalSize = 0;
                pRoutineId.defn.routine.locals.pParmIds = null;
                pRoutineId.defn.routine.locals.pConstantIds = null;
                pRoutineId.defn.routine.locals.pTypeIds = null;
                pRoutineId.defn.routine.locals.pVariableIds = null;
                pRoutineId.defn.routine.locals.pRoutineIds = null;
                pRoutineId.defn.routine.pSymtab = null;
                pRoutineId.defn.routine.pIcode = null;
                TType.SetType(ref pRoutineId.pType, TType.pDummyType);
            }

        }

        public SymtabNode SearchLocal(String pString)
        {
            return pSymtabs[SharedProperties.CurrentNestingLevel].Search(pString);
        }

        public SymtabNode EnterLocal(String pString, TDefnCode dc = TDefnCode.dcUndefined)
        {
            return pSymtabs[SharedProperties.CurrentNestingLevel].Enter(pString, dc);
        }

        public SymtabNode EnterNewLocal(String pString, TDefnCode dc = TDefnCode.dcUndefined)
        {
            return pSymtabs[SharedProperties.CurrentNestingLevel].EnterNew(pString, dc);
        }

        public Symtab CurrentSymtab
        {
            get { return pSymtabs[SharedProperties.CurrentNestingLevel]; }
            set { pSymtabs[SharedProperties.CurrentNestingLevel] = value; }
        }

        public SymtabNode SearchAll(String pString)
        {
            for (int i = SharedProperties.CurrentNestingLevel; i >= 0; --i)
            {
                SymtabNode pNode = pSymtabs[i].Search(pString);
                if (pNode != null) return pNode;
            }

            return null;
        }

        public SymtabNode Find(String pString)
        {
            SymtabNode pNode = SearchAll(pString);
            if (pNode == null)
            {
                pNode = pSymtabs[SharedProperties.CurrentNestingLevel].Enter(pString);
            }

            return pNode;
        }

        public void EnterScope()
        {
            if (++SharedProperties.CurrentNestingLevel > maxNestingLevel)
            {
                throw new NestingTooDeepException();
            }

            SetCurrentSymtab(Symtab.CreateTable());
        }

        private void SetCurrentSymtab(Symtab symtab)
        {
            pSymtabs[SharedProperties.CurrentNestingLevel] = symtab;
        }

        public Symtab ExitScope()
        {
            return pSymtabs[SharedProperties.CurrentNestingLevel--];
        }

        public Symtab GlobalSymtab
        {
            get { return pSymtabs[0]; }
        }
    }
}
