using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PascalCompiler.StatementParser
{
    public interface IParser
    {
        void Parse();
    }

    //public enum ErrorCode
    //{
    //    UnexpectedEndOfFile,
    //    UnexpectedToken,
    //    MissingColonEqual,
    //    MissingRightParen,
    //    InvalidExpression,
    //    UndefinedIdentifier,
    //    MissingUntil,
    //    MissingSemicolon,
    //    MissingDo,
    //    MissingThen,
    //    MissingEnd,
    //    MissingOf,
    //    MissingConstant,
    //    MissingColon,
    //    InvalidConstant,
    //    MissingIdentifier,
    //    MissingToOrDownto
    //}
}
