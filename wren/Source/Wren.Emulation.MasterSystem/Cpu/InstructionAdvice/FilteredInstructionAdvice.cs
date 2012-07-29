using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Wren.Emulation.MasterSystem.InstructionAdvice
{
    public abstract class FilteredInstructionAdvice : IInstructionAdvice
    {
        protected abstract Expression GetExpressionImpl(InstructionInfo info, IExpressionLibraryRegistry expressionLibraries);

        public Boolean IsFilterEnabled { get; set; }

        List<String> _enabledOpcodes;

        public FilteredInstructionAdvice()
        {
            IsFilterEnabled = true;
            _enabledOpcodes = new List<String>();
        }

        public void EnableOpcode(String opcode)
        {
            _enabledOpcodes.Add(opcode);
        }

        public Expression GetExpression(InstructionInfo info, IExpressionLibraryRegistry expressionLibraries)
        {
            String opcode = info.Prefix + info.Opcode.ToString("X2");
            if (!_enabledOpcodes.Contains(opcode) && IsFilterEnabled)
                return null;

            return GetExpressionImpl(info, expressionLibraries);
        }        
    }
}
