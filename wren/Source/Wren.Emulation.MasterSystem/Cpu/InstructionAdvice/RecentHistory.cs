using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Wren.Emulation.MasterSystem.ExpressionLibraries;
using System.Reflection;

namespace Wren.Emulation.MasterSystem.InstructionAdvice
{
    public class RecentHistory : IInstructionAdvice
    {
        private const Int32 HistorySize = 50;
        public class HistoryItem
        {
            public String Mnemonic { get; private set; }
            public Int32 ProgramCounter { get; set; }
            public Int32 ParameterByte1 { get; private set; }
            public Int32 ParameterByte2 { get; private set; }

            public HistoryItem(String mnemonic, Int32 programCounter, Int32 parameterByte1, Int32 parameterByte2)
            {
                Mnemonic = mnemonic;
                ProgramCounter = programCounter;
                ParameterByte1 = parameterByte1;
                ParameterByte2 = parameterByte2;
            }

            public override string ToString()
            {
                return String.Format("{0:X4}: {1}", ProgramCounter, Mnemonic);
            }
        }

        public IEnumerable<HistoryItem> Instructions
        {
            get
            {
                return _history;
            }
        }

        Queue<HistoryItem> _history;
        MethodInfo _enqueMethod;
        ConstructorInfo _historyItemConsturctor;

        public RecentHistory()
        {
            _history = new Queue<HistoryItem>(HistorySize + 1);
            _enqueMethod = this.GetType().GetMethod("EnqueHistoryItem");
            _historyItemConsturctor = typeof(HistoryItem).GetConstructor(new Type[] {
                typeof(String),
                typeof(Int32),
                typeof(Int32),
                typeof(Int32)
            });
        }

        public void ClearHistory()
        {
            _history.Clear();
        }

        public Expression GetExpression(InstructionInfo info, IExpressionLibraryRegistry expressionLibraries)
        {
            var instructionSize = 0;

            switch (info.ParameterMode)
            {
                case InstructionParameterMode.None:
                    instructionSize = 1;
                    break;
                case InstructionParameterMode.Byte:
                    instructionSize = 2;
                    break;
                case InstructionParameterMode.Word:
                    instructionSize = 3;
                    break;
                case InstructionParameterMode.Index:
                    instructionSize = 2;
                    break;
                case InstructionParameterMode.Address:
                    instructionSize = 3;
                    break;
                case InstructionParameterMode.IndexAndByte:
                    instructionSize = 3;
                    break;
                default:
                    break;
            }

            if (info.Prefix == "DDCB" || info.Prefix == "FDCB")
            {
                instructionSize++;
            }

            if (!String.IsNullOrEmpty(info.Prefix))
                instructionSize += info.Prefix.Length / 2;

            var progExpr = expressionLibraries.GetLibrary<IProgramControlExpressionLibrary>();
            return Expression.Call(Expression.Constant(this), _enqueMethod,
                Expression.New(_historyItemConsturctor,
                    Expression.Constant(info.Mnemonic),
                    Expression.Subtract(progExpr.ProgramCounterRegister,
                        Expression.Constant(instructionSize)),
                    progExpr.ParameterByte1,
                    progExpr.ParameterByte2)
                );
        }

        public void EnqueHistoryItem(HistoryItem item)
        {
            _history.Enqueue(item);
            if (_history.Count > HistorySize)
                _history.Dequeue();
        }
    }
}
