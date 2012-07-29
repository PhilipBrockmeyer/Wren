using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Linq.Expressions;


namespace Wren.Emulation.MasterSystem.ExpressionLibraries
{
    public interface ITemporaryExpressionLibrary : IExpressionLibrary
    {
        Expression Temp1 { get; }
        Expression Temp2 { get; }
        Expression Temp3 { get; }
        Expression Temp4 { get; }
        Expression Temp5 { get; }
        Expression Temp6 { get; }
    }
}
