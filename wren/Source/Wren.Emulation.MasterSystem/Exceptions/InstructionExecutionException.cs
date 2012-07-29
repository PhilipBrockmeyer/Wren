using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Emulation.MasterSystem.Exceptions
{
    [Serializable]
    public class InstructionExecutionException : Exception
    {
        public InstructionExecutionException() { }
        public InstructionExecutionException(string message) : base(message) { }
        public InstructionExecutionException(string message, Exception inner) : base(message, inner) { }
        protected InstructionExecutionException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }

        public IEnumerable<Wren.Emulation.MasterSystem.InstructionAdvice.RecentHistory.HistoryItem> History { get; private set; }
        public CpuData CpuData { get; private set; }

        public InstructionExecutionException(IEnumerable<Wren.Emulation.MasterSystem.InstructionAdvice.RecentHistory.HistoryItem> history, CpuData data, Exception innerException)
            : base("There was an error in cpu execution.", innerException)
        {
            CpuData = data;
            History = history;
        }
    }
}
