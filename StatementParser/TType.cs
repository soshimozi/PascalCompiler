using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PascalCompiler.StatementParser
{
    public class TType
    {
        int refCount;

        public TFormCode Form { get; set; }
        public int size { get; set; }
        public SymtabNode TypeId { get; set; }
        public Enumeration Enumeration;
        public SubRange SubRange;
        public TArray array;
        public Record record;

        public static TType pIntegerType = null;
        public static TType pRealType = null;
        public static TType pBooleanType = null;
        public static TType pCharType = null;
        public static TType pDummyType = null;

        public TType(TFormCode fc, int s, SymtabNode pId)
        {
            Form = fc;
            size = s;
            TypeId = pId;
            refCount = 0;

            switch (fc)
            {
                case TFormCode.fcSubrange:
                    SubRange.BaseType = null;
                    break;

                case TFormCode.fcArray:
                    array.pIndexType = array.pElmtType = null;
                    break;

                default:
                    break;
            }
        }

        public TType(int length)
        {
            Form = TFormCode.fcArray;
            size = length;
            TypeId = null;
            refCount = 0;

            array.pIndexType = array.pElmtType = null;
            TType.SetType(ref array.pIndexType, new TType(TFormCode.fcSubrange, sizeof(int), null));
            TType.SetType(ref array.pElmtType, TType.pCharType);
            array.elmtCount = length;

            //--Integer subrange index type, range 1..length
            TType.SetType(ref array.pIndexType.SubRange.BaseType, TType.pIntegerType);
            array.pIndexType.SubRange.min = 1;
            array.pIndexType.SubRange.max = length;
        }

        public bool IsScalar { get { return Form != TFormCode.fcArray && Form != TFormCode.fcRecord; } }

        public static void InitializePredefinedTypes(Symtab pSymtab)
        {
            SymtabNode pIntegerId = pSymtab.Enter("integer", TDefnCode.dcType);
            SymtabNode pRealId = pSymtab.Enter("real", TDefnCode.dcType);
            SymtabNode pBooleanId = pSymtab.Enter("boolean", TDefnCode.dcType);
            SymtabNode pCharId = pSymtab.Enter("char", TDefnCode.dcType);
            SymtabNode pFalseId = pSymtab.Enter("false", TDefnCode.dcType);
            SymtabNode pTrueId = pSymtab.Enter("true", TDefnCode.dcType);

            if (pIntegerType == null)
            {
                SetType(ref pIntegerType, new TType(TFormCode.fcScalar, sizeof(int), pIntegerId));
            }
            if (pRealType == null)
            {
                SetType(ref pRealType,
                    new TType(TFormCode.fcScalar, sizeof(float), pRealId));
            }
            if (pBooleanType == null)
            {
                SetType(ref pBooleanType,
                    new TType(TFormCode.fcEnum, sizeof(int), pBooleanId));
            }
            if (pCharType == null)
            {
                SetType(ref pCharType,
                    new TType(TFormCode.fcScalar, sizeof(char), pCharId));
            }

            //--Link each predefined type's id node to its type object.
            SetType(ref pIntegerId.pType, pIntegerType);
            SetType(ref pRealId.pType, pRealType);
            SetType(ref pBooleanId.pType, pBooleanType);
            SetType(ref pCharId.pType, pCharType);

            //--More initialization for the boolean type object.
            pBooleanType.Enumeration.max = 1;        // max value
            pBooleanType.Enumeration.ConstIds = pFalseId; // first constant

            //--More initialization for the "false" and "true" id nodes.
            pFalseId.defn.constantValue.integerValue = 0;
            pTrueId.defn.constantValue.integerValue = 1;
            SetType(ref pTrueId.pType, pBooleanType);
            SetType(ref pFalseId.pType, pBooleanType);
            pFalseId.Next = pTrueId;  // "false" node points to "true" node

            //--Initialize the dummy type object that will be used
            //--for erroneous type definitions and for typeless objects.
            SetType(ref pDummyType, new TType(TFormCode.fcNone, 1, null));

        }

        public static void RemovePredefinedTypes()
        {
            RemoveType(ref pIntegerType);
            RemoveType(ref pRealType);
            RemoveType(ref pBooleanType);
            RemoveType(ref pCharType);
            RemoveType(ref pDummyType);
        }

        public static TType SetType(ref TType pTargetType, TType pSourceType)
        {
            ++pSourceType.refCount;
            pTargetType = pSourceType;

            return pSourceType;
        }

        private static void RemoveType(ref TType pType)
        {
            if (pType != null && (--pType.refCount == 0))
            {
                pType = null;
            }
        }

        //--------------------------------------------------------------
        //  CheckRelOpOperands  Check that the types of the two operands
        //                      of a relational operator are compatible.
        //                      Flag an incompatible type error if not.
        //
        //      pType1 : ptr to the first  operand's type object
        //      pType2 : ptr to the second operand's type object
        //--------------------------------------------------------------


        /// <summary>
        ///--------------------------------------------------------------
        ///  CheckRelOpOperands  Check that the types of the two operands
        ///                      of a relational operator are compatible.
        ///                      Flag an incompatible type error if not.
        ///
        ///      pType1 : ptr to the first  operand's type object
        ///      pType2 : ptr to the second operand's type object
        ///--------------------------------------------------------------
        /// </summary>
        /// <param name="firstOperandType">The first operand's type object.</param>
        /// <param name="secondOperandType">The second operand's type object.</param>
        /// <returns></returns>
        public static bool CheckRelOpOperands(TType firstOperandType, TType secondOperandType)
        {
            firstOperandType = firstOperandType.Base;
            secondOperandType = secondOperandType.Base;

            //--Two identical scalar or enumeration types.
            if ((firstOperandType == secondOperandType)
            && ((firstOperandType.Form == TFormCode.fcScalar) || (firstOperandType.Form == TFormCode.fcEnum)))
            {
                return true;
            }

            //--One integer operand and one real operand.
            if (((firstOperandType == pIntegerType) && (secondOperandType == pRealType))
            || ((secondOperandType == pIntegerType) && (firstOperandType == pRealType)))
            {
                return true;
            }

            //--Two strings of the same length.
            if ((firstOperandType.Form == TFormCode.fcArray)
            && (secondOperandType.Form == TFormCode.fcArray)
            && (firstOperandType.array.pElmtType == pCharType)
            && (secondOperandType.array.pElmtType == pCharType)
            && (firstOperandType.array.elmtCount == secondOperandType.array.elmtCount))
            {
                return true;
            }

            return false;
        }

        public TType Base
        {
            get
            {
                return Form == TFormCode.fcSubrange ? SubRange.BaseType : this;
            }
        }

        public static void InitializeStandardRoutines(Symtab globalSymbolTable)
        {
            throw new NotImplementedException();
        }

        public static bool RealOperands(TType firstOperandType, TType secondOperandType)
        {
            firstOperandType = firstOperandType.Base;
            secondOperandType = secondOperandType.Base;

            return ((firstOperandType == pRealType) && (secondOperandType == pRealType))
               || ((firstOperandType == pRealType) && (secondOperandType == pIntegerType))
               || ((secondOperandType == pRealType) && (firstOperandType == pIntegerType));
        }

        public static bool IntegerOperands(TType firstOperandType, TType secondOperandType)
        {
            firstOperandType = firstOperandType.Base;
            secondOperandType = secondOperandType.Base;

            return (firstOperandType == pIntegerType) && (secondOperandType == pIntegerType);
        }

        public static bool CheckBoolean(TType firstOperandType, TType secondOperandType)
        {
            if ((firstOperandType.Base != pBooleanType)
            || (secondOperandType != null && (secondOperandType.Base != pBooleanType)))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        internal static bool CheckIntegerOrReal(TType operandType1, TType operandType2 = null)
        {
            operandType2 = operandType2.Base;
            if ((operandType2 != pIntegerType) && (operandType2 != pRealType))
            {
                return false;
            }

            if (operandType2 != null)
            {
                operandType2 = operandType2.Base;
                if ((operandType2 != pIntegerType) && (operandType2 != pRealType))
                {
                    return false;
                }
            }

            return true;
        }

        //TType Base
        //{
        //    get { return Form == TFormCode.fcSubrange ? SubRange.BaseType : this; }
        //}

        internal static bool CheckAssignmentTypeCompatible(TType pTargetType, TType pValueType)
        {
            pTargetType = pTargetType.Base;
            pValueType = pValueType.Base;

            //--Two identical types.
            if (pTargetType == pValueType) return true;

            //--real := integer
            if ((pTargetType == pRealType)
            && (pValueType == pIntegerType)) return true;


            //--Two strings of the same length.
            if ((pTargetType.Form == TFormCode.fcArray)
            && (pValueType.Form == TFormCode.fcArray)
            && (pTargetType.array.pElmtType == TType.pCharType)
            && (pValueType.array.pElmtType == TType.pCharType)
            && (pTargetType.array.elmtCount ==
                        pValueType.array.elmtCount))
            {
                return true;
            }

            return false;
        }
    }

    public struct TArray
    {
        public TType pIndexType;       // ptr to index type object
        public TType pElmtType;        // ptr to elmt type object
        public int minIndex, maxIndex; // min and max index values
        public int elmtCount;          // count of array elmts
    }

    public struct Record
    {
        public Symtab pSymtab; // ptr to record fields symtab
    }

    public struct SubRange
    {
        public TType BaseType; // ptr to base type object
        public int min, max;   // min and max subrange limit values
    }

    public struct Enumeration
    {
        public SymtabNode ConstIds;  // ptr to list of const id nodes
        public int max;                // max constant value
    }

    public enum TFormCode
    {
        fcNone, fcScalar, fcEnum, fcSubrange, fcArray, fcRecord
    }

    
}
