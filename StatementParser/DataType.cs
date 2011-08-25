using System;
using System.Collections.Generic;
using System.Text;

namespace PascalCompiler.StatementParser
{
    public class DataType
    {
        private TypeCode typeCode;

        public TypeCode Code
        {
            get { return typeCode; }
            set { typeCode = value; }
        }

        public static bool CheckAssignmentTypeCompatible(DataType targetType, DataType valueType, IInputBuffer buffer)
        {
            //--Two identical types.
            if (targetType.Code == valueType.Code) return true;

            //--real := integer
            if ((targetType.Code == TypeCode.Double || targetType.Code == TypeCode.Single)
                    && IsIntegerType(valueType.Code))
            {
                return true;
            }

            // integer := real
            if (IsIntegerType(targetType.Code)
                    && (targetType.Code == TypeCode.Double || targetType.Code == TypeCode.Single))
            {
                return true;
            }

            return false;
        }


        public static bool IsIntegerType(TypeCode code)
        {
            return (code == TypeCode.Int16 ||
            code == TypeCode.Int32 ||
            code == TypeCode.Int64 ||
            code == TypeCode.UInt16 ||
            code == TypeCode.UInt32 ||
            code == TypeCode.UInt64);
        }
    }
}
