using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Wren.Emulation.MasterSystem.ExpressionLibraries;

namespace Wren.Emulation.MasterSystem
{
    public interface IInstructionAdvice
    {
        Expression GetExpression(InstructionInfo info, IExpressionLibraryRegistry expressionLibraries);
    }
}
