using System;
namespace Wren.Emulation.MasterSystem
{
    public interface IInstructionScanner
    {
        InstructionInfo[] BuildInstructionInfo(System.Collections.Generic.IEnumerable<Type> scannedTypes);
    }
}
