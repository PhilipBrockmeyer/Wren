using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Wren.Emulation.MasterSystem.ArithmeticAndLogic;

namespace Wren.Emulation.MasterSystem.ExpressionLibraries
{
    public class FlagLookupValuesExpressionLibrary : IFlagLookupValuesExpressionLibrary
    {
        public Expression InitializeLocals
        {
            get { return null; }
        }

        public Expression FinalizeLocals
        {
            get { return null; }
        }

        public ParameterExpression AdditionFlagsParameter { get; private set; }
        public ParameterExpression AdditionWithCarryFlagsParameter { get; private set; }
        public ParameterExpression IncrementFlagsParameter { get; private set; }
        public ParameterExpression DecrementFlagsParameter { get; private set; }
        public ParameterExpression SignZeroParityFlagsParameter { get; private set; }
        public ParameterExpression SubtractionFlagsParameter { get; private set; }
        public ParameterExpression SubtractionWithCarryFlagsParameter { get; private set; }

        public FlagLookupValuesExpressionLibrary()
        {
            AdditionFlagsParameter = Expression.Variable(typeof(Int32[]), "additionFlags");
            AdditionWithCarryFlagsParameter = Expression.Variable(typeof(Int32[]), "additionWithCarryFlags");
            IncrementFlagsParameter = Expression.Variable(typeof(Int32[]), "incrementFlags");
            DecrementFlagsParameter = Expression.Variable(typeof(Int32[]), "decrementFlags");
            SignZeroParityFlagsParameter = Expression.Variable(typeof(Int32[]), "signZeroParityFlags");
            SubtractionFlagsParameter = Expression.Variable(typeof(Int32[]), "subtractionFlags");
            SubtractionWithCarryFlagsParameter = Expression.Variable(typeof(Int32[]), "subtractionWithCarryFlags");
        }

        public Expression AdditionFlagsCalcultorFlags
        {
            get { return AdditionFlagsParameter; }
        }

        public Expression AdditionWithCarryFlagsCalcultorFlags
        {
            get { return AdditionWithCarryFlagsParameter; }
        }

        public Expression IncrementFlagsCalcultorFlags
        {
            get { return IncrementFlagsParameter; }
        }

        public Expression DecrementFlagsCalcultorFlags
        {
            get { return DecrementFlagsParameter; }
        }
        
        public Expression SignZeroParityCalcultorFlags
        {
            get { return SignZeroParityFlagsParameter; }
        }

        public Expression SubtractionFlagsCalcultorFlags
        {
            get { return SubtractionFlagsParameter; }
        }

        public Expression SubtractionWithCarryFlagsCalcultorFlags
        {
            get { return SubtractionWithCarryFlagsParameter; }
        }

        public IEnumerable<ParameterExpression> GetParameters()
        {
            yield return AdditionFlagsParameter;
            yield return AdditionWithCarryFlagsParameter;
            yield return DecrementFlagsParameter;        
            yield return IncrementFlagsParameter;
            yield return SignZeroParityFlagsParameter;
            yield return SubtractionFlagsParameter;
            yield return SubtractionWithCarryFlagsParameter;
        }

        public IEnumerable<Object> GetObjects(ICpuContext context)
        {
            yield return context.Flags.GetFlags<AdditionFlagsCalculator>();
            yield return context.Flags.GetFlags<AdditionWithCarryFlagsCalculator>();
            yield return context.Flags.GetFlags<DecrementFlagsCalculator>();
            yield return context.Flags.GetFlags<IncrementFlagsCalculator>();
            yield return context.Flags.GetFlags<SignZeroParityFlagsCalculator>();
            yield return context.Flags.GetFlags<SubtractionFlagsCalculator>();
            yield return context.Flags.GetFlags<SubtractionWithCarryFlagsCalculator>();
        }

        public IEnumerable<ParameterExpression> GetLocals()
        {
            return new ParameterExpression[] {};
        }
    }
}
