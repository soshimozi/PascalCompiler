using System;
using System.Collections.Generic;
using System.Text;

namespace PascalCompiler.StatementParser
{
    public class WordToken : TToken
    {
        Dictionary<int, CharCode> characterMap = CharCodeMapManager.Instance.GetCharacterMap();

        bool _reservedWord = false;

        public WordToken() { code = TTokenCode.Identifier; }

        public override bool IsReservedWord
        {
            get { return _reservedWord; }
        }

        public override void Get(IInputBuffer buffer)
        {
            StringBuilder tokenBuilder = new StringBuilder();
            char ch = buffer.CurrentChar;  // char fetched from input
            do
            {
                tokenBuilder.Append(ch);
                ch = buffer.GetChar();
            } while (characterMap[ch] == CharCode.Letter ||
                        characterMap[ch] == CharCode.Digit);

            tokenString = tokenBuilder.ToString();
            checkForReservedWord();
        }


        public override void Print(IOutputBuffer buffer)
        {
            string line;
            if (Code == TTokenCode.Identifier)
            {
                line = string.Format("\t{0,-18} {1}", ">> identifier:", TokenString);
            }
            else
            {
                line = string.Format("\t{0, -18} {1}", ">> reserved word:", TokenString);
            }

            buffer.PutLine(line);
        }

        private void checkForReservedWord()
        {
            int len = TokenString.Length;

            code = TTokenCode.Identifier;  // first assume it's an identifier
            ReservedWord rw = ReservedWordDictionary.Instance.FindReservedWord(TokenString);
            if (rw != null)
            {
                code = rw.Code;
                _reservedWord = true;
            }
            else
            {
                _reservedWord = false;
            }
        }

        public override bool IsDelimiter
        {
            get { throw new NotImplementedException(); }
        }
    }

}
