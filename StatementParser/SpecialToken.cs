using System;
using System.Collections.Generic;
using System.Text;

namespace PascalCompiler.StatementParser
{
    class SpecialToken : TToken
    {
        public SpecialToken() { }

        override public void Get(IInputBuffer buffer)
        {
            char ch = buffer.CurrentChar;
            StringBuilder tokenBuilder = new StringBuilder();

            tokenBuilder.Append(ch);

            switch (ch)
            {
                case '^': code = TTokenCode.UpArrow; buffer.GetChar(); break;
                case '*': code = TTokenCode.Star; buffer.GetChar(); break;
                case '(': code = TTokenCode.LParen; buffer.GetChar(); break;
                case ')': code = TTokenCode.RParen; buffer.GetChar(); break;
                case '-': code = TTokenCode.Minus; buffer.GetChar(); break;
                case '+': code = TTokenCode.Plus; buffer.GetChar(); break;
                case '=': code = TTokenCode.Equal; buffer.GetChar(); break;
                case '[': code = TTokenCode.LBracket; buffer.GetChar(); break;
                case ']': code = TTokenCode.RBracket; buffer.GetChar(); break;
                case ';': code = TTokenCode.Semicolon; buffer.GetChar(); break;
                case ',': code = TTokenCode.Comma; buffer.GetChar(); break;
                case '/': code = TTokenCode.Slash; buffer.GetChar(); break;
                case ':':   // : or :=
                    ch = buffer.GetChar();
                    if (ch == '=')
                    {
                        tokenBuilder.Append(ch);
                        code = TTokenCode.ColonEqual;
                        buffer.GetChar();
                    }
                    else
                    {
                        code = TTokenCode.Colon;
                    }
                    break;

                case '<': 

                    ch = buffer.GetChar();     // < or <= or <>
                    if (ch == '=')
                    {
                        tokenBuilder.Append('=');
                        code = TTokenCode.Le;
                        buffer.GetChar();
                    }
                    else if (ch == '>')
                    {
                        tokenBuilder.Append(ch);
                        code = TTokenCode.Ne;
                        buffer.GetChar();
                    }
                    else
                    {
                        code = TTokenCode.Lt;
                    }
                    break;

                case '>':
                    ch = buffer.GetChar();     // > or >=
                    if (ch == '=')
                    {
                        tokenBuilder.Append('=');
                        code = TTokenCode.Ge;
                        buffer.GetChar();
                    }
                    else
                    {
                        code = TTokenCode.Gt;
                    }
                    break;

                case '.':
                    ch = buffer.GetChar(); // . or ..
                    if (ch == '.')
                    {
                        tokenBuilder.Append(ch);
                        code = TTokenCode.DotDot;
                        buffer.GetChar();
                    }
                    else
                    {
                        code = TTokenCode.Period;
                    }

                    break;

                default:
                    code = TTokenCode.Error;                  // error
                    buffer.GetChar();
                    break;
            }

            tokenString = tokenBuilder.ToString();
        }

        public override bool IsDelimiter
        {
            get { return true; }
        }

        public override void Print(IOutputBuffer buffer)
        {
            string lineText = string.Format("\t{0, -18} {1}", ">> special:", TokenString);
            buffer.PutLine(lineText);
        }

    }
}
