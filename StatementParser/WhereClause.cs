using System;
using System.Collections.Generic;
using System.Text;

namespace PascalCompiler.StatementParser
{
    public class WhereClause
    {
        public enum Operation
        {
            Equal,
            LessThan,
            GreaterThan,
            GreaterThanOrEqual,
            LessThanOrEqual,
            NotEqual,
            Like,
            Set
        }

        string fieldName;
        Operation fieldOperation;
        int setSize;

        public int SetSize
        {
            get { return setSize; }
            set { setSize = value; }
        }

        public Operation FieldOperation
        {
            get { return fieldOperation; }
            set { fieldOperation = value; }
        }

        public string FieldName
        {
            get { return fieldName; }
            set { fieldName = value; }
        }
    }
}
