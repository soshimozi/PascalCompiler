using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PascalCompiler.StatementParser
{
    class BackendManager
    {
        private static BackendManager m_instance = null;

        private IntermediateCode m_icode = new IntermediateCode();

        internal IntermediateCode IntermediateCode
        {
            get { return m_icode; }
            set { m_icode = value; }
        }
        private BackendManager()
        {
        }

        public static BackendManager Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new BackendManager();
                }

                return m_instance;
            }
        }
        
    
    }
}
