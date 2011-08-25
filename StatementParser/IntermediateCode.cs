using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PascalCompiler.StatementParser
{
    public class IntermediateCode : IScanner
    {
        private readonly string [] symbolStrings = new string[] {
            null,
            null, null, null, null, null,
            "^", "*", "(", ")", "-", "+",
            "=", "[", "]", ":", ";", "<",
            ">", ",", ".", "/", ":=", "<=", ">=",
            "<>", "..",
            "and", "array", "begin", "case", "const", "div",
            "do", "downto", "else", "end", "file", "for", "function",
            "goto", "if", "in", "label", "mod", "nil", "not", "of", "or",
            "packed", "procedure", "program", "record", "repeat", "set",
            "then", "to", "type", "until", "var", "while", "with" };

        private int m_errorCount = 0;
        private int m_currentLineNumber = 0;
        private SymtabNode pNode = null;

        private BinaryFormatter m_formatter;

        public IntermediateCode(IntermediateCode icode = null)
        {
            if (icode == null)
            {
                m_formatter = new BinaryFormatter();

            } else
            {
                m_formatter = new BinaryFormatter(icode.m_formatter.Stream);
            }
        }

        public void GoTo(int location)
        {
            m_formatter.GoTo(location);
        }

        public void Reset()
        {
            m_formatter.GoTo(0);
        }

        public int CurrentLocation
        {
            get
            {
                return m_formatter.CurrentLocation;
            }
        }

        public int CurrentLineNumber
        {
            get
            {
                return m_currentLineNumber;
            }
        }

        public void Put(int intValue )
        {
            m_formatter.Write(intValue);
        }

        public void Put(SymtabNode node)
        {
            m_formatter.Write(node.SymtabIndex);
            m_formatter.Write(node.NodeIndex);
        }

        public void Put(TTokenCode tokenCode)
        {
            m_formatter.Write((int)tokenCode);
        }
        
        /// <summary>
        ///  Insert a line marker into the
        ///  intermediate code just before the
        ///  last appended token code.
        /// </summary>
        public void InsertLineMarker(int lineNumber)
        {
            if (m_errorCount > 0) return;

            int lastCode;
            m_formatter.Rewind(sizeof(int));
            lastCode = m_formatter.ReadInt32();
            m_formatter.Rewind(sizeof(int));

            //--Insert a line marker code
            //--followed by the current line number.
            Put(TTokenCode.LineMarker);
            Put(lineNumber);

            //--Re-append the last token code;
            Put((TTokenCode)lastCode);
        }


        #region IScanner Members

        public TToken Get()
        {
            TToken pToken = null;
            TTokenCode token;
            int code;

            //--Loop to process any line markers
            //--and extract the next token code.
            do
            {
                //--First read the token code.
                code = m_formatter.ReadInt32();
                token = (TTokenCode)code;

                if (token == TTokenCode.LineMarker)
                {
                    int number = m_formatter.ReadInt32();
                    m_currentLineNumber = number;
                }

            } while (token == TTokenCode.LineMarker);


            //--Determine the token class, based on the token code.
            switch (token)
            {
                case TTokenCode.Number: pToken = new NumberToken(); break;
                case TTokenCode.String: pToken = new StringToken(); break;

                case TTokenCode.Identifier:
                    pToken = new WordToken();
                    pToken.Code = TTokenCode.Identifier;
                    break;

                default:
                    if (code < (int)TTokenCode.And)
                    {
                        pToken = new SpecialToken();
                        pToken.Code = token;
                    }
                    else
                    {
                        pToken = new WordToken(); // reserved word
                        pToken.Code = token;
                    }
                    break;
            }

            // now get node
            //--Extract the symbol table node and set the token string.
            switch (token)
            {
                case TTokenCode.Identifier:
                case TTokenCode.Number:
                case TTokenCode.String:
                    pNode = GetNode();
                    pToken.TokenString = pNode.String;
                    break;

                default:
                    pNode = null;
                    pToken.TokenString = symbolStrings[code];
                    break;
            }

            return pToken;
        }

        #endregion

        public SymtabNode SymtabNode
        {
            get { return pNode; }
        }

        private SymtabNode GetNode()
        {
            int tableIndex = m_formatter.ReadInt32();
            int nodeIndex = m_formatter.ReadInt32();

            //return SharedProperties.pSymtabList
            int i = 0, count = tableIndex;

            Symtab table = SharedProperties.pSymtabList;

            while (i < tableIndex && table != null)
            {
                table = table.next;
                tableIndex++;
            }

            SymtabNode node = null;
            if( table != null )
            {
                SymtabNode [] nodes = table.Values;

                var query = from nd in nodes
                            where nd.NodeIndex.Equals(nodeIndex)
                            select nd;

                node = query.FirstOrDefault();
            }

            return node;
        }
    }
}
