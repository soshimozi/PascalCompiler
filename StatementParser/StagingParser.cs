using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PascalCompiler.StatementParser
{
    class StagingParser
    {
        private TextScanner pScanner;          // ptr to the scanner
        private TToken pToken;              // ptr to the current token
        private TTokenCode token;            // code of current token
        private SymtabStack symtabStack;    // symbol table stack
        private IntermediateCode icode;     // icode buffer
        //private TListBuffer list = new TListBuffer(true, 25, 80);
        private TextBuffer buffer;

        public event EventHandler<ErrorEventArgs> Error;

        private readonly TTokenCode[] tlDeclarationStart = new TTokenCode[]
        {
            TTokenCode.Const,
            TTokenCode.Type,
            TTokenCode.Var,
            TTokenCode.Procedure,
            TTokenCode.Function,
            TTokenCode.Dummy
        };

        private readonly TTokenCode[] tlDeclarationFollow = new TTokenCode[]
        {
            TTokenCode.Semicolon,
            TTokenCode.Identifier,
            TTokenCode.Dummy
        };

        private readonly TTokenCode[] tlFormalParmsFollow
            = new TTokenCode[] {
                TTokenCode.RParen,
                TTokenCode.Semicolon,
                TTokenCode.Dummy
            };

        private readonly TTokenCode[] tlActualVarParmFollow
            = new TTokenCode[] {
                TTokenCode.Colon,
                TTokenCode.RParen,
                TTokenCode.Dummy
            };


        private readonly TTokenCode[] tlFuncIdFollow
            = new TTokenCode[] {
                TTokenCode.LParen,
                TTokenCode.Colon,
                TTokenCode.Semicolon,
                TTokenCode.Dummy
            };


        private readonly TTokenCode[] tlProgProcIdFollow
            = new TTokenCode[] {
                TTokenCode.LParen,
                TTokenCode.Semicolon,
                TTokenCode.Dummy
            };


        private readonly TTokenCode[] tlHeaderFollow
            = new TTokenCode[] {
                TTokenCode.Semicolon,
                TTokenCode.Dummy
            };


        private readonly TTokenCode[] tlProcFuncFollow
            = new TTokenCode[] {
                TTokenCode.Semicolon,
                TTokenCode.Dummy
            };

        private readonly TTokenCode[] tlProcFuncStart
            = new TTokenCode[] {
                TTokenCode.Procedure,
                TTokenCode.Function,
                TTokenCode.Dummy
            };

        private readonly TTokenCode[] tlProgramEnd
            = new TTokenCode[] {
                TTokenCode.Period,
                TTokenCode.Dummy
            };

        private readonly TTokenCode[] tlStatementStart
            = new TTokenCode[] { 
                TTokenCode.Begin, 
                TTokenCode.Case, 
                TTokenCode.For, 
                TTokenCode.If, 
                TTokenCode.Repeat, 
                TTokenCode.While, 
                TTokenCode.Identifier,
                TTokenCode.Dummy 
            };

        private readonly TTokenCode[] tlStatementFollow
            = new TTokenCode[] {
                TTokenCode.Semicolon, 
                TTokenCode.Period, 
                TTokenCode.End, 
                TTokenCode.Else, 
                TTokenCode.Until, 
                TTokenCode.Dummy
            };

        private readonly TTokenCode[] tlColonEqual
            = new TTokenCode[] {
                TTokenCode.ColonEqual,   
                TTokenCode.Dummy
            };

        private readonly TTokenCode[] tlExpressionStart
            = new TTokenCode[] {
                TTokenCode.Plus, 
                TTokenCode.Minus, 
                TTokenCode.Identifier, 
                TTokenCode.Number, 
                TTokenCode.String,
                TTokenCode.Not, 
                TTokenCode.LParen, 
                TTokenCode.Dummy
            };

        private readonly TTokenCode[] tlRelOps
            = new TTokenCode[] {
                TTokenCode.Equal, 
                TTokenCode.Ne, 
                TTokenCode.Lt, 
                TTokenCode.Gt, 
                TTokenCode.Le, 
                TTokenCode.Ge, 
                TTokenCode.Dummy
            };

        private readonly TTokenCode[] tlExpressionFollow
            = new TTokenCode[] {
                TTokenCode.Comma, 
                TTokenCode.RParen, 
                TTokenCode.RBracket, 
                TTokenCode.Colon, 
                TTokenCode.Then, 
                TTokenCode.To, 
                TTokenCode.Downto,
                TTokenCode.Do, 
                TTokenCode.Of, 
                TTokenCode.Dummy
            };

        private readonly TTokenCode[] tlUnaryOps
            = new TTokenCode[] {
                TTokenCode.Plus, 
                TTokenCode.Minus, 
                TTokenCode.Dummy
            };

        private readonly TTokenCode[] tlAddOps
            = new TTokenCode[] {
                TTokenCode.Plus, 
                TTokenCode.Minus, 
                TTokenCode.Or, 
                TTokenCode.Dummy
            };

        private readonly TTokenCode[] tlMulOps
            = new TTokenCode[] {
                TTokenCode.Star, 
                TTokenCode.Slash, 
                TTokenCode.Div, 
                TTokenCode.Mod, 
                TTokenCode.And, 
                TTokenCode.Dummy
            };

        private readonly TTokenCode[] tlDo
            = new TTokenCode[] {
                TTokenCode.Do,           
                TTokenCode.Dummy
            };

        private readonly TTokenCode[] tlThen
            = new TTokenCode[] {
                TTokenCode.Then,
                TTokenCode.Dummy
            };

        private readonly TTokenCode[] tlEnd
            = new TTokenCode[] {
                TTokenCode.End,          
                TTokenCode.Dummy
            };

        private readonly TTokenCode[] tlCaseLabelStart
            = new TTokenCode[] {
                TTokenCode.Identifier, 
                TTokenCode.Number, 
                TTokenCode.Plus, 
                TTokenCode.Minus, 
                TTokenCode.String, 
                TTokenCode.Dummy
            };

        private readonly TTokenCode[] tlOf
            = new TTokenCode[] {
                TTokenCode.Of,           
                TTokenCode.Dummy
            };

        private readonly TTokenCode[] tlColon
            = new TTokenCode[] {
                TTokenCode.Colon,        
                TTokenCode.Dummy
            };

        private readonly TTokenCode[] tlToDownto
            = new TTokenCode[] {
                TTokenCode.To,        
                TTokenCode.Downto,
                TTokenCode.Dummy
            };


        private readonly TTokenCode[] tlFieldDeclFollow
            = new TTokenCode[]  {
                TTokenCode.Semicolon,
                TTokenCode.Identifier,
                TTokenCode.End,
                TTokenCode.Dummy
            };

        private readonly TTokenCode[] tlSublistFollow
            = new TTokenCode[]  {
                TTokenCode.Colon,
                TTokenCode.Dummy
            };

        private readonly TTokenCode[] tlIdentifierStart
            = new TTokenCode[]  {
                TTokenCode.Identifier,
                TTokenCode.Dummy
            };

        private readonly TTokenCode[] tlIdentifierFollow
            = new TTokenCode[]  {
                TTokenCode.Comma,
                TTokenCode.Identifier,
                TTokenCode.Colon,
                TTokenCode.Semicolon,
                TTokenCode.Dummy
            };


        private readonly TTokenCode[] tlEnumConstStart
            = new TTokenCode[]  {
                TTokenCode.Identifier,
                TTokenCode.Dummy
            };

        private readonly TTokenCode[] tlEnumConstFollow
            = new TTokenCode[]  {
                TTokenCode.Comma,
                TTokenCode.Identifier,
                TTokenCode.RParen,
                TTokenCode.Semicolon,
                TTokenCode.Dummy
            };


        private readonly TTokenCode[] tlSubrangeLimitFollow
            = new TTokenCode[]  {
                TTokenCode.DotDot,
                TTokenCode.Identifier,
                TTokenCode.Plus,
                TTokenCode.Minus,
                TTokenCode.String,
                TTokenCode.RBracket,
                TTokenCode.Comma,
                TTokenCode.Semicolon,
                TTokenCode.Of,
                TTokenCode.Dummy
            };

        private readonly TTokenCode[] tlIndexStart
            = new TTokenCode[]  {
                TTokenCode.Identifier,
                TTokenCode.Number,
                TTokenCode.String,
                TTokenCode.LParen,
                TTokenCode.Plus,
                TTokenCode.Minus,
                TTokenCode.Dummy
            };

        private readonly TTokenCode[] tlIndexFollow
            = new TTokenCode[]  {
                TTokenCode.Comma,
                TTokenCode.RBracket,
                TTokenCode.Of,
                TTokenCode.Semicolon,
                TTokenCode.Dummy
            };

        private readonly TTokenCode[] tlIndexListFolow
            = new TTokenCode[]  {
                TTokenCode.Of,
                TTokenCode.Identifier,
                TTokenCode.LParen,
                TTokenCode.Array,
                TTokenCode.Record,
                TTokenCode.Plus,
                TTokenCode.Minus,
                TTokenCode.Number,
                TTokenCode.String,
                TTokenCode.Semicolon,
                TTokenCode.Dummy
            };

        private readonly TTokenCode[] tlSubscriptOrFieldStart
            = new TTokenCode[]  {
                TTokenCode.LBracket,
                TTokenCode.Period,
                TTokenCode.Dummy
            };

        
        public TTokenCode[] ToDownToList
        {
            get { return tlToDownto; }
        }

        public TTokenCode[] ColonList
        {
            get { return tlColon; }
        }

        protected TTokenCode[] EndList
        {
            get { return tlEnd; }
        }

        protected TTokenCode[] CaseLabelStartList
        {
            get { return tlCaseLabelStart; }
        }

        protected TTokenCode[] OfList
        {
            get { return tlOf; }
        }

        public TTokenCode[] StatementStartList
        {
            get { return tlStatementStart; }
        }

        public TTokenCode[] StatementFollowList
        {
            get { return tlStatementFollow; }
        }

        public TTokenCode[] ColonEqualList
        {
            get { return tlColonEqual; }
        }

        public TTokenCode[] ExpressionStartList
        {
            get { return tlExpressionStart; }
        }

        public TTokenCode[] RelOpsList
        {
            get { return tlRelOps; }
        }

        public TTokenCode[] ExpressionFollowList
        {
            get { return tlExpressionFollow; }
        }

        public TTokenCode[] UnaryOpsList
        {
            get { return tlUnaryOps; }
        }

        public TTokenCode[] AddOpsList
        {
            get { return tlAddOps; }
        }

        public TTokenCode[] MulOpsList
        {
            get { return tlMulOps; }
        }

        public TTokenCode[] DoList
        {
            get { return tlDo; }
        }

        public TTokenCode[] ThenList
        {
            get { return tlThen; }
        }


        protected void GetToken()
        {
            pToken = pScanner.Get();
            token = pToken.Code;
        }

        protected void GetTokenAppend()
        {
            GetToken();
            icode.Put(token);
        }

        protected void InsertLineMarker()
        {
            icode.InsertLineMarker(buffer.LineNumber);
        }

        protected SymtabNode SearchAll(String pString)
        {
            return symtabStack.SearchAll(pString);
        }

        protected SymtabNode SearchLocal(String pString)
        {
            return symtabStack.SearchAll(pString);
        }

        protected SymtabNode EnterLocal(String pString, TDefnCode dc = TDefnCode.dcUndefined)
        {
            return symtabStack.EnterLocal(pString, dc);
        }

        protected SymtabNode EnterNewLocal(String pString, TDefnCode dc = TDefnCode.dcUndefined)
        {
            return symtabStack.EnterNewLocal(pString, dc);
        }

        public StagingParser(TextBuffer buffer, IntermediateCode icode, SymtabStack symbolStack)
        {
            this.buffer = buffer;
			this.icode = icode;
			this.symtabStack = symbolStack;

            pScanner = new TextScanner(this.buffer);

            // -- Enter the special "input" and "output" variable identifiers
            // -- into the symbol table.
            EnterLocal("input");
            EnterLocal("output");
        }

        protected SymtabNode Find(string pString)
        {
            SymtabNode pNode = symtabStack.Find(pString);
            return pNode;
        }

        protected void CopyQuotedString(ref string pString, string pQuotedString)
        {
            int length = pQuotedString.Length - 2;  // don't count quotes
            pString = pString.Substring(1, length);
        }

        public void Parse()
        {
            //// -- Create a dummy program identifier symbol table node.
            //SymtabNode dummyProgramId = new SymtabNode("DummyProgram", TDefnCode.dcProgram);
            //dummyProgramId.defn.routine.locals.pParmIds = null;
            //dummyProgramId.defn.routine.locals.pConstantIds = null;
            //dummyProgramId.defn.routine.locals.pTypeIds = null;
            //dummyProgramId.defn.routine.locals.pVariableIds = null;
            //dummyProgramId.defn.routine.pSymtab = null;
            //dummyProgramId.defn.routine.pIcode = null;

            //// -- Extract the first token and parse the declarations
            //GetToken();
            //ParseDeclarations(dummyProgramId);

            ////--Resynchronize at the final period.
            //Resync(tlProgramEnd);
            //CondGetTokenAppend(TTokenCode.Period, TErrorCode.MissingPeriod);
            GetToken();
            SymtabNode pProgramId = ParseProgram();
        }

        private SymtabNode ParseProgram()
        {
            // --<program-header>
            SymtabNode pProgramId = ParseProgramHeader();

            // --;
            Resync(tlHeaderFollow, tlDeclarationStart, tlStatementStart);
            if (token == TTokenCode.Semicolon) GetToken();
            else if (TokenIn(token, tlDeclarationStart) ||
                TokenIn(token, tlStatementStart))
            {
                OnError(TErrorCode.MissingSemicolon);
            }

            // -- <block>
            ParseBlock(pProgramId);
            pProgramId.defn.routine.pSymtab = symtabStack.ExitScope();

            // --.
            Resync(tlProgramEnd);
            CondGetTokenAppend(TTokenCode.Period, TErrorCode.MissingPeriod);

            return pProgramId;
        }

        private void ParseBlock(SymtabNode pRoutineId)
        {
            //--<declarations>
            ParseDeclarations(pRoutineId);

            // --<compound-statement> : Reset the icode and append BEGIN to it,
            //                          and then Parse the compound statement
            Resync(tlStatementStart);
            if (token != TTokenCode.Begin) OnError(TErrorCode.MissingBEGIN);
            icode.Reset();
            ParseCompound();

            // -- Set the program's or routine's icode.
            pRoutineId.defn.routine.pIcode = new IntermediateCode(icode);
        }

        private SymtabNode ParseProgramHeader()
        {
            SymtabNode pProgramId = null; // ptr to program id node

            // -- PROGRAM
            CondGetToken(TTokenCode.Program, TErrorCode.MissingPROGRAM);

            // --<id>
            if (token == TTokenCode.Identifier)
            {
                pProgramId = EnterNewLocal(pToken.TokenString, TDefnCode.dcProgram);
                pProgramId.defn.routine.which = TRoutineCode.rcDeclared;

                pProgramId.defn.routine.parmCount = 0;
                pProgramId.defn.routine.totalParmSize = 0;
                pProgramId.defn.routine.totalLocalSize = 0;
                pProgramId.defn.routine.locals.pParmIds = null;
                pProgramId.defn.routine.locals.pTypeIds = null;
                pProgramId.defn.routine.locals.pVariableIds = null;
                pProgramId.defn.routine.locals.pRoutineIds = null;
                pProgramId.defn.routine.pSymtab = null;
                pProgramId.defn.routine.pIcode = null;
                TType.SetType(ref pProgramId.pType, TType.pDummyType);
                GetToken();
            }
            else OnError(TErrorCode.MissingIdentifier);

            // -- ( or ;
            Resync(tlProgProcIdFollow, tlDeclarationStart, tlStatementStart);

            // -- Enter the nesting level 1 and open a new scope for the program.
            symtabStack.EnterScope();

            // -- Optional ( <id-list> )
            if (token == TTokenCode.LParen)
            {
                SymtabNode pPrevParmId = null;

                // -- Loop to parse a comma-separated identifier list.
                do
                {
                    GetToken();
                    if (token == TTokenCode.Identifier)
                    {
                        SymtabNode pParmId = EnterNewLocal(pToken.TokenString, TDefnCode.dcVarParm);
                        TType.SetType(ref pParmId.pType, TType.pDummyType);
                        GetToken();

                        // -- Link program parm id nodes together.
                        if (pPrevParmId == null)
                        {
                            pProgramId.defn.routine.locals.pParmIds = pPrevParmId = pParmId;
                        }
                        else
                        {
                            pPrevParmId.Next = pParmId;
                            pPrevParmId = pParmId;
                        }
                    }
                    else OnError(TErrorCode.MissingIdentifier);

                } while (token == TTokenCode.Comma);


                // -- )
                Resync(tlFormalParmsFollow,
                    tlDeclarationStart,
                    tlStatementStart);

                CondGetToken(TTokenCode.RParen, TErrorCode.MissingRightParen);
            }

            return pProgramId;
        }

        private TType ParseSubroutineCall(SymtabNode pRoutineId, bool parmCheckFlag)
        {
            GetTokenAppend();

            return pRoutineId.defn.routine.which == TRoutineCode.rcDeclared || 
                    pRoutineId.defn.routine.which == TRoutineCode.rcForward ||
                    !parmCheckFlag
                    ? ParseDeclaredSubroutineCall(pRoutineId, parmCheckFlag)
                    : ParseStandardSubroutineCall(pRoutineId);
        }

        private TType ParseStandardSubroutineCall(SymtabNode pRoutineId)
        {
            switch (pRoutineId.defn.routine.which)
            {
                case TRoutineCode.rcRead:
                case TRoutineCode.rcReadln:
                    return ParseReadReadLnCall(pRoutineId);

                case TRoutineCode.rcWrite:
                case TRoutineCode.rcWriteln:
                    return parseWriteWritelnCall(pRoutineId);

                case TRoutineCode.rcEoln:
                case TRoutineCode.rcEof:
                    return ParseEofEolnCall();

                case TRoutineCode.rcAbs:
                case TRoutineCode.rcSqr:
                    return ParseAbsSqrCall();

                case TRoutineCode.rcArctan:
                case TRoutineCode.rcCos:
                case TRoutineCode.rcExp:
                case TRoutineCode.rcLn:
                case TRoutineCode.rcSin:
                case TRoutineCode.rcSqrt:
                    return ParseArctanCosExpLnSinSqrtCall();

                case TRoutineCode.rcPred:
                case TRoutineCode.rcSucc:
                    return ParsePredSuccCall();

                case TRoutineCode.rcChr:
                    return ParseChrCall();

                case TRoutineCode.rcOdd:
                    return ParseOddCall();

                case TRoutineCode.rcOrd:
                    return ParseOrdCall();


                case TRoutineCode.rcRound:
                case TRoutineCode.rcTrunc:
                    return ParseRoundTruncCall();

                default:
                    return TType.pDummyType;
            }
        }

        private TType ParseRoundTruncCall()
        {
            //--There should be one real parameter.
            if (token == TTokenCode.LParen)
            {
                GetTokenAppend();

                TType pParmType = ParseExpression().Base;
                if (pParmType != TType.pRealType) OnError(TErrorCode.IncompatibleTypes);

                //--There better not be any more parameters.
                if (token != TTokenCode.RParen) SkipExtraParms();

                //-- )
                CondGetTokenAppend(TTokenCode.RParen, TErrorCode.MissingRightParen);
            }
            else OnError(TErrorCode.WrongNumberOfParms);

            return TType.pIntegerType;
        }

        private TType ParsePredSuccCall()
        {
            TType pResultType = TType.pDummyType;  // ptr to result type object

            //--There should be one integer or enumeration parameter.
            if (token == TTokenCode.LParen)
            {
                GetTokenAppend();

                TType pParmType = ParseExpression().Base;
                if ((pParmType != TType.pIntegerType) &&
                    (pParmType.Form != TFormCode.fcEnum))
                {
                    OnError(TErrorCode.IncompatibleTypes);
                    pResultType = TType.pIntegerType;
                }
                else pResultType = pParmType;

                //--There better not be any more parameters.
                if (token != TTokenCode.RParen) SkipExtraParms();

                //-- )
                CondGetTokenAppend(TTokenCode.RParen, TErrorCode.MissingRightParen);
            }
            else OnError(TErrorCode.WrongNumberOfParms);

            return pResultType;
        }

        private TType ParseOrdCall()
        {
            //--There should be one character or enumeration parameter.
            if (token == TTokenCode.LParen)
            {
                GetTokenAppend();

                TType pParmType = ParseExpression().Base;
                if ((pParmType != TType.pCharType) && (pParmType.Form != TFormCode.fcEnum))
                {
                    OnError(TErrorCode.IncompatibleTypes);
                }

                //--There better not be any more parameters.
                if (token != TTokenCode.RParen) SkipExtraParms();

                //-- )
                CondGetTokenAppend(TTokenCode.RParen, TErrorCode.MissingRightParen);
            }
            else OnError(TErrorCode.WrongNumberOfParms);

            return TType.pIntegerType;
        }

        private TType ParseOddCall()
        {
            //--There should be one integer parameter.
            if (token == TTokenCode.LParen)
            {
                GetTokenAppend();

                TType pParmType = ParseExpression().Base;
                if (pParmType != TType.pIntegerType) OnError(TErrorCode.IncompatibleTypes);

                //--There better not be any more parameters.
                if (token != TTokenCode.RParen) SkipExtraParms();

                //-- )
                CondGetTokenAppend(TTokenCode.RParen, TErrorCode.MissingRightParen);
            }
            else OnError(TErrorCode.WrongNumberOfParms);

            return TType.pBooleanType;
        }

        private TType ParseChrCall()
        {
            //--There should be one character parameter.
            if (token == TTokenCode.LParen)
            {
                GetTokenAppend();

                TType pParmType = ParseExpression().Base;
                if (pParmType != TType.pIntegerType) OnError(TErrorCode.IncompatibleTypes);

                //--There better not be any more parameters.
                if (token != TTokenCode.RParen) SkipExtraParms();

                //-- )
                CondGetTokenAppend(TTokenCode.RParen, TErrorCode.MissingRightParen);
            }
            else OnError(TErrorCode.WrongNumberOfParms);

            return TType.pCharType;
        }

        private TType ParseArctanCosExpLnSinSqrtCall()
        {
            //--There should be one integer or real parameter.
            if (token == TTokenCode.LParen)
            {
                GetTokenAppend();

                TType pParmType = ParseExpression().Base;
                if ((pParmType != TType.pIntegerType) && (pParmType != TType.pRealType))
                {
                    OnError(TErrorCode.IncompatibleTypes);
                }

                //--There better not be any more parameters.
                if (token != TTokenCode.RParen) SkipExtraParms();

                //-- )
                CondGetTokenAppend(TTokenCode.RParen, TErrorCode.MissingRightParen);
            }
            else OnError(TErrorCode.WrongNumberOfParms);

            return TType.pRealType;
        }

        private TType ParseAbsSqrCall()
        {
            TType pResultType = TType.pDummyType;  // ptr to result type object

            //--There should be one integer or real parameter.
            if (token == TTokenCode.LParen)
            {
                GetTokenAppend();

                TType pParmType = ParseExpression().Base;
                if ((pParmType != TType.pIntegerType) && (pParmType != TType.pRealType))
                {
                    OnError(TErrorCode.IncompatibleTypes);
                    pResultType = TType.pIntegerType;
                }
                else pResultType = pParmType;

                //--There better not be any more parameters.
                if (token != TTokenCode.RParen) SkipExtraParms();

                //-- )
                CondGetTokenAppend(TTokenCode.RParen, TErrorCode.MissingRightParen);
            }
            else OnError(TErrorCode.WrongNumberOfParms);

            return pResultType;
        }

        private void SkipExtraParms()
        {
            OnError(TErrorCode.WrongNumberOfParms);

            while (token == TTokenCode.Comma)
            {
                GetTokenAppend();
                ParseExpression();
            }
        }

        private TType ParseEofEolnCall()
        {
            //--There should be no actual parameters, but parse
            //--them anyway for error recovery.
            if (token == TTokenCode.LParen)
            {
                OnError(TErrorCode.WrongNumberOfParms);
                ParseActualParmList(null, false);
            }

            return TType.pBooleanType;
        }

        private TType parseWriteWritelnCall(SymtabNode pRoutineId)
        {
            //--Actual parameters are optional only for writeln.
            if (token != TTokenCode.LParen)
            {
                if (pRoutineId.defn.routine.which == TRoutineCode.rcWrite)
                {
                    OnError(TErrorCode.WrongNumberOfParms);
                }
                return TType.pDummyType;
            }

            //--Loop to parse comma-separated list of actual parameters.
            do
            {
                //-- ( or ,
                GetTokenAppend();

                //--Value <expr> : The type must be either a non-Boolean
                //--               scalar or a string.
                TType pActualType = ParseExpression().Base;
                if (((pActualType.Form != TFormCode.fcScalar) ||
                    (pActualType == TType.pBooleanType))
                    && ((pActualType.Form != TFormCode.fcArray) ||
                    (pActualType.array.pElmtType != TType.pCharType)))
                {
                    OnError(TErrorCode.IncompatibleTypes);
                }

                //--Optional field width <expr>
                if (token == TTokenCode.Colon)
                {
                    GetTokenAppend();
                    if (ParseExpression().Base != TType.pIntegerType)
                    {
                        OnError(TErrorCode.IncompatibleTypes);
                    }

                    //--Optional precision <expr>
                    if (token == TTokenCode.Colon)
                    {
                        GetTokenAppend();
                        if (ParseExpression().Base != TType.pIntegerType)
                        {
                            OnError(TErrorCode.IncompatibleTypes);
                        }
                    }
                }
            } while (token == TTokenCode.Comma);

            //-- )
            CondGetTokenAppend(TTokenCode.RParen, TErrorCode.MissingRightParen);

            return TType.pDummyType;
        }

        private TType ParseReadReadLnCall(SymtabNode pRoutineId)
        {
            //--Actual parameters are optional for readln.
            if (token != TTokenCode.LParen)
            {
                if (pRoutineId.defn.routine.which == TRoutineCode.rcRead)
                {
                    OnError(TErrorCode.WrongNumberOfParms);
                }
                return TType.pDummyType;
            }

            //--Loop to parse comma-separated list of actual parameters.
            do
            {
                //-- ( or ,
                GetTokenAppend();

                //--Each actual parameter must be a scalar variable,
                //--but parse an expression anyway for error recovery.
                if (token == TTokenCode.Identifier)
                {
                    SymtabNode pParmId = Find(pToken.TokenString);
                    icode.Put(pParmId);

                    if (ParseVariable(pParmId).Base.Form
                        != TFormCode.fcScalar) OnError(TErrorCode.IncompatibleTypes);
                }
                else
                {
                    ParseExpression();
                    OnError(TErrorCode.InvalidVarParm);
                }

                //-- , or )
                Resync(tlActualVarParmFollow,
                       tlStatementFollow, tlStatementStart);
            } while (token == TTokenCode.Comma);

            //-- )
            CondGetTokenAppend(TTokenCode.RParen, TErrorCode.MissingRightParen);

            return TType.pDummyType;
        }

        private TType ParseDeclaredSubroutineCall(SymtabNode pRoutineId, bool parmCheckFlag)
        {
            ParseActualParmList(pRoutineId, parmCheckFlag);
            return pRoutineId.pType;
        }

        private void ParseActualParmList(SymtabNode pRoutineId, bool parmCheckFlag)
        {
            SymtabNode pFormalId = pRoutineId != null ? pRoutineId.defn.routine.locals.pParmIds : null;
            // -- if there are no actual parameters, therw better not be
            // -- any formal parameters either.
            if (token != TTokenCode.LParen)
            {
                if (parmCheckFlag && pFormalId != null) OnError(TErrorCode.WrongNumberOfParms);
                return;
            }

            // -- Loop to parse acual parameter expressions
            // -- separated by commas.
            do
            {
                // -- ( or ,
                GetTokenAppend();

                ParseActualParm(pFormalId, parmCheckFlag);
                if (pFormalId != null) pFormalId = pFormalId.Next;
            } while (token == TTokenCode.Comma);

            // -- )
            CondGetTokenAppend(TTokenCode.RParen, TErrorCode.MissingRightParen);

            // There better not be any more formal paraters.
            if (parmCheckFlag && pFormalId != null) OnError(TErrorCode.WrongNumberOfParms);
        }

        private void ParseActualParm(SymtabNode pFormalId, bool parmCheckFlag)
        {
            // -- If we're not checking the acual parameters against
            // -- the corresponding formal parameters (as during error
            // -- recovery), just parse the actual parameter.
            if (!parmCheckFlag)
            {
                ParseExpression();
                return;
            }

            // -- If we've already run out formal parmeter,
            // -- we have and error.  Go into error recovery mode and
            // -- parse the actual parameter anyway
            if (pFormalId == null)
            {
                OnError(TErrorCode.WrongNumberOfParms);
                ParseExpression();
                return;
            }

            // -- Formal value parameter: The acual parameter can be an
            // --                         arbitrary expressoin that is
            // --                         assignment type compatible with
            // --                         the formal parameter.
            if (pFormalId.defn.how == TDefnCode.dcValueParm)
            {
                if( !TType.CheckAssignmentTypeCompatible(pFormalId.pType, ParseExpression()) )
                {
                    OnError(TErrorCode.IncompatibleTypes);
                }

            }

            // -- Formal VAR parameter: The actual parameter must be a
            // --                       variable of the same type as the
            // --                       formal parameter.
            else if (token == TTokenCode.Identifier)
            {
                SymtabNode pActualId = Find(pToken.TokenString);
                icode.Put(pActualId);

                if (pFormalId.pType != ParseVariable(pActualId))
                {
                    OnError(TErrorCode.IncompatibleTypes);
                }

                Resync(tlExpressionFollow, tlStatementFollow, tlStatementStart);
            }
            else
            {
                ParseExpression();
                OnError(TErrorCode.InvalidVarParm);
            }

        }

        private TType ParseVariable(SymtabNode pId)
        {
            TType pResultType = pId.pType;  // ptr to result type

            //--Check how the variable identifier was defined.
            switch (pId.defn.how)
            {
                case TDefnCode.dcVariable:
                case TDefnCode.dcValueParm:
                case TDefnCode.dcVarParm:
                case TDefnCode.dcFunction:
                case TDefnCode.dcUndefined: break;  // OK

                default:
                    pResultType = TType.pDummyType;
                    OnError(TErrorCode.InvalidIdentifierUsage);
                    break;
            }

            GetTokenAppend();

            //-- [ or . : Loop to parse any subscripts and fields.
            bool doneFlag = false;
            do
            {
                switch (token)
                {

                    case TTokenCode.LBracket:
                        pResultType = ParseSubscripts(pResultType);
                        break;

                    case TTokenCode.Period:
                        pResultType = ParseField(pResultType);
                        break;

                    default: 
                        doneFlag = true;
                        break;
                }
            } while (!doneFlag);

            return pResultType;
        }

        private TType ParseSubscripts(TType pType)
        {
            //--Loop to parse a list of subscripts separated by commas.
            do
            {
                //-- [ (first) or , (subsequent)
                GetTokenAppend();

                //-- The current variable is an array type.
                if (pType.Form == TFormCode.fcArray)
                {

                    //--The subscript expression must be assignment type
                    //--compatible with the corresponding subscript type.
                    if (!TType.CheckAssignmentTypeCompatible(pType.array.pIndexType,
                                  ParseExpression()))
                    {
                        OnError(TErrorCode.IncompatibleTypes);
                    }

                    //--Update the variable's type.
                    pType = pType.array.pElmtType;
                }

                //--No longer an array type, so too many subscripts.
                //--Parse the extra subscripts anyway for error recovery.
                else
                {
                    OnError(TErrorCode.TooManySubscripts);
                    ParseExpression();
                }

            } while (token == TTokenCode.Comma);

            //-- ]
            CondGetTokenAppend(TTokenCode.RBracket, TErrorCode.MissingRightBracket);

            return pType;
        }

        private TType ParseField(TType pType)
        {
            GetTokenAppend();

            if ((token == TTokenCode.Identifier) && (pType.Form == TFormCode.fcRecord))
            {
                SymtabNode pFieldId = pType.record.pSymtab.Search(pToken.TokenString);
                if (pFieldId == null) OnError(TErrorCode.InvalidField);
                icode.Put(pFieldId);

                GetTokenAppend();
                return pFieldId != null ? pFieldId.pType : TType.pDummyType;
            }
            else
            {
                OnError(TErrorCode.InvalidField);
                GetTokenAppend();
                return TType.pDummyType;
            }
        }

        private void ParseDeclarations(SymtabNode pRoutineId)
        {
            if (token == TTokenCode.Const)
            {
                GetToken();
                ParseConstantDefinitions(pRoutineId);
            }

            if (token == TTokenCode.Type)
            {
                GetToken();
                ParseTypeDefinitions(pRoutineId);
            }

            if (token == TTokenCode.Var)
            {
                GetToken();
                ParseVariableDeclarations(pRoutineId);
            }

            if (TokenIn(token, tlProcFuncStart))
            {
                ParseSubroutineDeclarations(pRoutineId);
            }
        }

        private void ParseSubroutineDeclarations(SymtabNode pRoutineId)
        {
            SymtabNode pLastId = null;      // ptr to last routine id node
                                            // in local list

            // --Loop to parse procedure and function definitions.
            while (TokenIn(token, tlProcFuncStart))
            {
                SymtabNode pRtnId = ParseSubroutine();

                // -- Link the routine's local (nested) routine id nodes together
                if (pRoutineId.defn.routine.locals.pRoutineIds == null)
                {
                    pRoutineId.defn.routine.locals.pRoutineIds = pRtnId;
                }
                else
                {
                    pLastId.Next = pRtnId;
                }

                pLastId = pRtnId;
            }
        }

        private SymtabNode ParseSubroutine()
        {
            // -- <routine-header>
            SymtabNode pRoutineId = (token == TTokenCode.Procedure) ? ParseProcedureHeader() : ParseFunctionHeader();

            // --;
            Resync(tlHeaderFollow, tlDeclarationStart, tlStatementStart);
            if (token == TTokenCode.Semicolon) GetToken();
            else if (TokenIn(token, tlDeclarationStart) ||
                TokenIn(token, tlStatementStart))
            {
                OnError(TErrorCode.MissingSemicolon);
            }

            // -- <block> or forward
            if (pToken.TokenString != "forward")
            {
                pRoutineId.defn.routine.which = TRoutineCode.rcDeclared;
            }
            else
            {
                GetToken();
                pRoutineId.defn.routine.which = TRoutineCode.rcForward;
            }

            pRoutineId.defn.routine.pSymtab = symtabStack.ExitScope();
            return pRoutineId;
        }

        private SymtabNode ParseProcedureHeader()
        {
            SymtabNode pProcId = null;         // ptr to procedure id node
            bool forwardFlag = false;   // true if forwarded, false if not

            GetToken();

            // --<id> : If the procedure id has already been declared in
            // --       this scope, it must have been a forward declaration.
            if (token == TTokenCode.Identifier)
            {
                pProcId = SearchLocal(pToken.TokenString);
                if (pProcId == null)
                {
                    // -- Not already declared.
                    pProcId = EnterLocal(pToken.TokenString, TDefnCode.dcProdecure);
                    pProcId.defn.routine.totalLocalSize = 0;

                    TType.SetType(ref pProcId.pType, TType.pDummyType);
                }
                else if (pProcId.defn.how == TDefnCode.dcProdecure &&
                          pProcId.defn.routine.which == TRoutineCode.rcForward)
                {
                    // -- Forwarded.
                    forwardFlag = true;
                }
                else OnError(TErrorCode.RedefinedIdentifier);

                GetToken();

                // -- ( or ;
                Resync(tlProgProcIdFollow, tlDeclarationStart, tlStatementStart);

                // --Enter the next nesting level and open a new scope
                // --for the procedure.
                symtabStack.EnterScope();

                // -- Optional (<id-list>) : If therw was a forward delcaration,
                // --                        there must not be a parameter list,
                // --                        but if there is, parse it anyway
                // --                        for error recovery.
                if (token == TTokenCode.LParen)
                {
                    int parmCount;          // count of formal parms
                    int totalParmSize;      // total byte size of all parms
                    SymtabNode pParmList = ParseFormalParmList(out parmCount, out totalParmSize);

                    if (forwardFlag) OnError(TErrorCode.AlreadyForwarded);
                    else
                    {
                        // -- Not forwarded
                        pProcId.defn.routine.parmCount = parmCount;
                        pProcId.defn.routine.totalParmSize = totalParmSize;
                        pProcId.defn.routine.locals.pParmIds = pParmList;
                    }
                }
                else if (!forwardFlag)
                {
                    pProcId.defn.routine.parmCount = 0;
                    pProcId.defn.routine.totalParmSize = 0;
                    pProcId.defn.routine.locals.pParmIds = null;
                }

                pProcId.defn.routine.locals.pConstantIds = null;
                pProcId.defn.routine.locals.pTypeIds = null;
                pProcId.defn.routine.locals.pRoutineIds = null;

                TType.SetType(ref pProcId.pType, TType.pDummyType);
            }
            else
            {
                OnError(TErrorCode.MissingIdentifier);
            }

            return pProcId;
        }

        private SymtabNode ParseFormalParmList(out int count, out int totalSize)
        {
            count = totalSize = 0;

            SymtabNode pParmList = null;            // ptr to list of parm nodes

            SymtabNode pParmId;                     // ptrs to parm symtab nodes;
            SymtabNode pFirstId = null, pLastId = null;
            SymtabNode pPrevSublistLastId = null;
            TDefnCode parmDefn;                     // how a parm is defined

            GetToken();

            // -- Loop to parse a parameter declaraions separated by semicolons.
            while ((token == TTokenCode.Identifier) || (token == TTokenCode.Var))
            {
                TType pParmType;        // ptr to parm's type object

                pFirstId = null;

                // --VAR or value parameter?
                if (token == TTokenCode.Var)
                {
                    parmDefn = TDefnCode.dcVarParm;
                    GetToken();
                }
                else parmDefn = TDefnCode.dcValueParm;

                // -- Loop to parse the comma-separated sublist of parameter ids.
                while (token == TTokenCode.Identifier)
                {
                    pParmId = EnterNewLocal(pToken.TokenString, parmDefn);
                    ++count;
                    if (pParmList == null) pParmList = pParmId;

                    // -- Link the parm id nodes together
                    if (pFirstId == null) pFirstId = pLastId = pParmId;
                    else
                    {
                        pLastId.Next = pParmId;
                        pLastId = pParmId;
                    }

                    // --
                    GetToken();
                    Resync(tlIdentifierFollow);
                    if (token == TTokenCode.Comma)
                    {
                        // -- Saw comma.
                        // -- Skip extra commas and look for an identifier.
                        do
                        {
                            GetToken();
                            Resync(tlIdentifierStart, tlIdentifierFollow);
                            if (token == TTokenCode.Comma)
                            {
                                OnError(TErrorCode.MissingIdentifier);
                            }
                        } while (token == TTokenCode.Comma);

                        if (token != TTokenCode.Identifier)
                        {
                            OnError(TErrorCode.MissingIdentifier);
                        }
                    }
                    else if (token == TTokenCode.Identifier) OnError(TErrorCode.MissingComma);
                }

                // -- :
                Resync(tlSublistFollow, tlDeclarationFollow);
                CondGetToken(TTokenCode.Colon, TErrorCode.MissingColon);

                // --<type-id>
                if (token == TTokenCode.Identifier)
                {
                    SymtabNode pTypeId = Find(pToken.TokenString);
                    if (pTypeId.defn.how != TDefnCode.dcType) OnError(TErrorCode.InvalidType);
                    pParmType = pTypeId.pType;
                    GetToken();

                }
                else
                {
                    OnError(TErrorCode.MissingIdentifier);
                    pParmType = TType.pDummyType;
                }

                // -- Loop to assign the offset and type to each
                // -- parm id in the sublist
                for (pParmId = pFirstId; pParmId != null; pParmId = pParmId.Next)
                {
                    pParmId.defn.dataOffset = totalSize++;
                    TType.SetType(ref pParmId.pType, pParmType);
                }

                // -- Link this sublist to the previous sublist.
                if (pPrevSublistLastId != null) pPrevSublistLastId.Next = pFirstId;
                pPrevSublistLastId = pLastId;

                // -- ; or )
                Resync(tlFormalParmsFollow, tlDeclarationFollow);
                if (token == TTokenCode.Identifier || token == TTokenCode.Var)
                {
                    OnError(TErrorCode.MissingSemicolon);
                }
                else while (token == TTokenCode.Semicolon) GetToken();
            }

            // -- )
            CondGetToken(TTokenCode.RParen, TErrorCode.MissingRightParen);

            return pParmList;
        }

        private SymtabNode ParseFunctionHeader()
        {
            SymtabNode pFuncId = null;         // ptr to procedure id node
            bool forwardFlag = false;   // true if forwarded, false if not

            GetToken();

            // --<id> : If the procedure id has already been declared in
            // --       this scope, it must have been a forward declaration.
            if (token == TTokenCode.Identifier)
            {
                pFuncId = SearchLocal(pToken.TokenString);
                if (pFuncId == null)
                {
                    // -- Not already declared.
                    pFuncId = EnterLocal(pToken.TokenString, TDefnCode.dcFunction);
                    pFuncId.defn.routine.totalLocalSize = 0;

                    TType.SetType(ref pFuncId.pType, TType.pDummyType);
                }
                else if (pFuncId.defn.how == TDefnCode.dcFunction &&
                          pFuncId.defn.routine.which == TRoutineCode.rcForward)
                {
                    // -- Forwarded.
                    forwardFlag = true;
                }
                else OnError(TErrorCode.RedefinedIdentifier);

                GetToken();

                // -- ( or ;
                Resync(tlFuncIdFollow, tlDeclarationStart, tlStatementStart);

                // --Enter the next nesting level and open a new scope
                // --for the procedure.
                symtabStack.EnterScope();

                // -- Optional (<id-list>) : If therw was a forward delcaration,
                // --                        there must not be a parameter list,
                // --                        but if there is, parse it anyway
                // --                        for error recovery.
                if (token == TTokenCode.LParen)
                {
                    int parmCount;          // count of formal parms
                    int totalParmSize;      // total byte size of all parms
                    SymtabNode pParmList = ParseFormalParmList(out parmCount, out totalParmSize);

                    if (forwardFlag) OnError(TErrorCode.AlreadyForwarded);
                    else
                    {
                        // -- Not forwarded
                        pFuncId.defn.routine.parmCount = parmCount;
                        pFuncId.defn.routine.totalParmSize = totalParmSize;
                        pFuncId.defn.routine.locals.pParmIds = pParmList;
                    }
                }
                else if (!forwardFlag)
                {
                    pFuncId.defn.routine.parmCount = 0;
                    pFuncId.defn.routine.totalParmSize = 0;
                    pFuncId.defn.routine.locals.pParmIds = null;
                }

                pFuncId.defn.routine.locals.pConstantIds = null;
                pFuncId.defn.routine.locals.pTypeIds = null;
                pFuncId.defn.routine.locals.pRoutineIds = null;

                // -- Optional <type-id> : If there was a forward declaration,
                // --                      there must not be a type id, but if
                // --                      there is, parse it anyway for error
                // --                      recovery.
                if (!forwardFlag || (token == TTokenCode.Colon))
                {
                    CondGetToken(TTokenCode.Colon, TErrorCode.MissingColon);
                    if (token == TTokenCode.Identifier)
                    {
                        SymtabNode pTypeId = Find(pToken.TokenString);
                        if (pTypeId.defn.how != TDefnCode.dcType) OnError(TErrorCode.InvalidType);

                        if (forwardFlag) OnError(TErrorCode.AlreadyForwarded);
                        else
                        {
                            TType.SetType(ref pFuncId.pType, pTypeId.pType);
                        }

                        GetToken();
                    }
                    else
                    {
                        OnError(TErrorCode.MissingIdentifier);
                    }
                }

                TType.SetType(ref pFuncId.pType, TType.pDummyType);
            }
            else
            {
                OnError(TErrorCode.MissingIdentifier);
            }

            return pFuncId;
        }

        private void ParseVariableDeclarations(SymtabNode pRoutineId)
        {
            ParseVarOrFieldDecls(pRoutineId, null, 0);
        }

        private void ParseVarOrFieldDecls(SymtabNode pRoutineId,
                                            TType pRecordType,
                                            int offset)
        {
            SymtabNode pId, pFirstId, pLastId;          // ptrs to symtab nodes
            SymtabNode pPrevSublistLastId = null;   // ptr to last node of
                                                    // previous sublist

            int totalSize = 0;                      // local variables

            // -- Loop to parse a list of variable or field declarations
            // -- separated by semicolons.
            while (token == TTokenCode.Identifier)
            {
                // -- <id-sublist>
                pFirstId = ParseIdSublist(pRoutineId, pRecordType, out pLastId);

                // -- :
                Resync(tlSublistFollow, tlDeclarationFollow);
                CondGetToken(TTokenCode.Colon, TErrorCode.MissingColon);

                // -- <type>
                TType pType = ParseTypeSpec();

                // -- Now loop to assign the type and offset to each
                // -- identifier in the sublist
                for (pId = pFirstId; pId != null; pId = pId.Next)
                {
                    TType.SetType(ref pId.pType, pType);

                    if (pRoutineId != null)
                    {
                        // -- Variables
                        pId.defn.dataOffset = offset++;
                        totalSize += pType.size;
                    }
                    else
                    {
                        // -- Record fields
                        pId.defn.dataOffset = offset;
                        offset += pType.size;
                    }

                }

                if (pFirstId != null)
                {
                    // -- Set the first sublist into the routine id's symtab node.
                    if (pRoutineId != null &&
                        (pRoutineId.defn.routine.locals.pVariableIds == null))
                    {
                        pRoutineId.defn.routine.locals.pVariableIds = pFirstId;
                    }

                    // -- Link this list to the previous sublist
                    if (pPrevSublistLastId != null) pPrevSublistLastId.Next = pFirstId;
                    pPrevSublistLastId = pLastId;

                }

                // -- ; for variable and record field declaraion, or
                // -- END for field declaration
                if (pRoutineId != null)
                {
                    Resync(tlDeclarationFollow, tlDeclarationStart);
                    CondGetToken(TTokenCode.Semicolon, TErrorCode.MissingSemicolon);

                    // -- Skip extra semicolons
                    while (token == TTokenCode.Semicolon) GetToken();
                    Resync(tlDeclarationFollow, tlDeclarationStart,
                        tlStatementStart);
                }
                else
                {
                    Resync(tlFieldDeclFollow);
                    if (token != TTokenCode.End)
                    {
                        CondGetToken(TTokenCode.Semicolon, TErrorCode.MissingSemicolon);

                        // -- Skip extra semicolons
                        while (token == TTokenCode.Semicolon) GetToken();
                        Resync(tlFieldDeclFollow, tlDeclarationStart, tlStatementStart);
                    }
                }
            }

            // -- Set the routine identifier node or the record type object.
            if (pRoutineId != null)
            {
                pRoutineId.defn.routine.totalLocalSize = totalSize;
            }
            else
            {
                pRecordType.size = offset;
            }
        }

        private TType ParseTypeSpec()
        {
            switch (token)
            {
                case TTokenCode.Identifier:
                    SymtabNode pId = Find(pToken.TokenString);

                    switch(pId.defn.how)
                    {
                        case TDefnCode.dcType: return ParseIdentifierType(pId);
                        case TDefnCode.dcConstant: return ParseSubrangeType(pId);

                        default:
                            OnError(TErrorCode.NotATypeIdentifier);
                            GetToken();
                            return TType.pDummyType;
                    }

                case TTokenCode.LParen: return ParseEnumerationType();
                case TTokenCode.Array: return ParseArrayType();
                case TTokenCode.Record: return ParseRecordType();

                case TTokenCode.Plus:
                case TTokenCode.Minus:
                case TTokenCode.Number:
                case TTokenCode.String:
                    return ParseSubrangeType(null);

                default:
                    OnError(TErrorCode.InvalidType);
                    return TType.pDummyType;
            }
        }

        private TType ParseEnumerationType()
        {
            TType pType = new TType(TFormCode.fcEnum, sizeof(int), null);
            SymtabNode pLastId = null;
            int constValue = -1;

            GetToken();
            Resync(tlEnumConstStart);

            while (token == TTokenCode.Identifier)
            {
                SymtabNode pConstId = EnterNewLocal(pToken.TokenString);
                ++constValue;

                if (pConstId.defn.how == TDefnCode.dcUndefined)
                {
                    pConstId.defn.how = TDefnCode.dcConstant;
                    pConstId.defn.constantValue.integerValue = constValue;

                    TType.SetType(ref pConstId.pType, pType);

                    if (pLastId == null)
                    {
                        pType.Enumeration.ConstIds = pLastId = pConstId;
                    }
                    else
                    {
                        pLastId.Next = pConstId;
                        pLastId = pConstId;
                    }
                }

                GetToken();
                Resync(tlEnumConstFollow);
                if (token == TTokenCode.Comma)
                {
                    do
                    {
                        GetToken();
                        Resync(tlEnumConstStart, tlEnumConstFollow);
                        if (token == TTokenCode.Comma) OnError(TErrorCode.MissingIdentifier);
                    } while (token == TTokenCode.Comma);

                    if (token != TTokenCode.Identifier) OnError(TErrorCode.MissingComma);
                }
                else if (token == TTokenCode.Identifier) OnError(TErrorCode.MissingComma);
            }

            CondGetToken(TTokenCode.RParen, TErrorCode.MissingRightParen);

            pType.Enumeration.max = constValue;
            return pType;
        }

        private TType ParseArrayType()
        {
            TType pArrayType = new TType(TFormCode.fcArray, 0, null);

            TType pElmtType = pArrayType;
            bool indexFlag;

            GetToken();
            CondGetToken(TTokenCode.LBracket, TErrorCode.MissingLeftBracket);

            do
            {
                ParseIndexType(pElmtType);

                Resync(tlIndexFollow, tlIndexStart);
                if (token == TTokenCode.Comma || TokenIn(token, tlIndexStart))
                {
                    pElmtType = TType.SetType(ref pElmtType.array.pElmtType, new TType(TFormCode.fcArray, 0, null));

                    CondGetToken(TTokenCode.Comma, TErrorCode.MissingComma);
                    indexFlag = true;
                }
                else indexFlag = false;

            } while (indexFlag);

            CondGetToken(TTokenCode.RBracket, TErrorCode.MissingRightBracket);

            Resync(tlIndexListFolow, tlDeclarationStart, tlStatementStart);
            CondGetToken(TTokenCode.Of, TErrorCode.MissingOF);

            TType.SetType(ref pElmtType.array.pElmtType, ParseTypeSpec());

            if (pArrayType.Form != TFormCode.fcNone)
            {
                pArrayType.size = ArraySize(pArrayType);
            }

            return pArrayType;
        }

        private void ParseIndexType(TType pArrayType)
        {
            if (TokenIn(token, tlIndexStart))
            {
                TType pIndexType = ParseTypeSpec();
                TType.SetType(ref pArrayType.array.pIndexType, pIndexType);

                switch (pIndexType.Form)
                {
                    case TFormCode.fcSubrange:
                        pArrayType.array.elmtCount =
                            pIndexType.SubRange.max -
                            pIndexType.SubRange.min + 1;
                        pArrayType.array.minIndex = pIndexType.SubRange.min;
                        pArrayType.array.maxIndex = pIndexType.SubRange.max;
                        return;

                    case TFormCode.fcArray:
                        pArrayType.array.elmtCount =
                            pIndexType.Enumeration.max + 1;
                        pArrayType.array.minIndex = 0;
                        pArrayType.array.maxIndex =
                            pIndexType.Enumeration.max;
                        return;

                }

            }

            TType.SetType(ref pArrayType.array.pIndexType, TType.pDummyType);
            pArrayType.array.elmtCount = 0;
            pArrayType.array.minIndex = pArrayType.array.maxIndex = 0;
            OnError(TErrorCode.InvalidIndexType);
        }

        private int ArraySize(TType pArrayType)
        {
            if (pArrayType.array.pElmtType.size == 0)
            {
                pArrayType.array.pElmtType.size =
                    ArraySize(pArrayType.array.pElmtType);
            }

            return (pArrayType.array.elmtCount *
                        pArrayType.array.pElmtType.size);
        }

        private TType ParseRecordType()
        {
            TType pType = new TType(TFormCode.fcRecord, 0, null);

            symtabStack.EnterScope();

            pType.record.pSymtab = symtabStack.CurrentSymtab;

            GetToken();
            ParseFieldDeclarations(pType, 0);

            CondGetToken(TTokenCode.End, TErrorCode.MissingEND);
            return pType;
        }

        private void ParseFieldDeclarations(TType pRecordType, int offset)
        {
            ParseVarOrFieldDecls(null, pRecordType, offset);
        }

        private TType ParseSubrangeType(SymtabNode pMinId)
        {
            TType pType = new TType(TFormCode.fcSubrange, 0, null);

            TType.SetType(ref pType.SubRange.BaseType,
                            ParseSubrangeLimit(pMinId, ref pType.SubRange.min));

            Resync(tlSubrangeLimitFollow, tlDeclarationStart);
            CondGetToken(TTokenCode.DotDot, TErrorCode.MissingDotDot);

            TType pMaxType = ParseSubrangeLimit(null, ref pType.SubRange.max);

            if (pMaxType != pType.SubRange.BaseType)
            {
                OnError(TErrorCode.IncompatibleTypes);
                pType.SubRange.max = pType.SubRange.min;
            }
            else if (pType.SubRange.min > pType.SubRange.max)
            {
                OnError(TErrorCode.MinGtMax);

                int temp = pType.SubRange.min;
                pType.SubRange.min = pType.SubRange.max;
                pType.SubRange.max = temp;
            }

            pType.size = pType.SubRange.BaseType.size;
            return pType;
        }

        private TType ParseSubrangeLimit(SymtabNode pLimitId, ref int limit)
        {
            TType pType = TType.pDummyType;

            TTokenCode sign = TTokenCode.Dummy;

            limit = 0;

            if (TokenIn(token, tlUnaryOps))
            {
                if (token == TTokenCode.Minus) sign = TTokenCode.Minus;
                GetToken();
            }

            switch (token)
            {
                case TTokenCode.Number:
                    if (pToken.DataType == TDataType.tyInteger)
                    {
                        limit = (sign == TTokenCode.Minus) ? -pToken.Value.integerValue
                            : pToken.Value.integerValue;

                        pType = TType.pIntegerType;
                    }
                    else OnError(TErrorCode.InvalidSubrangeType);
                    break;

                case TTokenCode.Identifier:

                    if (pLimitId != null) pLimitId = Find(pToken.TokenString);

                    if (pLimitId.defn.how == TDefnCode.dcUndefined)
                    {
                        pLimitId.defn.how = TDefnCode.dcConstant;
                        TType.SetType(ref pLimitId.pType, TType.pDummyType);
                        pType = TType.pDummyType;
                        break;
                    }
                    else if ((pLimitId.pType == TType.pRealType) ||
                              (pLimitId.pType == TType.pDummyType) ||
                              (pLimitId.pType.Form == TFormCode.fcArray))
                    {
                        OnError(TErrorCode.InvalidSubrangeType);
                    }
                    else if (pLimitId.defn.how == TDefnCode.dcConstant)
                    {
                        if (pLimitId.pType == TType.pIntegerType)
                        {
                            limit = sign == TTokenCode.Minus ? -pLimitId.defn.constantValue.integerValue : pLimitId.defn.constantValue.integerValue;
                        }
                        else if (pLimitId.pType == TType.pCharType)
                        {
                            if (sign != TTokenCode.Dummy) OnError(TErrorCode.InvalidConstant);
                            limit = pLimitId.defn.constantValue.characterValue;
                        }
                        else if (pLimitId.pType.Form == TFormCode.fcEnum)
                        {
                            if (sign != TTokenCode.Dummy) OnError(TErrorCode.InvalidConstant);
                            limit = pLimitId.defn.constantValue.integerValue;
                        }

                        pType = pLimitId.pType;
                    }
                    else OnError(TErrorCode.NotAConstantIdentifier);
                    break;

                case TTokenCode.String:

                    if (sign != TTokenCode.Dummy) OnError(TErrorCode.InvalidConstant);

                    if (pToken.TokenString.Length != 3)
                    {
                        OnError(TErrorCode.InvalidSubrangeType);
                    }

                    limit = pToken.TokenString[1];
                    pType = TType.pCharType;
                    break;

                default:
                    OnError(TErrorCode.MissingConstant);
                    return pType;
            }

            GetHashCode();
            return pType;
        }

        private TType ParseIdentifierType(SymtabNode pId2)
        {
            GetToken();
            return pId2.pType;
        }

        private SymtabNode ParseIdSublist(SymtabNode pRoutineId, TType pRecordType, out SymtabNode pLastId)
        {
            SymtabNode pId;
            SymtabNode pFirstId = null;

            pLastId = null;


            // -- Loop to parse each identifier in the sublist.
            while (token == TTokenCode.Identifier)
            {
                // -- Variable: Enter into local symbol table.
                // -- Field:    Enter into record symbol table.
                pId = pRoutineId != null ? EnterNewLocal(pToken.TokenString)
                    : pRecordType.record.pSymtab.EnterNew(pToken.TokenString);

                // -- Link newly-declared identifier nodes together
                // -- into a sublist.
                if (pId.defn.how == TDefnCode.dcUndefined)
                {
                    pId.defn.how = pRoutineId != null ? TDefnCode.dcVariable : TDefnCode.dcField;

                    if (pFirstId == null) pFirstId = pLastId = pId;
                    else
                    {
                        pLastId.Next = pId;
                        pLastId = pId;
                    }
                }

               // -- ,
                GetToken();
                Resync(tlIdentifierFollow);
                if (token == TTokenCode.Comma)
                {
                    // -- Saw comma.
                    // -- Skip extra commas and look for an identifier
                    do
                    {
                        GetToken();
                        Resync(tlIdentifierStart, tlIdentifierFollow);
                        if (token == TTokenCode.Comma) OnError(TErrorCode.MissingIdentifier);
                    } while (token == TTokenCode.Comma);
                }
            }

            return pFirstId;
        }

        private void ParseTypeDefinitions(SymtabNode pRoutineId)
        {
            SymtabNode pLastId = null;

            while (token == TTokenCode.Identifier)
            {
                SymtabNode pTypeId = EnterNewLocal(pToken.TokenString);

                if (pRoutineId.defn.routine.locals.pTypeIds == null)
                {
                    pRoutineId.defn.routine.locals.pTypeIds = pTypeId;
                }
                else
                {
                    pLastId.Next = pTypeId;
                }

                pLastId = pTypeId;

                GetToken();
                CondGetToken(TTokenCode.Equal, TErrorCode.MissingEqual);

                TType.SetType(ref pTypeId.pType, ParseTypeSpec());
                pTypeId.defn.how = TDefnCode.dcType;

                if (pTypeId.pType.TypeId == null)
                {
                    pTypeId.pType.TypeId = pTypeId;
                }

                Resync(tlDeclarationFollow, tlDeclarationStart,
                    tlStatementStart);
                CondGetToken(TTokenCode.Semicolon, TErrorCode.MissingSemicolon);

                while (token == TTokenCode.Semicolon) GetToken();
                Resync(tlDeclarationFollow, tlDeclarationStart, tlStatementStart);
            }

        }

        private void ParseConstantDefinitions(SymtabNode pRoutineId)
        {
            SymtabNode pLastId = null;      // ptr to last constant id node
                                            // in local list

            // -- Loop to parse a list of constant definitions
            // -- separated by semicolons.
            while (token == TTokenCode.Identifier)
            {   // --<id>
                SymtabNode pConstId = EnterNewLocal(pToken.TokenString);

                // -- Link the routine's local constant id nodes together.
                if (pRoutineId.defn.routine.locals.pConstantIds == null)
                {
                    pRoutineId.defn.routine.locals.pConstantIds = pConstId;
                }
                else
                {
                    pLastId.Next = pConstId;
                }

                pLastId = pConstId;

                // -- =
                GetToken();
                CondGetToken(TTokenCode.Equal, TErrorCode.MissingEqual);

                // --<constant>
                ParseConstant(pConstId);
                pConstId.defn.how = TDefnCode.dcConstant;

                // --;
                Resync(tlDeclarationFollow, tlDeclarationStart, tlStatementStart);
                CondGetToken(TTokenCode.Semicolon, TErrorCode.MissingSemicolon);

                // -- Skip extra semicolons
                while (token == TTokenCode.Semicolon) GetToken();
                Resync(tlDeclarationFollow, tlDeclarationStart, tlStatementStart);
            }
        }

        private void ParseConstant(SymtabNode pConstId)
        {
            TTokenCode sign = TTokenCode.Dummy;     // Unary + or - sign, or none
            
            //-- Unary + or -
            if (TokenIn(token, tlUnaryOps))
            {
                if (token == TTokenCode.Minus) sign = TTokenCode.Minus;
                GetToken();
            }

            switch (token)
            {
                // -- Numeric constant: Integer or real type.
                case TTokenCode.Number:
                    if (pToken.DataType == TDataType.tyInteger)
                    {
                        pConstId.defn.constantValue.integerValue
                            = sign == TTokenCode.Minus ? -pToken.Value.integerValue
                                                        : pToken.Value.integerValue;


                        TType.SetType(ref pConstId.pType, TType.pIntegerType);
                    }
                    else
                    {
                        pConstId.defn.constantValue.realValue =
                            sign == TTokenCode.Minus ? -pToken.Value.realValue
                                                       : pToken.Value.realValue;

                        TType.SetType(ref pConstId.pType, TType.pRealType);
                    }

                    GetToken();
                    break;

                // -- Identifier constant
                case TTokenCode.Identifier:
                    ParseIdentifierConstant(pConstId, sign);
                    break;

                    // -- String constant: Character or string
                    // --                   (character array) type.
                case TTokenCode.String:
                    int length = pToken.TokenString.Length - 2; // skip quotes

                    if (sign != TTokenCode.Dummy) OnError(TErrorCode.InvalidConstant);

                    // -- Single character
                    if (length == 1)
                    {
                        pConstId.defn.constantValue.characterValue = pToken.TokenString[1];
                        TType.SetType(ref pConstId.pType, TType.pCharType);
                    }

                    // -- String (character array): Create a new unnamed
                    // --                           string type.
                    else
                    {
                        string pString = string.Empty;
                        CopyQuotedString(ref pString, pToken.TokenString);
                        pConstId.defn.constantValue.stringValue = pString;
                        TType.SetType(ref pConstId.pType, new TType(length));
                    }

                    GetToken();
                    break;
            }

        }

        private void ParseIdentifierConstant(SymtabNode pId1, TTokenCode sign)
        {
            SymtabNode pId2 = Find(pToken.TokenString);     // ptr to <id-2>

            if (pId2.defn.how != TDefnCode.dcConstant)
            {
                OnError(TErrorCode.NotAConstantIdentifier);
                TType.SetType(ref pId1.pType, TType.pDummyType);
                GetToken();
                return;
            }

            // -- Integer identifier
            if (pId2.pType == TType.pIntegerType)
            {
                pId1.defn.constantValue.integerValue =
                    sign == TTokenCode.Minus ? -pId2.defn.constantValue.integerValue
                    : pId2.defn.constantValue.integerValue;

                TType.SetType(ref pId1.pType, TType.pIntegerType);
            }

            // -- Real identifier
            else if (pId2.pType == TType.pRealType)
            {
                pId1.defn.constantValue.realValue =
                    sign == TTokenCode.Minus ? -pId2.defn.constantValue.realValue
                    : pId2.defn.constantValue.realValue;

                TType.SetType(ref pId1.pType, TType.pRealType);
            }

            // -- Character identifier: No unary sign allowed
            else if (pId2.pType == TType.pCharType)
            {
                if (sign != TTokenCode.Dummy) OnError(TErrorCode.InvalidConstant);

                pId1.defn.constantValue.characterValue =
                    pId2.defn.constantValue.characterValue;

                TType.SetType(ref pId1.pType, TType.pCharType);
            }

            // -- Enumeration identifier: No unary sign allowed.
            else if (pId2.pType.Form == TFormCode.fcEnum)
            {
                if (sign != TTokenCode.Dummy) OnError(TErrorCode.InvalidConstant);

                pId1.defn.constantValue.integerValue =
                    pId2.defn.constantValue.integerValue;

                TType.SetType(ref pId1.pType, pId2.pType);

            }

            // -- Array identifier: Must be character array, and
            //                      no unary sign allowed.
            else if (pId2.pType.Form == TFormCode.fcArray)
            {
                if ((sign != TTokenCode.Dummy) ||
                    (pId2.pType.array.pElmtType != TType.pCharType))
                {
                    OnError(TErrorCode.InvalidConstant);
                }

                pId1.defn.constantValue.stringValue =
                    pId2.defn.constantValue.stringValue;

                TType.SetType(ref pId1.pType, pId2.pType);
            }

            GetToken();
        }

        private void OnError(TErrorCode errorCode)
        {
            buffer.ErrorCount++;

            if (Error != null)
            {
                Error(this, new ErrorEventArgs() { ErrorCode = errorCode, CharacterPosition = buffer.CurrentPosition, CurrentLine = buffer.LineNumber });
            }
        }

        // -- Statements
        protected void ParseStatement()
        {
            InsertLineMarker();

            // -- Call the appropriate parsing functoin based on
            // -- the statement's first token.
            switch (token)
            {
                case TTokenCode.Identifier:
                    //--Search for the identifier and enter it if
                    //--necessary.  Append the symbol table node handle
                    //--to the icode.
                    SymtabNode pNode = Find(pToken.TokenString);
                    icode.Put(pNode);

                    //--Based on how the identifier is defined,
                    //--parse an assignment statement or a procedure call.
                    if (pNode.defn.how == TDefnCode.dcUndefined)
                    {
                        pNode.defn.how = TDefnCode.dcVariable;
                        TType.SetType(ref pNode.pType, TType.pDummyType);
                        ParseAssignment(pNode);
                    }
                    else if (pNode.defn.how == TDefnCode.dcProdecure)
                    {
                        ParseSubroutineCall(pNode, true);
                    }
                    else ParseAssignment(pNode);

                    break;
                
                case TTokenCode.Repeat: ParseRepeat(); break;
                case TTokenCode.While: ParseWhile(); break;
                case TTokenCode.If: ParseIf(); break;
                case TTokenCode.For: ParseFor(); break;
                case TTokenCode.Case: ParseCase(); break;
                case TTokenCode.Begin: ParseCompound(); break;
            }

            if (token != TTokenCode.EndOfFile)
            {
                Resync(tlStatementFollow, tlStatementStart);
            }
        }


        /// <summary>
        /// Parses the statement list until the terminator token
        /// </summary>
        /// <param name="terminator">The terminator.</param>
        private void ParseStatementList(TTokenCode terminator)
        {
            // -- Loop to parse staements and to check for and skip semicolons.
            do
            {
                ParseStatement();

                if (TokenIn(token, tlStatementStart))
                {
                    OnError(TErrorCode.MissingSemicolon);
                }
                else while (token == TTokenCode.Semicolon) GetTokenAppend();

            } while ((token != terminator) && (token != TTokenCode.EndOfFile));

        }

        private void ParseCompound()
        {
            GetTokenAppend();

            // -- <stmt-list>
            ParseStatementList(TTokenCode.End);

            // -- END
            CondGetTokenAppend(TTokenCode.End, TErrorCode.MissingEND);
        }

        private void ParseCase()
        {
            bool caseBranchFlag; // true if another CASE branch, else false

            // -- <expr>
            GetTokenAppend();
            ParseExpression();

            // -- OF
            Resync(tlOf, tlCaseLabelStart);
            CondGetTokenAppend(TTokenCode.Of, TErrorCode.MissingOF);

            // -- Loop to parse CASE branches
            caseBranchFlag = TokenIn(token, tlCaseLabelStart);
            while (caseBranchFlag)
            {
                if (TokenIn(token, tlCaseLabelStart)) ParseCaseBranch();

                if (token == TTokenCode.Semicolon)
                {
                    GetTokenAppend();
                    caseBranchFlag = true;
                }
                else if (TokenIn(token, tlCaseLabelStart))
                {
                    OnError(TErrorCode.MissingSemicolon);
                    caseBranchFlag = true;
                }
                else caseBranchFlag = false;
            }

            // -- END
            Resync(tlEnd, tlStatementStart);
            CondGetTokenAppend(TTokenCode.End, TErrorCode.MissingEND);
        }

        private void ParseCaseBranch()
        {
            bool caseLabelFlag; // true if another CASE label, else false

            // <case-label-list>
            do
            {
                ParseCaseLabel();

                if (token == TTokenCode.Comma)
                {
                    // -- Saw comman, look for another CASE label.
                    GetTokenAppend();
                    if (TokenIn(token, tlCaseLabelStart)) caseLabelFlag = true;
                    else
                    {
                        OnError(TErrorCode.MissingConstant);
                        caseLabelFlag = false;
                    }

                }
                else caseLabelFlag = false;

            } while (caseLabelFlag);

            // -- :
            Resync(tlColon, tlStatementStart);
            CondGetTokenAppend(TTokenCode.Colon, TErrorCode.MissingColon);

            // -- <stmt>
            ParseStatement();
        }

        private void ParseCaseLabel()
        {
            bool signFlag = false; // true if unary sign, else false
            // -- Unary + or -
            if (TokenIn(token, tlUnaryOps))
            {
                signFlag = true;
                GetTokenAppend();
            }

            switch (token)
            {
                // -- Identifier: Must be defined.
                case TTokenCode.Identifier:
                    if (SearchAll(pToken.TokenString) == null)
                    {
                        OnError(TErrorCode.UndefinedIdentifier);
                    }
                    GetTokenAppend();
                    break;

                // -- Numbr: Must be integer.
                case TTokenCode.Number:
                    if (pToken.DataType != TDataType.tyInteger) OnError(TErrorCode.InvalidConstant);
                    GetTokenAppend();
                    break;

                // -- String: Must be a singel character without a unary sign.
                // --       (Note that the string length includes the quotes.)
                case TTokenCode.String:
                    if (signFlag || (pToken.TokenString.Length != 3))
                    {
                        OnError(TErrorCode.InvalidConstant);
                    }
                    GetTokenAppend();
                    break;
            }
        }

        private void ParseFor()
        {
            // -- <id>
            GetTokenAppend();
            if ((token == TTokenCode.Identifier) && (SearchAll(pToken.TokenString) == null))
            {
                OnError(TErrorCode.UndefinedIdentifier);
            }

            CondGetTokenAppend(TTokenCode.Identifier, TErrorCode.MissingIdentifier);

            // -- :=
            Resync(tlColonEqual, tlExpressionStart);
            CondGetTokenAppend(TTokenCode.ColonEqual, TErrorCode.MissingColonEqual);

            // -- <expr-1>
            ParseExpression();

            // -- To or Downto
            Resync(tlToDownto, tlExpressionStart);
            if (TokenIn(token, tlToDownto)) GetTokenAppend();
            else OnError(TErrorCode.MissingTOorDOWNTO);

            // -- <expr-2>
            ParseExpression();

            // -- Do
            Resync(tlDo, tlStatementStart);
            CondGetTokenAppend(TTokenCode.Do, TErrorCode.MissingDO);

            // -- <stmt>
            ParseStatement();
        }

        private void ParseIf()
        {
            // -- <expr>
            GetTokenAppend();
            ParseExpression();

            // -- Then
            Resync(tlThen, tlStatementStart);
            CondGetTokenAppend(TTokenCode.Then, TErrorCode.MissingTHEN);

            // -- <stmt-1>
            ParseStatement();

            if (token == TTokenCode.Else)
            {
                // -- Else <stmt-2>
                GetTokenAppend();
                ParseStatement();
            }
        }

        private void ParseWhile()
        {
            // -- <expr>
            GetTokenAppend();
            ParseExpression();

            // -- DO
            Resync(tlDo, tlStatementStart);
            CondGetTokenAppend(TTokenCode.Do, TErrorCode.MissingDO);

            //--<stmt>
            ParseStatement();
        }

        private void ParseRepeat()
        {
            GetTokenAppend();

            // -- <stmt-list>
            ParseStatementList(TTokenCode.Until);

            // -- Until
            CondGetTokenAppend(TTokenCode.Until, TErrorCode.MissingUNTIL);

            // -- <expr>
            InsertLineMarker();
            ParseExpression();
        }

        protected void ParseAssignment(SymtabNode pTargetId)
        {
            TType pTargetType = ParseVariable(pTargetId);

            //-- :=
            Resync(tlColonEqual, tlExpressionStart);
            CondGetTokenAppend(TTokenCode.ColonEqual, TErrorCode.MissingColonEqual);

            //--<expr>
            TType pExprType = ParseExpression();

            //--Check for assignment compatibility.
            if (!TType.CheckAssignmentTypeCompatible(pTargetType, pExprType))
            {
                OnError(TErrorCode.IncompatibleAssignment);
            }
        }

        // -- Expressions
        protected TType ParseExpression()
        {
            TType pResultType;   // ptr to result type
            TType pOperandType;  // ptr to operand type

            //--Parse the first simple expression.
            pResultType = ParseSimpleExpression();

            //--If we now see a relational operator,
            //--parse the second simple expression.
            if (TokenIn(token, tlRelOps))
            {
                GetTokenAppend();
                pOperandType = ParseSimpleExpression();

                //--Check the operand types and return the boolean type.
                if (!TType.CheckRelOpOperands(pResultType, pOperandType))
                {
                    OnError(TErrorCode.IncompatibleTypes);
                }

                pResultType = TType.pBooleanType;
            }

            //--Make sure the expression ended properly.
            Resync(tlExpressionFollow, tlStatementFollow, tlStatementStart);

            return pResultType;
        }

        private TType ParseSimpleExpression()
        {
            TType pResultType;          // ptr to result type
            TType pOperandType;         // ptr to operand type
            TTokenCode op;                   // operator
            bool unaryOpFlag = false;  // true if unary op, else false

            //--Unary + or -
            if (TokenIn(token, tlUnaryOps))
            {
                unaryOpFlag = true;
                GetTokenAppend();
            }

            //--Parse the first term.
            pResultType = ParseTerm();

            //--If there was a unary sign, check the term's type.
            if (unaryOpFlag && !TType.CheckIntegerOrReal(pResultType))
            {
                OnError(TErrorCode.IncompatibleTypes);
            }

            //--Loop to parse subsequent additive operators and terms.
            while (TokenIn(token, tlAddOps))
            {

                //--Remember the operator and parse the subsequent term.
                op = token;
                GetTokenAppend();
                pOperandType = ParseTerm();

                //--Check the operand types to determine the result type.
                switch (op)
                {

                    case TTokenCode.Plus:
                    case TTokenCode.Minus:

                        //--integer <op> integer => integer
                        if (TType.IntegerOperands(pResultType, pOperandType))
                        {
                            pResultType = TType.pIntegerType;
                        }

                        //--real    <op> real    => real
                        //--real    <op> integer => real
                        //--integer <op> real    => real
                        else if (TType.RealOperands(pResultType, pOperandType))
                        {
                            pResultType = TType.pRealType;
                        }

                        else OnError(TErrorCode.IncompatibleTypes);
                        break;

                    case TTokenCode.Or:

                        //--boolean OR boolean => boolean
                        if (!TType.CheckBoolean(pResultType, pOperandType))
                        {
                            OnError(TErrorCode.IncompatibleTypes);
                        }

                        pResultType = TType.pBooleanType;
                        break;
                }

            }

            return pResultType;
        }

        private TType ParseTerm()
        {
            TType pResultType;   // ptr to result type
            TType pOperandType;  // ptr to operand type
            TTokenCode op;               // operator

            //--Parse the first factor.
            pResultType = ParseFactor();

            //--Loop to parse subsequent multiplicative operators and factors.
            while (TokenIn(token, tlMulOps))
            {

                //--Remember the operator and parse the subsequent factor.
                op = token;
                GetTokenAppend();
                pOperandType = ParseFactor();

                //--Check the operand types to determine the result type.
                switch (op)
                {

                    case TTokenCode.Star:

                        //--integer * integer => integer
                        if (TType.IntegerOperands(pResultType, pOperandType))
                        {
                            pResultType = TType.pIntegerType;
                        }

                        //--real    * real    => real
                        //--real    * integer => real
                        //--integer * real    => real
                        else if (TType.RealOperands(pResultType, pOperandType))
                        {
                            pResultType = TType.pRealType;
                        }

                        else OnError(TErrorCode.IncompatibleTypes);
                        break;

                    case TTokenCode.Slash:

                        //--integer / integer => real
                        //--real    / real    => real
                        //--real    / integer => real
                        //--integer / real    => real
                        if (TType.IntegerOperands(pResultType, pOperandType)
                            || TType.RealOperands(pResultType, pOperandType))
                        {
                            pResultType = TType.pRealType;
                        }
                        else OnError(TErrorCode.IncompatibleTypes);
                        break;

                    case TTokenCode.Div:
                    case TTokenCode.Mod:

                        //--integer <op> integer => integer
                        if (TType.IntegerOperands(pResultType, pOperandType))
                        {
                            pResultType = TType.pIntegerType;
                        }
                        else OnError(TErrorCode.IncompatibleTypes);
                        break;

                    case TTokenCode.And:

                        //--boolean AND boolean => boolean
                        if (!TType.CheckBoolean(pResultType, pOperandType))
                        {
                            OnError(TErrorCode.IncompatibleTypes);
                        }

                        pResultType = TType.pBooleanType;
                        break;
                }
            }

            return pResultType;
        }

        /// <summary>
        /// Parse a factor (identifier, number, 
        /// string, NOT &lt;factor&gt;, or parenthesized
        /// subexpression).
        /// </summary>
        private TType ParseFactor()
        {
            TType pResultType = TType.pDummyType;

            SymtabNode pNode = null;
            switch (token)
            {
                case TTokenCode.Identifier:
                    //--Search for the identifier and enter it if
                    //--necessary.  Append the symbol table node handle
                    //--to the icode.
                    pNode = Find(pToken.TokenString);
                    icode.Put(pNode);

                    if (pNode.defn.how == TDefnCode.dcUndefined)
                    {
                        pNode.defn.how = TDefnCode.dcVariable;
                        TType.SetType(ref pNode.pType, TType.pDummyType);
                    }

                    //--Based on how the identifier is defined,
                    //--parse a constant, function call, or variable.
                    switch (pNode.defn.how)
                    {

                        case TDefnCode.dcFunction:
                            pResultType = ParseSubroutineCall(pNode, true);
                            break;

                        case TDefnCode.dcProdecure:
                            OnError(TErrorCode.InvalidIdentifierUsage);
                            pResultType = ParseSubroutineCall(pNode, false);
                            break;

                        case TDefnCode.dcConstant:
                            GetTokenAppend();
                            pResultType = pNode.pType;
                            break;

                        default:
                            pResultType = ParseVariable(pNode);
                            break;
                    }

                    break;

                case TTokenCode.Number:
                    //--Search for the number and enter it if necessary.
                    pNode = SearchAll(pToken.TokenString);
                    if (pNode == null)
                    {
                        pNode = EnterLocal(pToken.TokenString);

                        //--Determine the number's type, and set its value into
                        //--the symbol table node.
                        if (pToken.DataType == TDataType.tyInteger)
                        {
                            pResultType = TType.pIntegerType;
                            pNode.defn.constantValue.integerValue =
                                        pToken.Value.integerValue;
                        }
                        else
                        {
                            pResultType = TType.pRealType;
                            pNode.defn.constantValue.realValue =
                                        pToken.Value.realValue;
                        }

                        TType.SetType(ref pNode.pType, pResultType);
                    }

                    //--Append the symbol table node handle to the icode.
                    icode.Put(pNode);

                    pResultType = pNode.pType;
                    GetTokenAppend();
                    break;

                case TTokenCode.String:
                    //--Search for the string and enter it if necessary.
                    string pString = pToken.TokenString;
                    pNode = SearchAll(pString);
                    if (pNode == null)
                    {
                        pNode = EnterLocal(pString);
                        pString = pNode.String;

                        //--Compute the string length (without the quotes).
                        //--If the length is 1, the result type is character,
                        //--else create a new string type.
                        int length = pString.Length - 2;
                        pResultType = length == 1 ? TType.pCharType
                                      : new TType(length);
                        TType.SetType(ref pNode.pType, pResultType);

                        //--Set the character value or string pointer into the
                        //--symbol table node.
                        if (length == 1)
                        {
                            pNode.defn.constantValue.characterValue = pString[1];
                        }
                        else
                        {
                            pNode.defn.constantValue.stringValue = pString.Substring(1);
                        }
                    }

                    //--Append the symbol table node handle to the icode.
                    icode.Put(pNode);

                    GetTokenAppend();
                    break;

                case TTokenCode.Not:
                    GetTokenAppend();
                    ParseFactor();
                    pResultType = TType.pBooleanType;
                    break;

                case TTokenCode.LParen:

                    // -- Parenthesized subexpression:  Call ParseExpression
                    // --                               recursively ...
                    GetTokenAppend();
                    pResultType = ParseExpression();
                
                    // -- .. and check for the closing right parenthesis.
                    if (token == TTokenCode.RParen)
                        GetTokenAppend();
                    else
                        OnError(TErrorCode.MissingRightParen);

                    break;

                default:
                    OnError(TErrorCode.InvalidExpression);
                    break;

            }

            return pResultType;
        }

        private bool TokenIn(TTokenCode tc, TTokenCode[] pList)
        {
            if (pList == null || pList.Length == 0) return false; // empty list
            return pList.Contains(tc);
        }

        private void CondGetToken(TTokenCode tc, TErrorCode ec)
        {
            // -- Get another token only if the current one matches tc
            if (tc == token) GetToken();
            else OnError(ec);
        }

        private void CondGetTokenAppend(TTokenCode tc, TErrorCode ec)
        {
            // -- Get another token only if the current one matches tc
            if (tc == token) GetTokenAppend();
            else OnError(ec);
        }


        private void Resync(TTokenCode[] pList1, TTokenCode[] pList2 = null, TTokenCode[] pList3 = null)
        {

            // -- Is the current otken in one of the lists?
            bool errorFlag = (!TokenIn(token, pList1)) &&
                            (!TokenIn(token, pList2)) &&
                            (!TokenIn(token, pList3));

            if (errorFlag)
            {
                // -- Nope.  Flag it as an error.
                TErrorCode errorCode = (token == TTokenCode.EndOfFile) ? TErrorCode.UnexpectedEndOfFile : TErrorCode.UnexpectedToken;
                OnError(errorCode);

                // -- Skip tokens
                while ((!TokenIn(token, pList1)) &&
                        (!TokenIn(token, pList2)) &&
                        (!TokenIn(token, pList3)) &&
                        (token != TTokenCode.Period) &&
                        (token != TTokenCode.EndOfFile))
                {
                    GetToken();
                }

                // -- Flag an unexpected end of file (if hanve't already)
                if ((token == TTokenCode.EndOfFile) &&
                    (errorCode != TErrorCode.UnexpectedEndOfFile))
                {
                    OnError(TErrorCode.UnexpectedEndOfFile);
                }

            }
        }
    }
}
