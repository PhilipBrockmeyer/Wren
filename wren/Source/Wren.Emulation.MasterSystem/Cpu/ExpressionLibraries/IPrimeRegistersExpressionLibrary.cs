using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Wren.Emulation.MasterSystem.ExpressionLibraries
{
    public interface IPrimeRegistersExpressionLibrary : IExpressionLibrary
    {
        Expression RegisterAPrime { get; }
        Expression RegisterFPrime { get; }
        Expression RegisterBPrime { get; }
        Expression RegisterCPrime { get; }
        Expression RegisterDPrime { get; }
        Expression RegisterEPrime { get; }
        Expression RegisterLPrime { get; }
        Expression RegisterHPrime { get; }
    }
}
