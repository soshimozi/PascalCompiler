using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace PascalCompiler.StatementParser
{
    public class TextBuffer : IInputBuffer
    {
        protected static byte EndOfFileCharacter = 0x7f;
        protected TextReader reader;
        protected int virtualPosition = 0;
        protected char currentChar = char.MinValue;
        protected string currentLine = "";

        protected int lineNumber = 0;
        protected int nestingLevel = 0;

        protected int errorCount = 0;

        protected bool listSource;

        public TextBuffer(Stream stream)
        {
            reader = new StreamReader(stream);
        }

        public TextBuffer(string text)
        {
            try
            {
                reader = new StringReader(text);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region IInputBuffer Members

        public int ErrorCount
        {
            get { return errorCount; }
            set { errorCount = value; }
        }

        public int CurrentPosition
        {
            get { return virtualPosition; }
        }

        public char CurrentChar
        {
            get { return currentChar; }
        }

        public char GetChar()
        {
            char ch;                // character to return

            if (currentLine == null)
            {
                return (char)EndOfFileCharacter;
            }

            if (currentChar == EndOfFileCharacter)
            {
                return (char)EndOfFileCharacter;  // end of file
            }
            else if (virtualPosition == currentLine.Length)
            {
                GetLine();  // end of line
                ch = GetChar();
            }
            else
            {   // next char
                currentChar = currentLine[virtualPosition];
                ++virtualPosition;
                ch = currentChar;
            }

            return ch;
        }

        public char PutBackChar()
        {
            --virtualPosition;
            if (virtualPosition < 0)
                virtualPosition = 0;

            if (virtualPosition < currentLine.Length)
                currentChar = currentLine[virtualPosition];
            else
                currentChar = Char.MinValue;

            return currentChar;
        }

        public int LineNumber
        {
            get { return lineNumber; }
        }

        public bool ListSource
        {
            get { return listSource; }
            set { listSource = value; }
        }

        public void GetLine()
        {
            //--If at the end of the source file, return the end-of-file char.
            if (reader.Peek() == 0)
                currentChar = (char)EndOfFileCharacter;
            else
            {
                currentLine = reader.ReadLine();
                if (currentLine != null && currentLine.Length > 0)
                {
                    currentChar = currentLine[0];
                }
                else
                {
                    currentChar = (char)0;
                }

                virtualPosition = 0;

                lineNumber++;

                Console.WriteLine("{0} :  {1}", lineNumber, currentLine);
            }
        }


        #endregion
    }
}
