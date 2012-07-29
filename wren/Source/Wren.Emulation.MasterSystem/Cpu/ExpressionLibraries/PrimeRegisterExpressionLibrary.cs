using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Wren.Emulation.MasterSystem.Properties;

namespace Wren.Emulation.MasterSystem.ExpressionLibraries
{
    public class PrimeRegisterExpressionLibrary : IPrimeRegistersExpressionLibrary
    {
        public ParameterExpression CpuParameter { get; private set; }
        public ParameterExpression RegisterAPrimeParameter { get; private set; }
        public ParameterExpression RegisterFPrimeParameter { get; private set; }
        public ParameterExpression RegisterBPrimeParameter { get; private set; }
        public ParameterExpression RegisterCPrimeParameter { get; private set; }
        public ParameterExpression RegisterDPrimeParameter { get; private set; }
        public ParameterExpression RegisterEPrimeParameter { get; private set; }
        public ParameterExpression RegisterHPrimeParameter { get; private set; }
        public ParameterExpression RegisterLPrimeParameter { get; private set; }

        public PrimeRegisterExpressionLibrary()
        {
            CpuParameter = Expression.Variable(typeof(CpuData), "cpu");
            RegisterAPrimeParameter = Expression.Variable(typeof(Int32), "registerAPrime");
            RegisterFPrimeParameter = Expression.Variable(typeof(Int32), "registerFPrime");
            RegisterBPrimeParameter = Expression.Variable(typeof(Int32), "registerBPrime");
            RegisterCPrimeParameter = Expression.Variable(typeof(Int32), "registerCPrime");
            RegisterDPrimeParameter = Expression.Variable(typeof(Int32), "registerDPrime");
            RegisterEPrimeParameter = Expression.Variable(typeof(Int32), "registerEPrime");
            RegisterHPrimeParameter = Expression.Variable(typeof(Int32), "registerHPrime");
            RegisterLPrimeParameter = Expression.Variable(typeof(Int32), "registerLPrime");
        }

        public Expression RegisterAPrime
        {
            get { return RegisterAPrimeParameter; }
        }

        public Expression RegisterFPrime
        {
            get { return RegisterFPrimeParameter; }
        }

        public Expression RegisterBPrime
        {
            get { return RegisterBPrimeParameter; }
        }

        public Expression RegisterCPrime
        {
            get { return RegisterCPrimeParameter; }
        }

        public Expression RegisterDPrime
        {
            get { return RegisterDPrimeParameter; }
        }

        public Expression RegisterEPrime
        {
            get { return RegisterEPrimeParameter; }
        }

        public Expression RegisterLPrime
        {
            get { return RegisterLPrimeParameter; }
        }

        public Expression RegisterHPrime
        {
            get { return RegisterHPrimeParameter; }
        }

        public Expression InitializeLocals
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(RegisterAPrimeParameter, Expression.Field(CpuParameter, FieldNames.CpuData_RegisterAPrime)),
                    Expression.Assign(RegisterFPrimeParameter, Expression.Field(CpuParameter, FieldNames.CpuData_RegisterFPrime)),
                    Expression.Assign(RegisterBPrimeParameter, Expression.Field(CpuParameter, FieldNames.CpuData_RegisterBPrime)),
                    Expression.Assign(RegisterCPrimeParameter, Expression.Field(CpuParameter, FieldNames.CpuData_RegisterCPrime)),
                    Expression.Assign(RegisterDPrimeParameter, Expression.Field(CpuParameter, FieldNames.CpuData_RegisterDPrime)),
                    Expression.Assign(RegisterEPrimeParameter, Expression.Field(CpuParameter, FieldNames.CpuData_RegisterEPrime)),
                    Expression.Assign(RegisterHPrimeParameter, Expression.Field(CpuParameter, FieldNames.CpuData_RegisterHPrime)),
                    Expression.Assign(RegisterLPrimeParameter, Expression.Field(CpuParameter, FieldNames.CpuData_RegisterLPrime))
                );
            }
        }

        public Expression FinalizeLocals
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(Expression.Field(CpuParameter, FieldNames.CpuData_RegisterAPrime), RegisterAPrimeParameter),
                    Expression.Assign(Expression.Field(CpuParameter, FieldNames.CpuData_RegisterFPrime), RegisterFPrimeParameter),
                    Expression.Assign(Expression.Field(CpuParameter, FieldNames.CpuData_RegisterBPrime), RegisterBPrimeParameter),
                    Expression.Assign(Expression.Field(CpuParameter, FieldNames.CpuData_RegisterCPrime), RegisterCPrimeParameter),
                    Expression.Assign(Expression.Field(CpuParameter, FieldNames.CpuData_RegisterDPrime), RegisterDPrimeParameter),
                    Expression.Assign(Expression.Field(CpuParameter, FieldNames.CpuData_RegisterEPrime), RegisterEPrimeParameter),
                    Expression.Assign(Expression.Field(CpuParameter, FieldNames.CpuData_RegisterHPrime), RegisterHPrimeParameter),
                    Expression.Assign(Expression.Field(CpuParameter, FieldNames.CpuData_RegisterLPrime), RegisterLPrimeParameter)
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
            yield return RegisterAPrimeParameter;
            yield return RegisterFPrimeParameter;
            yield return RegisterBPrimeParameter;
            yield return RegisterCPrimeParameter;
            yield return RegisterDPrimeParameter;
            yield return RegisterEPrimeParameter;
            yield return RegisterHPrimeParameter;
            yield return RegisterLPrimeParameter;
        }
    }
}
