using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PascalCompiler.StatementParser
{
    class RuntimeStack
    {
        private const int stackSize = 128;
        private const int frameHeaderSize = 5;

        //private TStackItem[] stack = new TStackItem[stackSize];  // stack items
        private List<object> stack = new List<object>();

        private int tos;               // ptr to the top of the stack
        private int pFrameBase;        // ptr to current stack frame base

        public event EventHandler<RuntimeErrorArgs> RuntimeError;

        public RuntimeStack()
        {
            tos = -1;  // point to just below bottom of stack
            pFrameBase = 0;  // point to bottom of stack

            //--Initialize the program's stack frame at the bottom.
            Push(0);  // function return value
            Push(0);  // static  link
            Push(0);  // dynamic link
            Push(0);  // return address icode pointer
            Push(0);  // return address icode location
        }

        public void Push(int value)
        {
            if (tos < stackSize - 1) stack[++tos] = value;
            else OnRuntimeError(TRuntimeError.rteStackOverflow);
        }

        void Push(float value)
        {
            if (tos < stackSize - 1) stack[++tos] = value;
            else OnRuntimeError(TRuntimeError.rteStackOverflow);
        }

        void Push(char value)
        {
            if (tos < stackSize - 1) stack[++tos] = value;
            else OnRuntimeError(TRuntimeError.rteStackOverflow);
        }

        void Push(object obj)
        {
        if (tos < stackSize-1) stack[++tos] = obj;
        else OnRuntimeError(TRuntimeError.rteStackOverflow);
        }

        public object PushFrameHeader(int oldLevel, int newLevel,
                    IntermediateCode pIcode)
        {
            TFrameHeader pHeader = (TFrameHeader)stack[pFrameBase];
            object pNewFrameBase = stack[tos + 1];  // point to item just above
            //   current TOS item

            Push(0);  // function return value (placeholder)

            //--Compute the static link.
            if (newLevel == oldLevel + 1)
            {

                //--Callee nested within caller:
                //--Push address of caller's stack frame.
                Push(pHeader);
            }
            else if (newLevel == oldLevel)
            {

                //--Callee at same level as caller:
                //--Push address of common parent's stack frame.
                Push(pHeader.staticLink);
            }
            else /* newLevel < oldLevel */
            {

                //--Callee nested less deeply than caller:
                //--Push address of nearest commmon ancestor's stack frame.
                int delta = oldLevel - newLevel;

                while (delta-- >= 0)
                {
                    pHeader = (TFrameHeader)pHeader.staticLink;
                }
                Push(pHeader);
            }

            Push(pFrameBase);  // dynamic link
            Push(pIcode);      // return address icode pointer
            Push(0);           // return address icode location (placeholder)

            return pNewFrameBase;
        }

        public void ActivateFrame(int pNewFrameBase, int location)
        {
            pFrameBase = pNewFrameBase;
            ((TFrameHeader)stack[pFrameBase]).returnAddress.location = location;
        }

        public void PopFrame(SymtabNode pRoutineId, ref IntermediateCode pIcode)
        {
            TFrameHeader pHeader = (TFrameHeader)stack[pFrameBase];

            //--Don't do anything if it's the bottommost stack frame.
            if (pFrameBase != 0)
            {
                //--Return to the caller's intermediate code.
                pIcode = pHeader.returnAddress.icode;
                pIcode.GoTo((int)pHeader.returnAddress.location);

                //--Cut the stack back.  Leave a function value on top.
                
                tos = pFrameBase;

                //if (pRoutineId.defn.how != TDefnCode.dcFunction) --tos;
                //    pFrameBase = (TStackItem)pHeader.dynamicLink.Value;
            }
        }

        public object Pop() { return stack[tos--]; }
        public object TOS() { return stack[tos]; }

        public void AllocateValue(SymtabNode pId) { }
        public void DeallocateValue(SymtabNode pId) { }

        public object GetValueAddress(SymtabNode pId)
        {
            throw new NotImplementedException();
        }
        protected void OnRuntimeError(TRuntimeError error)
        {
            if (RuntimeError != null)
            {
                RuntimeError(this, new RuntimeErrorArgs() { RuntimeError = error });
            }
        }
    }

    //--Stack frame header
    class TFrameHeader {
	    public object functionValue;
	    public object staticLink;
	    public object dynamicLink;

        public struct ReturnAddress
        {
            public IntermediateCode icode;
            public int location;
        }

        public ReturnAddress returnAddress = new ReturnAddress();

    };
    public class TStackItem
    {
        public object Value;
    }

}
