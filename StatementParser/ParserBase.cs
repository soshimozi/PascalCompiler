using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PascalCompiler.StatementParser
{
    abstract class ParserBase
    {
        private IToken m_currentToken = null;
        private IScanner m_scanner;

        public ParserBase(IScanner scanner)
        {
            m_scanner = scanner;
        }

        private readonly TokenCode[] _statementStartList
            = new TokenCode[] { 
                TokenCode.Begin, 
                TokenCode.Case, 
                TokenCode.For, 
                TokenCode.If, 
                TokenCode.Repeat, 
                TokenCode.While, 
                TokenCode.Identifier,
                TokenCode.Dummy 
            };

        private readonly TokenCode[] _statementFollowList
            = new TokenCode[] {
                TokenCode.Semicolon, 
                TokenCode.Period, 
                TokenCode.End, 
                TokenCode.Else, 
                TokenCode.Until, 
                TokenCode.Dummy
            };

        private readonly TokenCode[] _colonEqualList
            = new TokenCode[] {
                TokenCode.ColonEqual,   
                TokenCode.Dummy
            };

        private readonly TokenCode[] _expressionStartList
            = new TokenCode[] {
                TokenCode.Plus, 
                TokenCode.Minus, 
                TokenCode.Identifier, 
                TokenCode.Number, 
                TokenCode.String,
                TokenCode.Not, 
                TokenCode.LParen, 
                TokenCode.Dummy
            };

        private readonly TokenCode[] _relOpsList
            = new TokenCode[] {
                TokenCode.Equal, 
                TokenCode.Ne, 
                TokenCode.Lt, 
                TokenCode.Gt, 
                TokenCode.Le, 
                TokenCode.Ge, 
                TokenCode.Dummy
            };

        private readonly TokenCode[] _expressionFollowList
            = new TokenCode[] {
                TokenCode.Comma, 
                TokenCode.RParen, 
                TokenCode.RBracket, 
                TokenCode.Colon, 
                TokenCode.Then, 
                TokenCode.To, 
                TokenCode.Downto,
                TokenCode.Do, 
                TokenCode.Of, 
                TokenCode.Dummy
            };

        private readonly TokenCode[] _unaryOpsList
            = new TokenCode[] {
                TokenCode.Plus, 
                TokenCode.Minus, 
                TokenCode.Dummy
            };

        private readonly TokenCode[] _addOpsList
            = new TokenCode[] {
                TokenCode.Plus, 
                TokenCode.Minus, 
                TokenCode.Or, 
                TokenCode.Dummy
            };

        private readonly TokenCode[] _mulOpsList
            = new TokenCode[] {
                TokenCode.Star, 
                TokenCode.Slash, 
                TokenCode.Div, 
                TokenCode.Mod, 
                TokenCode.And, 
                TokenCode.Dummy
            };

        private readonly TokenCode[] _doList
            = new TokenCode[] {
                TokenCode.Do,           
                TokenCode.Dummy
            };

        private readonly TokenCode[] _thenList
            = new TokenCode[] {
                TokenCode.Then,
                TokenCode.Dummy
            };

        private readonly TokenCode[] m_endList
            = new TokenCode[] {
                TokenCode.End,          
                TokenCode.Dummy
            };

        private readonly TokenCode[] m_caseLabelStartList
            = new TokenCode[] {
                TokenCode.Identifier, 
                TokenCode.Number, 
                TokenCode.Plus, 
                TokenCode.Minus, 
                TokenCode.String, 
                TokenCode.Dummy
            };

        private readonly TokenCode[] m_ofList
            = new TokenCode[] {
                TokenCode.Of,           
                TokenCode.Dummy
            };

        private readonly TokenCode[] m_colonList
            = new TokenCode[] {
                TokenCode.Colon,        
                TokenCode.Dummy
            };

        private readonly TokenCode[] m_toDowntoList
            = new TokenCode[] {
                TokenCode.To,        
                TokenCode.Downto,
                TokenCode.Dummy
            };

        public TokenCode[] ToDownToList
        {
            get { return m_toDowntoList; }
        }

        public TokenCode[] ColonList
        {
            get { return m_colonList; }
        }
        
        protected TokenCode[] EndList 
        { 
            get { return m_endList; } 
        }

        protected TokenCode[] CaseLabelStartList 
        { 
            get { return m_caseLabelStartList; } 
        }

        protected TokenCode[] OfList 
        { 
            get { return m_ofList; } 
        }

        public TokenCode[] StatementStartList
        {
            get { return _statementStartList; }
        }

        public TokenCode[] StatementFollowList
        {
            get { return _statementFollowList; }
        }

        public TokenCode[] ColonEqualList
        {
            get { return _colonEqualList; }
        }

        public TokenCode[] ExpressionStartList
        {
            get { return _expressionStartList; }
        }
        
        public TokenCode[] RelOpsList
        {
            get { return _relOpsList; }
        } 

        public TokenCode[] ExpressionFollowList
        {
            get { return _expressionFollowList; }
        }
        
        public TokenCode[] UnaryOpsList
        {
            get { return _unaryOpsList; }
        }

        public TokenCode[] AddOpsList
        {
            get { return _addOpsList; }
        }
        
        public TokenCode[] MulOpsList
        {
            get { return _mulOpsList; }
        }
        
        public TokenCode[] DoList
        {
            get { return _doList; }
        }
        
        public TokenCode[] ThenList
        {
            get { return _thenList; }
        }

        abstract public void Parse();

        protected void GetToken()
        {
            m_currentToken = m_scanner.Get();
        }

        protected void GetTokenAppend()
        {
            GetToken();
            IntermediateCode.Put(CurrentCode);
        }

        protected DataType CurrentDataType
        {
            get
            {
                if (m_currentToken != null)
                {
                    return m_currentToken.DataType;
                }
                else
                {
                    return null;
                }
            }
        }

        protected DataValue CurrentTokenValue
        {
            get
            {
                if (m_currentToken != null)
                {
                    return m_currentToken.TokenValue;
                }
                else
                {
                    return null;
                }
            }
        }

        protected string CurrentTokenString
        {
            get
            {
                if (m_currentToken != null)
                {
                    return m_currentToken.TokenString;
                }
                else
                {
                    return null;
                }
            }
        }

        protected TokenCode CurrentCode
        {
            get
            {
                if (m_currentToken != null)
                {
                    return m_currentToken.Code;
                }
                else
                {
                    return TokenCode.Dummy;
                }

            }
        }

        protected void CondGetTokenAppend(TokenCode tokenCode, ErrorCode errorCode)
        {
            //--Get another token only if the current one matches tokenCode.
            if (CurrentCode == tokenCode) GetTokenAppend();
            else Error(errorCode);  // error if no match
        }

        protected IntermediateCode IntermediateCode
        {
            get
            {
                return BackendManager.Instance.IntermediateCode;
            }
        }

        protected Symtab GlobalSymbolTable
        {
            get
            {
                if (SymbolTableManager.Instance.TableCount <= 0)
                {
                    // global symbol table is always 0
                    SymbolTableManager.Instance.CreateTable();
                }

                return SymbolTableManager.Instance.GetSymbolTable(0);
            }
        }

        protected SymtabNode EnterLocal(string tokenString)
        {
            return GlobalSymbolTable.Enter(tokenString);
        }

        protected SymtabNode SearchAll(string tokenString)
        {
            return GlobalSymbolTable.Search(tokenString);
        }

        protected void Error(ErrorCode errorCode)
        {
            // TODO: Handle errors somehow
        }

        protected bool TokenIn(TokenCode tokenCode, TokenCode[] tokenList)
        {
            if (tokenList != null)
            {
                foreach (TokenCode code in tokenList)
                {
                    if (tokenCode == code)
                        return true;
                }
            }

            return false;
        }

    }
}
