using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PascalCompiler.StatementParser
{
    class SymbolTableManager
    {
        private static SymbolTableManager m_instance;

        List<Symtab> m_symbolTables = new List<Symtab>();

        private SymbolTableManager()
        {
        }

        public static SymbolTableManager Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new SymbolTableManager();
                }

                return m_instance;
            }
        }

        public int CreateTable()
        {
            int id = m_symbolTables.Count;
            Symtab table = new Symtab(id);
            m_symbolTables.Add(table);

            return id;
        }

        public Symtab GetSymbolTable(int index)
        {
            if (index < 0 || index >= m_symbolTables.Count)
            {
                throw new ArgumentOutOfRangeException("Index is out of range.");
            }

            return m_symbolTables[index];
        }

        public int TableCount { get { return m_symbolTables.Count; } }
    }
}
