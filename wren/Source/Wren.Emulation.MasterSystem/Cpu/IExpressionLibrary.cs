using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Wren.Emulation.MasterSystem
{
    public interface IExpressionLibrary
    {
        Expression InitializeLocals { get; }
        Expression FinalizeLocals { get; }

        IEnumerable<ParameterExpression> GetLocals();
        IEnumerable<ParameterExpression> GetParameters();
        IEnumerable<Object> GetObjects(ICpuContext context);
    }
}
