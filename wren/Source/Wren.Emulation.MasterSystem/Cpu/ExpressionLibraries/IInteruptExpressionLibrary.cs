using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Wren.Emulation.MasterSystem.ExpressionLibraries
{
    public interface IInteruptExpressionLibrary : IExpressionLibrary
    {
        Expression IsHalted { get; }
        Expression InteruptVectorRegister { get; }
        Expression IFF1 { get; }
        Expression IFF2 { get; }
        Expression InteruptMode { get; }
        Expression InteruptRequested { get; }

        Expression RefreshRegister { get; }
    }
}
