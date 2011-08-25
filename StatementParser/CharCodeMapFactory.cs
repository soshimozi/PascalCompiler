using System;
using System.Collections.Generic;
using System.Text;

namespace PascalCompiler.StatementParser
{
    internal class CharCodeMapManager
    {
        static byte EndOfFileCharacter = 0x7f;

        static CharCodeMapManager _instance = null;
        
        Dictionary<int, CharCode> charCodeMap;
        private CharCodeMapManager()
        {
            charCodeMap = new Dictionary<int, CharCode>();
            InitalizeValues(); 
        }

        public static CharCodeMapManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CharCodeMapManager();
                }

                return _instance;
            }
        }

        public Dictionary<int, CharCode> GetCharacterMap()
        {
            return charCodeMap;
        }

        private void InitalizeValues()
        {
            int i;

            //--Initialize the character code map.
            for (i = 0; i < 127; ++i) charCodeMap[i] = CharCode.Error;
            for (i = 'a'; i <= 'z'; ++i) charCodeMap[i] = CharCode.Letter;
            for (i = 'A'; i <= 'Z'; ++i) charCodeMap[i] = CharCode.Letter;
            for (i = '0'; i <= '9'; ++i) charCodeMap[i] = CharCode.Digit;
            charCodeMap['?'] = CharCode.Special;
            charCodeMap['!'] = CharCode.Special;
            charCodeMap['+'] = charCodeMap['-'] = CharCode.Special;
            charCodeMap['*'] = charCodeMap['/'] = CharCode.Special;
            charCodeMap['='] = charCodeMap['^'] = CharCode.Special;
            charCodeMap['.'] = charCodeMap[','] = CharCode.Special;
            charCodeMap['<'] = charCodeMap['>'] = CharCode.Special;
            charCodeMap['('] = charCodeMap[')'] = CharCode.Special;
            charCodeMap['['] = charCodeMap[']'] = CharCode.Special;
            charCodeMap['{'] = charCodeMap['}'] = CharCode.Special;
            charCodeMap[':'] = charCodeMap[';'] = CharCode.Special;
            charCodeMap[' '] = charCodeMap['\t'] = CharCode.WhiteSpace;
            charCodeMap['\n'] = charCodeMap['\0'] = CharCode.WhiteSpace;
            charCodeMap['\''] = CharCode.Quote;
            charCodeMap[EndOfFileCharacter] = CharCode.EndOfFile;
        }
    }

    public enum CharCode
    {
        Letter,
        Digit,
        Special,
        Quote,
        WhiteSpace,
        EndOfFile,
        Error
    }
}
