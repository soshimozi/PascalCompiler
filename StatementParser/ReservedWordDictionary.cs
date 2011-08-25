using System;
using System.Collections.Generic;
using System.Text;

namespace PascalCompiler.StatementParser
{
    internal class ReservedWordDictionary
    {
        static ReservedWordDictionary _instance = null;

        Dictionary<string, ReservedWord> reservedWords = new Dictionary<string, ReservedWord>();

        public static ReservedWordDictionary Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ReservedWordDictionary();
                }

                return _instance;
            }
        }

        private ReservedWordDictionary()
        {
            InitializeReservedWords();
        }

        public ReservedWord FindReservedWord(string key)
        {
            ReservedWord reservedWord = null;
            if (reservedWords.ContainsKey(key.ToLower()))
            {
                reservedWord = reservedWords[key.ToLower()];
            }

            return reservedWord;
        }

        private void InitializeReservedWords()
        {
            AddReservedWord("Do", TTokenCode.Do);
            AddReservedWord("If", TTokenCode.If);
            AddReservedWord("In", TTokenCode.In);
            AddReservedWord("Of", TTokenCode.Of);
            AddReservedWord("Or", TTokenCode.Or);
            AddReservedWord("To", TTokenCode.To);
            AddReservedWord("And", TTokenCode.And);
            AddReservedWord("Div", TTokenCode.Div);
            AddReservedWord("End", TTokenCode.End);
            AddReservedWord("For", TTokenCode.For);
            AddReservedWord("Mod", TTokenCode.Mod);
            AddReservedWord("Nil", TTokenCode.Nil);
            AddReservedWord("Not", TTokenCode.Not);
            AddReservedWord("Set", TTokenCode.Set);
            AddReservedWord("Var", TTokenCode.Var);
            AddReservedWord("Case", TTokenCode.Case);
            AddReservedWord("Else", TTokenCode.Else);
            AddReservedWord("File", TTokenCode.File);
            AddReservedWord("Goto", TTokenCode.Goto);
            AddReservedWord("Then", TTokenCode.Then);
            AddReservedWord("Type", TTokenCode.Type);
            AddReservedWord("With", TTokenCode.With);
            AddReservedWord("Array", TTokenCode.Array);
            AddReservedWord("Begin", TTokenCode.Begin);
            AddReservedWord("Const", TTokenCode.Const);
            AddReservedWord("Label", TTokenCode.Label);
            AddReservedWord("Until", TTokenCode.Until);
            AddReservedWord("While", TTokenCode.While);
            AddReservedWord("Downto", TTokenCode.Downto);
            AddReservedWord("Packed", TTokenCode.Packed);
            AddReservedWord("Record", TTokenCode.Record);
            AddReservedWord("Repeat", TTokenCode.Repeat);
            AddReservedWord("Program", TTokenCode.Program);
            AddReservedWord("Function", TTokenCode.Function);
            AddReservedWord("Procedure", TTokenCode.Procedure);
            AddReservedWord("integer", TTokenCode.Integer);
        }

        private void AddReservedWord(string tokenString, TTokenCode tokenCode)
        {
            ReservedWord rw = new ReservedWord() { Code = tokenCode, TokenString = tokenString };
            reservedWords.Add(tokenString.ToLower(), rw);
        }

    }
}
