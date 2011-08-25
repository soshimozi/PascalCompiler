using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PascalCompiler.StatementParser
{
    public class TExecutor : TBackend
    {
        private SymtabNode pInputNode;
        private SymtabNode pOutputNode;
        private Stack<float> runtimeStack = new Stack<float>();

        private int statementCount;
        private int errorCount;

        public TExecutor(IntermediateCode icode)
            : base(icode)
        {
            pInputNode = SharedProperties.globalSymtab.Search("input");
            pOutputNode = SharedProperties.globalSymtab.Search("output");
        }

        public int StatementCount { get { return statementCount; } }
        public int ErrorCount { get { return errorCount; } }

        public override void Go()
        {
            // -- Reset the icode to the begining
            // -- and extract the first token
            icode.Reset();
            GetToken();

            // -- Loop to execute statements until end of the program.
            do
            {
                ExecuteStatement();


                // -- Skip semicolons.
                while (token == TTokenCode.Semicolon) GetToken();
            } while (token != TTokenCode.Period);
        }

        private void ExecuteStatement()
        {
            if (token != TTokenCode.Begin) statementCount++;

            switch (token)
            {
                case TTokenCode.Identifier:
                    ExecuteAssignment();
                    break;

                case TTokenCode.Repeat:
                    ExecuteRepeat();
                    break;

                case TTokenCode.Begin:
                    ExecuteCompound();
                    break;

                case TTokenCode.While:
                case TTokenCode.If:
                case TTokenCode.For:
                case TTokenCode.Case:
                    OnError(TRuntimeError.rteUnimplementedRuntimeFeature);
                    break;
            }

        }

        private void ExecuteStatementList(TTokenCode terminator)
        {
            // -- Loop to execute statements and skip semicolons.
            do
            {
                ExecuteStatement();
                while (token == TTokenCode.Semicolon) GetToken();

            } while (token != terminator);
        }

        private void ExecuteCompound()
        {
            GetToken();

            // -- <stmt-list> END
            ExecuteStatementList(TTokenCode.End);

            GetToken();
        }

        private void ExecuteRepeat()
        {
            int atLoopStart = CurrentLocation;    // Location of the loop start

            do
            {
                GetToken();

                // --<stmt-list> UNTIL
                ExecuteStatementList(TTokenCode.Until);

                // -- <expr>
                GetToken();
                ExecuteExpression();

                // Decide whether or not to branch back to the loop start.
                if (runtimeStack.Pop() == 0) GoTo(atLoopStart);

            } while (CurrentLocation == atLoopStart);
        }

        private void ExecuteAssignment()
        {
            SymtabNode pTargetNode = pNode;

            GetToken(); // :=
            GetToken(); // first token of expression

            // -- Execute the expression and pop its value into the
            // -- target variable's symbol table node.
            ExecuteExpression();

            pTargetNode.Value = runtimeStack.Pop();

            // -- If the target variable is "output", print it's value
            if (pTargetNode == pOutputNode)
            {
                Console.WriteLine("At {0}: output = {1}", icode.CurrentLineNumber, pTargetNode.Value);
            }
        }

        private void ExecuteExpression()
        {
            TTokenCode op;

            ExecuteSimpleExpression();

            // -- If there we now see a relational operator,
            // -- execute a second simple expression
            if ((token == TTokenCode.Equal) || (token == TTokenCode.Ne) ||
                (token == TTokenCode.Lt) || (token == TTokenCode.Gt) ||
                (token == TTokenCode.Le) || (token == TTokenCode.Ge))
            {
                op = token;

                GetToken();
                ExecuteSimpleExpression();

                // -- Pop off the two operand values, ...
                float operand2 = runtimeStack.Pop();
                float operand1 = runtimeStack.Pop();

                // -- ... perform the operation, and push the resulting value
                // --     onto the runtime stack.
                switch(op)
                {
                    case TTokenCode.Equal:
                        runtimeStack.Push(operand1 == operand2 ? 1 : 0);
                        break;

                    case TTokenCode.Ne:
                        runtimeStack.Push(operand1 != operand2 ? 1 : 0);
                        break;

                    case TTokenCode.Lt:
                        runtimeStack.Push(operand1 < operand2 ? 1 : 0);
                        break;

                    case TTokenCode.Gt:
                        runtimeStack.Push(operand1 > operand2 ? 1 : 0);
                        break;

                    case TTokenCode.Le:
                        runtimeStack.Push(operand1 <= operand2 ? 1 : 0);
                        break;

                    case TTokenCode.Ge:
                        runtimeStack.Push( operand1 >= operand2 ? 1 : 0);
                        break;
                }
            }
        }

        private void ExecuteSimpleExpression()
        {
            TTokenCode op;   // binary operator
            TTokenCode unaryOp = TTokenCode.Plus; // unary operator

            // -- Unary + or -
            if ((token == TTokenCode.Plus) || (token == TTokenCode.Minus))
            {
                unaryOp = token;
                GetToken();
            }

            // -- Execute the first term and then negate it's value
            // -- if there was a numary -.
            ExecuteTerm();
            if (unaryOp == TTokenCode.Minus) runtimeStack.Push(-runtimeStack.Pop());

            // -- Loop to execute subsequent additive operators and terms.
            while ((token == TTokenCode.Plus) || (token == TTokenCode.Minus) ||
                (token == TTokenCode.Or))
            {
                op = token;

                GetToken();
                ExecuteTerm();

                // -- Pop off the two operand values, ...
                float operand2 = runtimeStack.Pop();
                float operand1 = runtimeStack.Pop();

                switch (op)
                {
                    case TTokenCode.Plus:
                        runtimeStack.Push(operand1 + operand2);
                        break;

                    case TTokenCode.Minus:
                        runtimeStack.Push(operand1 - operand2);
                        break;

                    case TTokenCode.Or:
                        runtimeStack.Push((operand1 != 0) || (operand2 != 0) ? 1 : 0);
                        break;
                }

            }
        }

        private void ExecuteTerm()
        {
            TTokenCode op;

            // -- Execute the first factor.
            ExecuteFactor();

            // -- Loop to execute subsequent multiplicative operators and factors.
            while ((token == TTokenCode.Star) || (token == TTokenCode.Slash) ||
                (token == TTokenCode.Div) || (token == TTokenCode.Mod) ||
                (token == TTokenCode.And))
            {
                op = token;

                GetToken();
                ExecuteFactor();

                // -- Pop off the two operand values, ...
                float operand2 = runtimeStack.Pop();
                float operand1 = runtimeStack.Pop();

                // -- .. perform the operation, and push the resulting value
                // --    onto the runtime stack
                bool divZeroFlag = false;    // true if division by 0, else false
                switch (op)
                {
                    case TTokenCode.Star:
                        runtimeStack.Push(operand1 * operand2);
                        break;

                    case TTokenCode.Slash:
                        if (operand2 != 0) runtimeStack.Push(operand1 / operand2);
                        else divZeroFlag = true;
                        break;

                    case TTokenCode.Div:
                        if (operand2 != 0) runtimeStack.Push((int)operand1 / (int)operand2);
                        else divZeroFlag = true;
                        break;

                    case TTokenCode.Mod:
                        if (operand2 != 0) runtimeStack.Push((int)operand1 % (int)operand2);
                        else divZeroFlag = true;
                        break;

                    case TTokenCode.And:
                        runtimeStack.Push((operand1 != 0) && (operand2 != 0) ? 1 : 0);
                        break;

                }

                if (divZeroFlag)
                {
                    OnError(TRuntimeError.rteDivisionByZero);
                    runtimeStack.Push(0);
                }
            }
        }

        private void ExecuteFactor()
        {
            switch (token)
            {
                case TTokenCode.Identifier:

                    // -- If the variable is "input", propmpt for its value.
                    if (pNode == pInputNode)
                    {
                        Console.Write(">> At {0}: input ? ", icode.CurrentLineNumber);

                        float value = 0;
                        if (!float.TryParse(Console.ReadLine(), out value))
                        {
                            OnError(TRuntimeError.rteInvalidUserInput);
                        }

                        pNode.Value = value;
                    }

                    runtimeStack.Push(pNode.Value);
                    GetToken();
                    break;

                case TTokenCode.Number:

                    // -- Push the number's value onto the runtime stack
                    runtimeStack.Push(pNode.Value);
                    GetToken();
                    break;

                case TTokenCode.String:
                    // -- Just push 0 for now
                    runtimeStack.Push(0);
                    GetToken();
                    break;

                case TTokenCode.Not:

                    // -- Execute factor and invert its value.
                    GetToken();
                    ExecuteFactor();
                    runtimeStack.Push((runtimeStack.Pop() != 0) ? 0 : 1);
                    break;

                case TTokenCode.LParen:

                    // -- Parenthesized subexpression:  Call ExecuteExpression
                    // --                               recursively.
                    GetToken();     // first token after (
                    ExecuteExpression();
                    GetToken();     // first token after )
                    break;




            }
        }

        public event EventHandler<RuntimeErrorArgs> Error;

        protected void OnError(TRuntimeError error)
        {
            errorCount++;

            if (Error != null)
            {
                Error(this, new RuntimeErrorArgs() { RuntimeError = error, LineNumber = icode.CurrentLineNumber });
            }
        }
    }
}
