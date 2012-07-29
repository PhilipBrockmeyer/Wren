using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Wren.Emulation.MasterSystem.InstructionAdvice
{
    public class CallStack : FilteredInstructionAdvice
    {
        public CallStack()
        {
            IsFilterEnabled = true;
            this.EnableOpcode("C4");
        }


        protected override Expression GetExpressionImpl(InstructionInfo info, IExpressionLibraryRegistry expressionLibraries)
        {
            throw new NotImplementedException();
        }
    }
}
