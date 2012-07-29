using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using Wren.Emulation.MasterSystem.ExpressionLibraries;
using Wren.Emulation.MasterSystem.Instructions;
namespace Wren.Emulation.MasterSystem
{
    public interface IInstructionSpaceBuilder
    {
        Expression BuildInstructionSpaceExpression(IEnumerable<InstructionInfo> instructions, 
                                                   IProgramControlExpressionLibrary programControlExpressionLibrary, 
                                                   IInstructionExpressionBuilder instructionBuilder,
                                                   InstructionSpace instructionSpace);
    }
}
