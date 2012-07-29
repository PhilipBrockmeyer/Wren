using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Wren.Emulation.MasterSystem.ExpressionLibraries;

namespace Wren.Emulation.MasterSystem.Instructions
{
    public class NoOperationInstruction
    {
        ITemporaryExpressionLibrary _tempLibrary;

        public NoOperationInstruction(ITemporaryExpressionLibrary tempLibrary)
        {
            _tempLibrary = tempLibrary;
        }

        [Instruction("NOP", 0x00, Cycles=4)]
        public Expression Nop
        {
            get
            {
                return Expression.Assign(_tempLibrary.Temp1, _tempLibrary.Temp1);
            }
        }
    }
}
