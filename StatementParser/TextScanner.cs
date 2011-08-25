using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PascalCompiler.StatementParser;

namespace PascalCompiler
{
    class TextScanner : IScanner
    {
        private TextBuffer buffer;
        public TextScanner(TextBuffer buffer)
        {
            this.buffer = buffer;
        }

        private void skipWhiteSpace()
        {
            char ch = buffer.CurrentChar;

            do
            {
                if (CharCodeMapManager.Instance.GetCharacterMap()[ch] == CharCode.WhiteSpace)
                {

                    //--Saw a whitespace character:  fetch the next character.
                    ch = buffer.GetChar();
                }
                else if (ch == '{')
                {
                    //--Skip over a comment, then fetch the next character.
                    do
                    {
                        ch = buffer.GetChar();
                    } while ((ch != '}') && (ch != 0x7e));

                    if (ch != 0x7e)
                    {
                        ch = buffer.GetChar();
                    }
                    else
                    {
                        // throw an unexpected end of file execption here
                    }
                }
            } while ((CharCodeMapManager.Instance.GetCharacterMap()[ch] == CharCode.WhiteSpace) || (ch == '{'));
        }
        
        #region IScanner Members

        public TToken Get()
        {
            TToken token;
            skipWhiteSpace();

            //--Determine the token class, based on the current character.
            switch (CharCodeMapManager.Instance.GetCharacterMap()[buffer.CurrentChar])
            {
                case CharCode.Letter: token = new WordToken(); break;
                case CharCode.Digit: token = new NumberToken(); break;
                case CharCode.Quote: token = new StringToken(); break;
                case CharCode.Special: token = new SpecialToken(); break;
                case CharCode.EndOfFile: token = new EOFToken(); break;
                default: if (buffer.CurrentPosition == 0 && buffer.CurrentChar == 0) token = new EOFToken(); else token = new ErrorToken(); break;
            }

            //--Extract a token of that class, and return a pointer to it.
            token.Get(buffer);
            return token;
        }

        #endregion
    }
}
