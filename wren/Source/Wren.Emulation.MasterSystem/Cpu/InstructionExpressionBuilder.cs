using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Emulation.MasterSystem.ExpressionLibraries;
using System.Linq.Expressions;
using Wren.Emulation.MasterSystem.Exceptions;

namespace Wren.Emulation.MasterSystem
{
    public class InstructionExpressionBuilder : IInstructionExpressionBuilder
    {
        List<IInstructionAdvice> _preInstructionAdvice;
        List<IInstructionAdvice> _postInstructionAdvice;

        IProgramControlExpressionLibrary _programControlExpressionLibrary;
        IExpressionLibraryRegistry _expressionLibrary;

        public InstructionExpressionBuilder(IExpressionLibraryRegistry expressionLibrary)
        {
            _expressionLibrary = expressionLibrary;
            _programControlExpressionLibrary = expressionLibrary.GetLibrary<IProgramControlExpressionLibrary>();
            _preInstructionAdvice = new List<IInstructionAdvice>();
            _postInstructionAdvice = new List<IInstructionAdvice>();
        }

        public void RegisterPreInstructionAdvice(IInstructionAdvice advice)
        {
            _preInstructionAdvice.Add(advice);
        }

        public void RegisterPostInstructionAdvice(IInstructionAdvice advice)
        {
            _postInstructionAdvice.Add(advice);
        }

        public Expression BuildExpression(InstructionInfo info)
        {
            if (info.Body == null)
                throw new NullExpressionException();

            var expressions = new List<Expression>();

            switch (info.ParameterMode)
            {
                case InstructionParameterMode.None:
                    break;
                case InstructionParameterMode.Byte:
                    expressions.Add(Expression.Assign(_programControlExpressionLibrary.ParameterByte1, _programControlExpressionLibrary.ReadAndIncrementProgramCounter));                    
                    break;

                case InstructionParameterMode.Index:
                    // index - (index & 0x80) << 1     -  Handles negative indexes.
                    expressions.Add(Expression.Assign(_programControlExpressionLibrary.ParameterByte2, _programControlExpressionLibrary.ReadAndIncrementProgramCounter));
                    expressions.Add(Expression.Assign(_programControlExpressionLibrary.ParameterByte1,
                            Expression.Subtract(
                                _programControlExpressionLibrary.ParameterByte2,
                                Expression.LeftShift(
                                    Expression.And(_programControlExpressionLibrary.ParameterByte2, Expression.Constant(0x80)), Expression.Constant(0x01)
                                )
                            )
                        )
                    );
                    break;

                case InstructionParameterMode.IndexAndByte:
                    // index - (index & 0x80) << 1     -  Handles negative indexes.
                    expressions.Add(Expression.Assign(_programControlExpressionLibrary.ParameterByte2, _programControlExpressionLibrary.ReadAndIncrementProgramCounter));
                    expressions.Add(Expression.Assign(_programControlExpressionLibrary.ParameterByte1,
                            Expression.Subtract(
                                _programControlExpressionLibrary.ParameterByte2,
                                Expression.LeftShift(
                                    Expression.And(_programControlExpressionLibrary.ParameterByte2, Expression.Constant(0x80)), Expression.Constant(0x01)
                                )
                            )
                        )
                    );

                    expressions.Add(Expression.Assign(_programControlExpressionLibrary.ParameterByte2, _programControlExpressionLibrary.ReadAndIncrementProgramCounter));
                    break;

                case InstructionParameterMode.Word:
                case InstructionParameterMode.Address:
                    expressions.Add(Expression.Assign(_programControlExpressionLibrary.ParameterByte2, _programControlExpressionLibrary.ReadAndIncrementProgramCounter));
                    expressions.Add(Expression.Assign(_programControlExpressionLibrary.ParameterByte1, _programControlExpressionLibrary.ReadAndIncrementProgramCounter));

                    break;
                default:
                    break;
            }

            foreach (var advice in _preInstructionAdvice)
            {
                var a = advice.GetExpression(info, _expressionLibrary);

                if (a != null)
                    expressions.Add(a);
            }

            expressions.Add(info.Body);

            foreach (var advice in _postInstructionAdvice)
            {
                var a = advice.GetExpression(info, _expressionLibrary);

                if (a != null)
                    expressions.Add(a);
            }

            if (_postInstructionAdvice.Count > 0)
            {
                expressions.Add(Expression.Assign(_programControlExpressionLibrary.CycleCounter, _programControlExpressionLibrary.CycleCounter));
            }

            if (info.Cycles.HasValue)
            {
                expressions.Add(
                    Expression.SubtractAssign(
                        _programControlExpressionLibrary.CycleCounter,
                        Expression.Constant(info.Cycles)
                    )
                );
            }

            return Expression.Block(expressions.ToArray());
        }
    }
}
