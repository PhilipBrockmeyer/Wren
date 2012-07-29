using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Wren.Emulation.MasterSystem.Properties;

namespace Wren.Emulation.MasterSystem.ExpressionLibraries
{
    public class InteruptExpressionLibrary : IInteruptExpressionLibrary
    {
        public ParameterExpression CpuParameter { get; private set; }
        public ParameterExpression IsHaltedParameter { get; private set; }
        public ParameterExpression InteruptVectorParameter { get; private set; }
        public ParameterExpression IFF1Parameter { get; private set; }
        public ParameterExpression IFF2Parameter { get; private set; }
        public ParameterExpression InteruptModeParameter { get; private set; }
        public ParameterExpression InteruptRequestedParameter { get; private set; }
        public ParameterExpression RefreshRegisterParameter { get; private set; }

        public InteruptExpressionLibrary()
        {
            CpuParameter = Expression.Variable(typeof(CpuData), "cpu");
            IsHaltedParameter = Expression.Variable(typeof(Boolean), "isHalted");
            InteruptVectorParameter = Expression.Variable(typeof(Int32), "InteruptVector");
            IFF1Parameter = Expression.Variable(typeof(Boolean), "iff1");
            IFF2Parameter = Expression.Variable(typeof(Boolean), "iff2");
            InteruptModeParameter = Expression.Variable(typeof(Int32), "interuptMode");
            InteruptRequestedParameter = Expression.Variable(typeof(Boolean), "interuptRequested");
            RefreshRegisterParameter = Expression.Variable(typeof(Int32), "refreshRegisterParameter");
        }

        public Expression IsHalted
        {
            get { return IsHaltedParameter; }
        }

        public Expression InteruptVectorRegister
        {
            get { return InteruptVectorParameter; }
        }

        public Expression IFF1
        {
            get { return IFF1Parameter; }
        }

        public Expression IFF2
        {
            get { return IFF2Parameter; }
        }

        public Expression InteruptMode
        {
            get { return InteruptModeParameter; }
        }

        public Expression InteruptRequested
        {
            get { return InteruptRequestedParameter; }
        }

        public Expression RefreshRegister
        {
            get { return RefreshRegisterParameter; }
        }

        public Expression InitializeLocals
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(IsHaltedParameter, Expression.Field(CpuParameter, FieldNames.CpuData_IsHalted)),
                    Expression.Assign(InteruptVectorParameter, Expression.Field(CpuParameter, FieldNames.CpuData_InteruptVectorRegister)),
                    Expression.Assign(IFF1Parameter, Expression.Field(CpuParameter, FieldNames.CpuData_IFF1)),
                    Expression.Assign(IFF2Parameter, Expression.Field(CpuParameter, FieldNames.CpuData_IFF2)),
                    Expression.Assign(InteruptModeParameter, Expression.Field(CpuParameter, FieldNames.CpuData_InteruptMode)),
                    Expression.Assign(InteruptRequestedParameter, Expression.Field(CpuParameter, FieldNames.CpuData_InteruptRequested)),
                    Expression.Assign(RefreshRegisterParameter, Expression.Field(CpuParameter, FieldNames.CpuData_RefreshRegister))
                );
            }
        }

        public Expression FinalizeLocals
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(Expression.Field(CpuParameter, FieldNames.CpuData_IsHalted), IsHaltedParameter),
                    Expression.Assign(Expression.Field(CpuParameter, FieldNames.CpuData_InteruptVectorRegister), InteruptVectorParameter),
                    Expression.Assign(Expression.Field(CpuParameter, FieldNames.CpuData_IFF1), IFF1Parameter),
                    Expression.Assign(Expression.Field(CpuParameter, FieldNames.CpuData_IFF2), IFF2Parameter),
                    Expression.Assign(Expression.Field(CpuParameter, FieldNames.CpuData_InteruptMode), InteruptModeParameter),
                    Expression.Assign(Expression.Field(CpuParameter, FieldNames.CpuData_InteruptRequested), InteruptRequestedParameter),
                    Expression.Assign(Expression.Field(CpuParameter, FieldNames.CpuData_RefreshRegister), RefreshRegisterParameter)                    
                );
            }
        }

        public IEnumerable<ParameterExpression> GetParameters()
        {
            yield return CpuParameter;
        }

        public IEnumerable<Object> GetObjects(ICpuContext context)
        {
            yield return context.Data;
        }

        public IEnumerable<ParameterExpression> GetLocals()
        {
            yield return IsHaltedParameter;
            yield return InteruptVectorParameter;
            yield return IFF1Parameter;
            yield return IFF2Parameter;
            yield return InteruptModeParameter;
            yield return InteruptRequestedParameter;
            yield return RefreshRegisterParameter;
        }
    }
}
