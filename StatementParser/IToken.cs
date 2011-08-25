using System;
using System.Collections.Generic;
using System.Text;

namespace PascalCompiler.StatementParser
{
    public interface IToken
    {
        void Get(IInputBuffer buffer);
        void Print(IOutputBuffer buffer);

        bool IsDelimiter
        {
            get;
        }

        bool IsReservedWord
        {
            get;
        }

        TokenCode Code
        {
            get;
            set;
        }

        DataType DataType
        {
            get;
            set;
        }

        DataValue TokenValue
        {
            get;
        }

        string TokenString
        {
            get;
            set;
        }
    }

    public enum TokenCode
    {
        Dummy,
        Do, If, In, Of, Or,
        To, And, Div, End, For, Mod, Nil, Not,
        Set, Var, Case, Else, File, Goto, Then, Type, With,
        Array, Begin, Const, Label, Until, While, Downto,
        Packed, Record, Repeat, Program, Function, Procedure,
        String,
        Identifier,
        UpArrow,
        Star,
        LParen,
        RParen,
        Minus,
        Plus,
        Equal,
        LBracket,
        RBracket,
        Semicolon,
        Comma,
        Slash,
        ColonEqual,
        Colon,
        Le,
        Ne,
        Lt,
        Ge,
        Gt,
        DotDot,
        Period,
        Error,
        Number,
        EndOfFile,
        Integer,
        LineMarker
    }
}
