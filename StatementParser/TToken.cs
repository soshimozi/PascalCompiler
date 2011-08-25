using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PascalCompiler.StatementParser
{
    public abstract class TToken
    {
        protected TTokenCode code;
        protected TDataType  type;
        protected TDataValue value = new TDataValue();
        
        protected string tokenString;

        public TTokenCode Code { get { return code; } set { code = value; } }
        public TDataType DataType { get { return type; }}
        public TDataValue Value { get { return value; }}
        public string TokenString { get { return tokenString; } set { tokenString = value; } }

        public abstract void Get(IInputBuffer buffer);
        public virtual bool IsDelimiter { get { return false; } }
        public virtual bool IsReservedWord { get { return false; } }
        public abstract void Print(IOutputBuffer buffer);
    }

    public enum TDataType {
        tyDummy, tyInteger, tyReal, tyCharacter, tyString,
    }

    public enum TTokenCode
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
