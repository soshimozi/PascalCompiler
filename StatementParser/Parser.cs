using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PascalCompiler.StatementParser
{
    class Parser : ParserBase
    {

        public Parser(IScanner scanner) : base(scanner)
        {
        }

        override public void Parse()
        {
            // always start with a statement
            ParseStatement();
        }

        /// <summary>
        /// Parse a statement
        /// </summary>
        private void ParseStatement()
        {
            GetTokenAppend();

            InsertLineMarker();

            switch (CurrentCode)
            {
                case TokenCode.Identifier: ParseAssignment(); break;
                case TokenCode.Repeat: ParseRepeat(); break;
                case TokenCode.While: ParseWhile(); break;
                case TokenCode.If: ParseIf(); break;
                case TokenCode.For: ParseFor(); break;
                case TokenCode.Case: ParseCase(); break;
                case TokenCode.Begin: ParseCompound(); break;
            }

            //--Resynchronize at a proper statement ending.
            if (CurrentCode != TokenCode.EndOfFile)
            {
                Resync(
                    new List<TokenCode[]> {
                        StatementFollowList, 
                        StatementStartList
                    });
            }
        }

        /// <summary>
        /// Resynchronize the parser.  If the current
        /// token is not in one of the token lists,
        /// flag it as an error and then skip tokens
        /// up to one that is in a list or end of file.
        /// </summary>
        /// <param name="tokenList1">The token list1.</param>
        /// <param name="tokenList2">The token list2.</param>
        /// <param name="tokenList3">The token list3.</param>
        private void Resync(List<TokenCode[]> listOfLists)
        {
            bool found = false;

            //--Is the current token in one of the lists?
            for (int i = 0; i < listOfLists.Count && !found; i++)
            {
                found = TokenIn(CurrentCode, listOfLists[i]);
            }


            if (!found)
            {

                //--Nope.  Flag it as an error.
                ErrorCode errorCode = CurrentCode == TokenCode.EndOfFile
                                ? ErrorCode.UnexpectedEndOfFile
                                : ErrorCode.UnexpectedToken;
                Error(errorCode);

                //--Skip tokens.

                bool inAnyList = false;
                while (!inAnyList)
                {
                    for (int i = 0; i < listOfLists.Count &&
                                    !inAnyList &&
                                    CurrentCode != TokenCode.Packed &&
                                    CurrentCode != TokenCode.End; i++)
                    {
                        inAnyList = TokenIn(CurrentCode, listOfLists[i]);
                    }

                    // didn't find it yet so get the next token
                    if (!inAnyList)
                    {
                        GetToken();
                    }
                }

                //--Flag an unexpected end of file (if haven't already).
                if ((CurrentCode == TokenCode.EndOfFile) &&
                    (errorCode != ErrorCode.UnexpectedEndOfFile))
                {
                    Error(ErrorCode.UnexpectedEndOfFile);
                }

            }
        }

        /// <summary>
        ///  ParseCompound       Parse a compound statement:
        ///
        ///                          BEGIN <stmt-list> END
        /// </summary>
        private void ParseCompound()
        {
            GetTokenAppend();

            //--<stmt-list>
            ParseStatementList(TokenCode.End);

            //--END
            CondGetTokenAppend(TokenCode.End, ErrorCode.MissingEnd);
        }

        private void ParseCase()
        {
            bool caseBranchFlag;  // true if another CASE branch, else false

            //--<expr>
            GetTokenAppend();
            ParseExpression();

            //--OF
            Resync(new List<TokenCode[]>() {OfList, CaseLabelStartList});
            CondGetTokenAppend(TokenCode.Of, ErrorCode.MissingOf);

            //--Loop to parse CASE branches.
            caseBranchFlag = TokenIn(CurrentCode, CaseLabelStartList);
            while (caseBranchFlag)
            {
                if (TokenIn(CurrentCode, CaseLabelStartList)) ParseCaseBranch();

                if (CurrentCode== TokenCode.Semicolon)
                {
                    GetTokenAppend();
                    caseBranchFlag = true;
                }
                else if (TokenIn(CurrentCode, CaseLabelStartList))
                {
                    Error(ErrorCode.MissingSemicolon);
                    caseBranchFlag = true;
                }
                else caseBranchFlag = false;
            }

            //--END
            Resync(new List<TokenCode[]>() {EndList, StatementStartList});
            CondGetTokenAppend(TokenCode.End, ErrorCode.MissingEnd);
        }

        private void ParseCaseBranch()
        {
            bool caseLabelFlag;  // true if another CASE label, else false

            //--<case-label-list>
            do
            {
                ParseCaseLabel();
                if (CurrentCode == TokenCode.Comma)
                {

                    //--Saw comma, look for another CASE label.
                    GetTokenAppend();
                    if (TokenIn(CurrentCode, CaseLabelStartList)) caseLabelFlag = true;
                    else
                    {
                        Error(ErrorCode.MissingConstant);
                        caseLabelFlag = false;
                    }
                }
                else caseLabelFlag = false;

            } while (caseLabelFlag);

            //-- :
            Resync(new List<TokenCode[]>() {ColonList, StatementStartList});
            CondGetTokenAppend(TokenCode.Colon, ErrorCode.MissingColon);

            //--<stmt>
            ParseStatement();
        }

        private void ParseCaseLabel()
        {
            bool signFlag = false;  // true if unary sign, else false

            //--Unary + or -
            if (TokenIn(CurrentCode, UnaryOpsList))
            {
                signFlag = true;
                GetTokenAppend();
            }

            switch (CurrentCode)
            {
                //--Identifier:  Must be defined.
                case TokenCode.Identifier:
                    if (SearchAll(CurrentTokenString) == null)
                    {
                        Error(ErrorCode.UndefinedIdentifier);
                    }
                    GetTokenAppend();
                    break;

                //--Number:  Must be integer.
                case TokenCode.Number:
                    if (
                        CurrentDataType.Code != TypeCode.Int16 &&
                        CurrentDataType.Code != TypeCode.Int32 &&
                        CurrentDataType.Code != TypeCode.Int64 &&
                        CurrentDataType.Code != TypeCode.UInt16 &&
                        CurrentDataType.Code != TypeCode.UInt32 &&
                        CurrentDataType.Code != TypeCode.UInt64
                        ) Error(ErrorCode.InvalidConstant);

                    GetTokenAppend();
                    break;

                //--String:  Must be a single character without a unary sign.
                //--         (Note that the string length includes the quotes.)
                case TokenCode.String:
                    if (signFlag || (CurrentTokenString.Length != 3))
                    {
                        Error(ErrorCode.InvalidConstant);
                    }
                    GetTokenAppend();
                    break;
            }
        }

        private void ParseFor()
        {
            //--<id>
            GetTokenAppend();
            if ((CurrentCode == TokenCode.Identifier) && (SearchAll(CurrentTokenString) == null))
            {
                Error(ErrorCode.UndefinedIdentifier);
            }
            CondGetTokenAppend(TokenCode.Identifier, ErrorCode.MissingIdentifier);

            //-- :=
            Resync( new List<TokenCode[]>() { ColonEqualList, ExpressionStartList } );
            CondGetTokenAppend(TokenCode.ColonEqual, ErrorCode.MissingColonEqual);

            //--<expr-1>
            ParseExpression();

            //--TO or DOWNTO
            Resync( new List<TokenCode[]>() { ToDownToList, ExpressionStartList });
            if (TokenIn(CurrentCode, ToDownToList)) GetTokenAppend();
            else Error(ErrorCode.MissingToOrDownto);

            //--<expr-2>
            ParseExpression();

            //--DO
            Resync(new List<TokenCode[]>() {DoList, StatementStartList });
            CondGetTokenAppend(TokenCode.Do, ErrorCode.MissingDo);

            //--<stmt>
            ParseStatement();
        }

        private void ParseIf()
        {
            //--<expr>
            GetTokenAppend();
            ParseExpression();

            //--THEN
            Resync(
                new List<TokenCode[]>() {
                    ThenList, 
                    StatementStartList
                });

            CondGetTokenAppend(TokenCode.Then, ErrorCode.MissingThen);

            //--<stmt-1>
            ParseStatement();

            if (CurrentCode == TokenCode.Else)
            {

                //--ELSE <stmt-2>
                GetTokenAppend();
                ParseStatement();
            }
        }

        private void ParseWhile()
        {
            //--<expr>
            GetTokenAppend();
            ParseExpression();

            //--DO
            Resync(
                new List<TokenCode[]> {
                    DoList, 
                    StatementStartList
                });

            CondGetTokenAppend(TokenCode.Do, ErrorCode.MissingDo);

            //--<stmt>
            ParseStatement();
        }

        private void ParseRepeat()
        {
            GetTokenAppend();

            //--<stmt-list>
            ParseStatementList(TokenCode.Until);

            //--UNTIL
            CondGetTokenAppend(TokenCode.Until, ErrorCode.MissingUntil);

            //--<expr>
            InsertLineMarker();
            ParseExpression();
        }

        /// <summary>
        /// Parse a statement list until the
        /// terminator token.
        /// </summary>
        /// <param name="terminator">The terminator.</param>
        private void ParseStatementList(TokenCode terminator)
        {
            //--Loop to parse statements and to check for and skip semicolons.
            do
            {
                ParseStatement();

                if (TokenIn(CurrentCode, StatementStartList))
                {
                    Error(ErrorCode.MissingSemicolon);
                }
                else while (CurrentCode == TokenCode.Semicolon) GetTokenAppend();
            } while ((CurrentCode != terminator) && (CurrentCode != TokenCode.EndOfFile));
        }

        private void InsertLineMarker()
        {
            IntermediateCode.InsertLineMarker();
        }

        /// <summary>
        ///  ParseAssignment         Parse an assignment statement:
        ///
        ///                              [id] := [expr]        
        /// </summary>
        private void ParseAssignment()
        {
            //--Search for the target variable's identifier and enter it
            //--if necessary.  Append the symbol table node handle
            //--to the icode.
            SymtabNode targetNode = SearchAll(CurrentTokenString);
            if (targetNode == null) targetNode = EnterLocal(CurrentTokenString);

            IntermediateCode.Put(targetNode);
            GetTokenAppend();

            //-- :=
            Resync(
                new List<TokenCode[]> {
                    ColonEqualList, 
                    ExpressionStartList
                });

            CondGetTokenAppend(TokenCode.ColonEqual, ErrorCode.MissingColonEqual);

            //--<expr>
            ParseExpression();
        }

        /// <summary>
        ///  ParseExpression     Parse an expression (binary relational
        ///                      operators = < > <> <= and >= ).        
        /// </summary>
        private void ParseExpression()
        {
            //--Parse the first simple expression.
            ParseSimpleExpression();

            //--If we now see a relational operator,
            //--parse the second simple expression.
            if (TokenIn(CurrentCode, RelOpsList))
            {
                GetTokenAppend();
                ParseSimpleExpression();
            }

            //--Make sure the expression ended properly.
            Resync(
                new List<TokenCode[]> {
                    ExpressionFollowList, 
                    StatementFollowList, 
                    StatementStartList
                });
        }

        private void ParseSimpleExpression()
        {
            //--Unary + or -
            if (TokenIn(CurrentCode, UnaryOpsList)) GetTokenAppend();

            //--Parse the first term.
            ParseTerm();

            //--Loop to parse subsequent additive operators and terms.
            while (TokenIn(CurrentCode, AddOpsList))
            {
                GetTokenAppend();
                ParseTerm();
            }
        }

        private void ParseTerm()
        {
            //--Parse the first factor.
            ParseFactor();

            //--Loop to parse subsequent multiplicative operators and factors.
            while (TokenIn(CurrentCode, MulOpsList))
            {
                GetTokenAppend();
                ParseFactor();
            }
        }

        private void ParseFactor()
        {
            switch (CurrentCode)
            {

                case TokenCode.Identifier:
                    {

                        //--Search for the identifier.  If found, append the
                        //--symbol table node handle to the icode.  If not
                        //--found, enter it and flag an undefined identifier error.
                        SymtabNode node = SearchAll(CurrentTokenString);
                        if (node != null) IntermediateCode.Put(node);
                        else
                        {
                            Error(ErrorCode.UndefinedIdentifier);
                            EnterLocal(CurrentTokenString);
                        }

                        GetTokenAppend();
                        break;
                    }

                case TokenCode.Number:
                    {

                        //--Search for the number and enter it if necessary.
                        //--Set the number's value in the symbol table node.
                        //--Append the symbol table node handle to the icode.
                        SymtabNode node = SearchAll(CurrentTokenString);
                        if (node == null)
                        {
                            node = EnterLocal(CurrentTokenString);

                            // if (node == null) throw an exception?

                            TypeCode tc = TypeCode.Empty;

                            if (node != null && CurrentDataType != null && CurrentTokenValue != null)
                            {
                                tc = CurrentDataType.Code;


                                //node.Value = tc == TypeCode.Int16 ||
                                //               tc == TypeCode.Int32 ||
                                //               tc == TypeCode.Int64 ||
                                //               tc == TypeCode.UInt16 ||
                                //               tc == TypeCode.UInt32 ||
                                //               tc == TypeCode.UInt64
                                //                ? (float)CurrentTokenValue.IntegerValue
                                //                : (float)CurrentTokenValue.RealValue;
                            }
                        }

                        if( node != null ) IntermediateCode.Put(node);

                        GetTokenAppend();
                        break;
                    }

                case TokenCode.String:
                    GetTokenAppend();
                    break;

                case TokenCode.Not:
                    GetTokenAppend();
                    ParseFactor();
                    break;

                case TokenCode.LParen:

                    //--Parenthesized subexpression:  Call ParseExpression
                    //--                              recursively ...
                    GetTokenAppend();
                    ParseExpression();

                    //-- ... and check for the closing right parenthesis.
                    if (CurrentCode == TokenCode.RParen) GetTokenAppend();
                    else Error(ErrorCode.MissingRightParen);

                    break;

                default:
                    Error(ErrorCode.InvalidExpression);
                    break;
            }
        }
    }
}
