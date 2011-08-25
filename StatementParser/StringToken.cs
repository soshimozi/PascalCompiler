using System;
using System.Collections.Generic;
using System.Text;

namespace PascalCompiler.StatementParser
{
    class StringToken : TToken
    {
        public StringToken() { }

        override public void Get(IInputBuffer buffer)
        {
            char ch;           // current character
            StringBuilder tokenBuilder = new StringBuilder();

            tokenBuilder.Append('\'');  // opening quote

            //--Get the string.
            ch = buffer.GetChar();  // first char after opening quote
            while (ch != 0x7f)
            {
                if (ch == '\'')
                {     // look for another quote

                    //--Fetched a quote.  Now check for an adjacent quote,
                    //--since two consecutive quotes represent a single
                    //--quote in the string.
                    ch = buffer.GetChar();
                    if (ch != '\'') break;  // not another quote, so previous
                    //   quote ended the string
                }
                //--Replace the end of line character with a blank.
                else if (ch == '\0') ch = ' ';

                //--Append current char to string, then get the next char.
                tokenBuilder.Append(ch);
                ch = buffer.GetChar();
            }

            tokenBuilder.Append('\'');  // closing quote
            tokenString = tokenBuilder.ToString();
            code = TTokenCode.String;
        }

        override public void Print(IOutputBuffer buffer)
        {
            string lineText = string.Format("\t{0, -18} {1}", ">> string:", TokenString);
            buffer.PutLine(lineText);
        }

        override public bool IsDelimiter
        {
            get { return true; }
        }
    }
}
