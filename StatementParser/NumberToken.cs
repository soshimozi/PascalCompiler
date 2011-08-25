using System;
using System.Collections.Generic;
using System.Text;

namespace PascalCompiler.StatementParser
{
    public class NumberToken : TToken
    {
        public NumberToken() { code = TTokenCode.Number;  }

        const int maxInteger = 32767;
        const int maxExponent = 37;
        const int maxDigitCount = 20;

        int digitCount = 0;      // total no. of digits in number
        bool countError = false;  // true if too many digits, else false

        override public void Get(IInputBuffer buffer)
        {
            StringBuilder tokenBuilder = new StringBuilder();

            char ch;

            float numValue = 0;           // value of number ignoring
                                            //    the decimal point
            int wholePlaces = 0;            // no. digits before the decimal point
            int decimalPlaces = 0;          // no. digits after  the decimal point
            char exponentSign = '+';
            float eValue = 0;             // value of number after 'E'
            int exponent = 0;               // final value of exponent
            bool sawDotDotFlag = false;     // true if encountered '..',
                                            //   else false

            ch = buffer.CurrentChar;
            digitCount = 0;
            countError = false;

            code = TTokenCode.Error;    // we don't know what it is yet, but
            type = TDataType.tyInteger;  //    assume it'll be an integer

            //--Get the whole part of the number by accumulating
            //--the values of its digits into numValue.  wholePlaces keeps
            //--track of the number of digits in this part.
            if (accumulateValue(buffer, tokenBuilder, ref numValue))
            {
                wholePlaces = digitCount;

                ch = buffer.CurrentChar;

                ////--If the current character is a dot, then either we have a
                ////--fraction part or we are seeing the first character of a '..'
                ////--token.  To find out, we must fetch the next character.
                if (ch == '.')
                {
                    ch = buffer.GetChar();

                    if (ch == '.')
                    {

                        //--We have a .. token.  Back up bufferp so that the
                        //--token can be extracted next.
                        sawDotDotFlag = true;
                        buffer.PutBackChar();
                    }
                    else
                    {
                        type = TDataType.tyReal;
                        tokenBuilder.Append(".");

                        //--We have a fraction part.  Accumulate it into numValue.
                        if (!accumulateValue(buffer, tokenBuilder, ref numValue)) return;

                        // get the current character
                        ch = buffer.CurrentChar;

                        decimalPlaces = digitCount - wholePlaces;
                    }
                }

                //--Get the exponent part, if any. There cannot be an
                //--exponent part if we already saw the '..' token.
                if (!sawDotDotFlag && ((ch == 'E') || (ch == 'e')))
                {
                    type = TDataType.tyReal;
                    tokenBuilder.Append(ch);
                    ch = buffer.GetChar();

                    //--Fetch the exponent's sign, if any.
                    if ((ch == '+') || (ch == '-'))
                    {
                        exponentSign = ch;
                        tokenBuilder.Append(ch);
                        ch = buffer.GetChar();
                    }

                    //--Accumulate the value of the number after 'E' into eValue.
                    digitCount = 0;
                    if (!accumulateValue(buffer, tokenBuilder, ref eValue)) return;

                    ch = buffer.CurrentChar;
                    if (exponentSign == '-') eValue = -eValue;
                }

                //--Were there too many digits?
                if (countError) 
                {
                    return;
                }

                //--Calculate and check the final exponent value,
                //--and then use it to adjust the number's value.
                exponent = (int)eValue - decimalPlaces;
                if ((exponent + wholePlaces < -maxExponent) ||
                    (exponent + wholePlaces >  maxExponent)) 
                {
                    return;
                }
                
                if (exponent != 0)
                {
                    numValue *= (float)Math.Pow(10.0, exponent);
                }

                //--Check and set the numeric value.
                if (type == TDataType.tyInteger) 
                {
                    if ((numValue < -maxInteger) || (numValue > maxInteger)) 
                    {
                        return;
                    }

                    value.integerValue = (int)numValue;
                }
                else 
                {
                    value.realValue = numValue;
                }

                tokenString = tokenBuilder.ToString();
                code = TTokenCode.Number;
            }
        }

        public override void Print(IOutputBuffer buffer)
        {
            string lineText;
            if (type == TDataType.tyInteger)
            {
                lineText = string.Format("\t{0, -18} {1}", ">> integer:", value.integerValue);
            }
            else
            {
                lineText = string.Format("\t{0, -18} {1:g}", ">> real:", value.realValue);
            }

            buffer.PutLine(lineText);
        }

        private bool accumulateValue(IInputBuffer buffer, StringBuilder tokenBuilder, ref float value)
        {
            char ch = buffer.CurrentChar;
            Dictionary<int, CharCode> charCodeMap = CharCodeMapManager.Instance.GetCharacterMap();

            //--Error if the first character is not a digit.
            if (charCodeMap[ch] != CharCode.Digit)
            {
                return false;           // failure
            }

            //--Accumulate the value as long as the total allowable
            //--number of digits has not been exceeded.
            do
            {
                tokenBuilder.Append(ch);

                if (++digitCount <= maxDigitCount)
                {
                    value = 10 * value + (ch - '0');  // shift left and add
                }
                else countError = true;         // too many digits, but keep reading anyway

                ch = buffer.GetChar();
            } while (charCodeMap[ch] == CharCode.Digit);

            return true;               // success
        }

    }
}
