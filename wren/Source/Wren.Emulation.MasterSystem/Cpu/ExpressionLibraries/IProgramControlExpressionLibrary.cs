using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Wren.Emulation.MasterSystem.ExpressionLibraries
{
    public interface IProgramControlExpressionLibrary : IExpressionLibrary
    {
        Expression ParameterByte1 { get; }
        Expression ParameterByte2 { get; }
        Expression ParameterWord { get; }
        Expression CycleCounter { get; }
        Expression ProgramCounterRegister { get; }
        Expression ReadAndIncrementProgramCounter { get; }

        Expression IsBreakpoint { get; }
        Expression BreakpointHandler { get; }
        Expression InteruptAddress { get; }
        Expression InstructionSize { get; }
    }
}
