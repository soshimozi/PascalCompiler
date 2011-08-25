using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PascalCompiler.StatementParser
{
    class TListBuffer : TTextOutBuffer
    {
        String pSourceFileName;     // ptr to source  file name (for page header)
        string date;                // date string for page header
        int pageNumber;             // current page number
        int lineCount;              // count of lines in the current page

        private const char formFeedChar = '\f';

        private bool listFlag;
        private int maxLinesPerPage;
        private int maxPrintLineLength;

        public TListBuffer(bool listFlag, int maxLinesPerPage, int maxPrintLineLength)
        {
            this.listFlag = listFlag;
            this.maxLinesPerPage = maxLinesPerPage;
            this.maxPrintLineLength = maxPrintLineLength;
        }

        void PrintPageHeader()
        {
            Console.Write(formFeedChar);
            Console.WriteLine("Page {0}  {1}   {2}", ++pageNumber, pSourceFileName, date);
            Console.WriteLine();

            // reset line count
            lineCount = 0;
        }

        public void Initialize(String fileName)
        {
            text = string.Empty;

            pageNumber = 0;
            pSourceFileName = fileName;

            date = DateTime.Now.ToShortDateString();

            PrintPageHeader();
        }

        public void PutLine(String pText, int lineNumber, int nestingLevel)
        {
            text = string.Format("{0:4} {1}: {2}", lineNumber, nestingLevel, pText);
            PutLine();
        }

        protected override void PutLine()
        {
            // -- Start a new page if the current one is full
            if (listFlag && (lineCount == maxLinesPerPage)) PrintPageHeader();

            // -- Truncate the line if it's too long
            if (text.Length > maxPrintLineLength)
                text = text.Substring(0, maxPrintLineLength);

            Console.WriteLine(text);
            text = string.Empty;

            ++lineCount;
        }
    }
}
