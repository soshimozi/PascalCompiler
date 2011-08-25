using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PascalCompiler.StatementParser
{
    abstract class TTextOutBuffer
    {
        //private StringBuilder builder = new StringBuilder();
        public string Text { get { return text; } } // output text buffer
        protected String text;

        protected abstract void PutLine();

        public void PutLine(String pText)
        {
            text = pText;
            PutLine();
        }
    }
}
