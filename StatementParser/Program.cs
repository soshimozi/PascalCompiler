using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PascalCompiler.StatementParser;
using System.IO;

namespace PascalCompiler.StatementParser
{
    class Program
    {
        static void Main(string[] args)
        {
 
//            string source = @"
//PROGRAM fibonacci (output);
//
//VAR
//    number : integer;
//
//FUNCTION fib (n : integer) : integer;
//
//    BEGIN
//	IF n <= 1 THEN fib := n
//	ELSE fib := fib(n - 2) + fib(n - 1)
//    END;
//
//BEGIN
//    FOR number:= 0 TO 16 DO BEGIN
//	writeln('number = ', number:2,
//		' fibonacci = ', fib(number):4);
//    END;
//END.";

            string simpleSource = @"identifier := 123;
somestring := 'abc';

updatevalue := identifier + 5;

output := updatevalue

.";

            string testNewtons 
                = 
@"{Square roots by Newton's algorithm.}

number := input;

root := number;
root := (number/root + root)/2; output := root;
root := (number/root + root)/2; output := root;
root := (number/root + root)/2; output := root;
root := (number/root + root)/2; output := root;
root := (number/root + root)/2; output := root;
root := (number/root + root)/2; output := root;
root := (number/root + root)/2; output := root;
root := (number/root + root)/2; output := root;
root := (number/root + root)/2; output := root;
.";

            string source;

            using (StreamReader reader = new StreamReader(File.OpenRead("source.pas")))
            {
                source = reader.ReadToEnd();
            }

            SharedProperties.globalSymtab = Symtab.CreateTable();

            TextBuffer buffer = new TextBuffer(File.OpenRead("source.pas"));

			IntermediateCode icode = new IntermediateCode();
			SymtabStack symbolTableStack = new SymtabStack(SharedProperties.globalSymtab);

            StagingParser parser = new StagingParser(buffer, icode, symbolTableStack);
            parser.Error += new EventHandler<ErrorEventArgs>(parser_Error);
            parser.Parse();

            Console.WriteLine();


            // -- print the parsers summary
            string message = string.Format("{0} source lines", buffer.LineNumber - 1);
            int spaceCount = (Console.WindowWidth - message.Length) / 2;
            Console.WriteLine(string.Format("{0," + spaceCount + "}", message));

            message = string.Format("{0} syntax errors.", buffer.ErrorCount);
            spaceCount = (Console.WindowWidth - message.Length) / 2;
            Console.WriteLine(string.Format("{0," + spaceCount + "}", message));

            Console.WriteLine();

            TExecutor executer = new TExecutor(icode);
            executer.Error += new EventHandler<RuntimeErrorArgs>(executer_Error);
            executer.Go();

            if (executer.ErrorCount > 0)
            {
                Console.WriteLine("Successful completion.  {0} statements executed.", executer.StatementCount);
            }
            else
            {
                Console.WriteLine("Completed with {0} errors.", executer.ErrorCount);
            }

            Console.ReadKey();
        }

        static void executer_Error(object sender, RuntimeErrorArgs e)
        {
            Console.WriteLine("*** Runtime Error: {0}", e.RuntimeError.ToString());
        }

        static void parser_Error(object sender, ErrorEventArgs e)
        {
            Console.WriteLine("{0," + e.CharacterPosition + "}", "^");
            Console.WriteLine("*** Syntax Error: {0}", e.ErrorCode.ToString());
        }
    }
}
