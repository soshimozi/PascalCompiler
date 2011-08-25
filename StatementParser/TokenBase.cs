using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PascalCompiler.StatementParser
{
    public abstract class TokenBase : IToken
    {
        internal TokenBase(TokenCode tc)
        {
            Code = tc;
        }

        #region IToken Members

        public abstract void Get(IInputBuffer buffer);

        public virtual void Print(IOutputBuffer buffer)
        {
        }

        public virtual bool IsDelimiter
        {
            get { return false; }
        }

        public virtual bool IsReservedWord
        {
            get { return false; }
        }

        public TokenCode Code
        {
            get;
            set;
        }

        public DataType DataType
        {
            get;
            set;
        }

        DataValue m_value = null;
        public DataValue TokenValue
        {
            get { if (m_value == null) { m_value = new DataValue(); } return m_value; }
        }

        public string TokenString
        {
            get;
            set;
        }

        #endregion
    }
}
