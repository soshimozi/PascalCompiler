using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace PascalCompiler.StatementParser
{
    public class DataValue
    {
        private object m_value;

        public int IntegerValue
        {
            get { return SafeConvert<int>(m_value); }
            set { m_value = value; }
        }

        public float RealValue
        {
            get { return SafeConvert<float>(m_value); }
            set { m_value = value; }
        }

        public char CharacterValue
        {
            get { return SafeConvert<char>(m_value); }
            set { m_value = value; }
        }

        public string StringValue
        {
            get { return SafeConvert<string>(m_value); }
            set { m_value = value; }
        }

        private T SafeConvert<T>(object value)
        {
            T converted = default(T);


            if (value is IConvertible)
            {
                // see if the value can be converted first
                try
                {
                    IConvertible convertible = value as IConvertible;
                    converted = (T)convertible.ToType(typeof(T), new CultureInfo("en-US"));
                }
                catch (Exception ex)
                {
                    converted = default(T);
                }
            }
            else
            {
                // otherwise just try to cast it
                try
                {
                    converted = (T)value;
                }
                catch (Exception ex)
                {
                    converted = default(T);
                }
            }

            return converted;
        }
    }
}
