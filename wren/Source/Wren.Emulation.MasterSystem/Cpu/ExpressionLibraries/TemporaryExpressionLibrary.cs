using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Wren.Emulation.MasterSystem.ExpressionLibraries
{
    public class TemporaryExpressionLibrary : ITemporaryExpressionLibrary
    {
        public ParameterExpression Temp1Parameter { get; private set; }
        public ParameterExpression Temp2Parameter { get; private set; }
        public ParameterExpression Temp3Parameter { get; private set; }
        public ParameterExpression Temp4Parameter { get; private set; }
        public ParameterExpression Temp5Parameter { get; private set; }
        public ParameterExpression Temp6Parameter { get; private set; }

        public TemporaryExpressionLibrary()
        {
            Temp1Parameter = Expression.Variable(typeof(Int32), "temp1");
            Temp2Parameter = Expression.Variable(typeof(Int32), "temp2");
            Temp3Parameter = Expression.Variable(typeof(Int32), "temp3");
            Temp4Parameter = Expression.Variable(typeof(Int32), "temp4");
            Temp5Parameter = Expression.Variable(typeof(Int32), "temp5");
            Temp6Parameter = Expression.Variable(typeof(Int32), "temp6");

        }

        public Expression Temp1 { get { return Temp1Parameter; } }
        public Expression Temp2 { get { return Temp2Parameter; } }
        public Expression Temp3 { get { return Temp3Parameter; } }
        public Expression Temp4 { get { return Temp4Parameter; } }
        public Expression Temp5 { get { return Temp5Parameter; } }
        public Expression Temp6 { get { return Temp6Parameter; } }

        public Expression InitializeLocals
        {
            get { return null; }
        }

        public Expression FinalizeLocals
        {
            get { return null; }
        }

        public IEnumerable<ParameterExpression> GetParameters()
        {
            return new ParameterExpression[] { };
        }

        public IEnumerable<Object> GetObjects(ICpuContext context)
        {
            return new Object[] { };
        }

        public IEnumerable<ParameterExpression> GetLocals()
        {
            yield return Temp1Parameter;
            yield return Temp2Parameter;
            yield return Temp3Parameter;
            yield return Temp4Parameter;
            yield return Temp5Parameter;
            yield return Temp6Parameter;
        }
    }
}
