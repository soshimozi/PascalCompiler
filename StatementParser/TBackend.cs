using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PascalCompiler.StatementParser
{
    public abstract class TBackend
    {
        protected TToken pToken;        // ptr to current token
        protected TTokenCode token;      // code of current token 
        protected SymtabNode pNode;               // ptr to symtab node
        protected IntermediateCode icode;

        public TBackend(IntermediateCode icode)
        {
            this.icode = icode;
        }

        protected void GetToken()
        {
            pToken = icode.Get();
            token = pToken.Code;
            pNode = icode.SymtabNode;
        }

        protected void GoTo(int location) { icode.GoTo(location); }
        protected int CurrentLocation
        {
            get { return icode.CurrentLocation; }
        }

        public abstract void Go();
    }
}
