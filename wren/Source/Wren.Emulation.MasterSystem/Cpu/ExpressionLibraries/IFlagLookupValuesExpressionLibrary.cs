using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Wren.Emulation.MasterSystem.ExpressionLibraries
{
    public interface IFlagLookupValuesExpressionLibrary : IExpressionLibrary
    {
        Expression AdditionFlagsCalcultorFlags { get; }
        Expression AdditionWithCarryFlagsCalcultorFlags { get; }
        Expression DecrementFlagsCalcultorFlags { get; }
        Expression IncrementFlagsCalcultorFlags { get; }
        Expression SignZeroParityCalcultorFlags { get; }
        Expression SubtractionFlagsCalcultorFlags { get; }
        Expression SubtractionWithCarryFlagsCalcultorFlags { get; }
    }
}
