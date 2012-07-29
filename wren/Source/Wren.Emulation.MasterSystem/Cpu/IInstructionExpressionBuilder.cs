using System;
using System.Linq.Expressions;
namespace Wren.Emulation.MasterSystem
{
    public interface IInstructionExpressionBuilder
    {
        Expression BuildExpression(InstructionInfo info);
    }
}
