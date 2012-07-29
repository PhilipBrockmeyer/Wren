using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Wren.Emulation.MasterSystem
{
    public class ExpressionBuilder
    {
        IExpressionLibraryRegistry _registry;

        public ExpressionBuilder(IExpressionLibraryRegistry registry)
        {
            _registry = registry;
        }

        public ParameterExpression[] GetParameterList()
        {
            var list = new List<ParameterExpression>();

            foreach (var lib in _registry.GetLibraries())
            {
                list.AddRange(lib.GetParameters());
            }

            return list.ToArray();
        }

        public Object[] GetObjects(ICpuContext context)
        {
            var list = new List<Object>();

            foreach (var lib in _registry.GetLibraries())
            {
                list.AddRange(lib.GetObjects(context));
            }

            return list.ToArray();
        }

        public Expression InitializeParameters()
        {
            var expressions = new List<Expression>();

            foreach (var lib in _registry.GetLibraries())
            {
                if (lib.InitializeLocals != null)
                    expressions.Add(lib.InitializeLocals);
            }

            return Expression.Block(expressions);
        }

        public Expression GetParameterizedExpression(Expression body)
        {
            List<Expression> expressions = new List<Expression>();
            expressions.AddRange(GetLocals());
            expressions.Add(InitializeParameters());
            expressions.Add(body);
            expressions.Add(FinalizeParameters());

            return Expression.Block(GetLocals(), expressions);
        }

        public Expression FinalizeParameters()
        {
            var expressions = new List<Expression>();

            foreach (var lib in _registry.GetLibraries())
            {
                if (lib.InitializeLocals != null)
                    expressions.Add(lib.FinalizeLocals);
            }

            return Expression.Block(expressions);
        }

        public Delegate Compile(Expression body)
        {
            var parameterizedBody = GetParameterizedExpression(body);

            var lambda = Expression.Lambda(parameterizedBody,
                    GetParameterList()
               );

            return lambda.Compile();
        }

        public void Invoke(CpuContext context, Expression body)
        {
            Compile(body).DynamicInvoke(GetObjects(context));
        }

        public IEnumerable<ParameterExpression> GetLocals()
        {
            var list = new List<ParameterExpression>();

            foreach (var lib in _registry.GetLibraries())
            {
                list.AddRange(lib.GetLocals());
            }

            return list.ToArray();
        }
    }
}
