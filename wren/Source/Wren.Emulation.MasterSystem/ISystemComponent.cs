using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.IO;

namespace Wren.Emulation.MasterSystem
{
    public delegate Int32 RunDelegate(Int32 cycles, ISystemBus systemBus);

    public interface ISystemComponent { }

    public interface IRunnableSystemComponent : ISystemComponent
    {
        RunDelegate GetRunMethod(ISystemBus systemBus);
    }

    public interface IAddressableSystemComponent : ISystemComponent
    {
        void RegisterAddressBlocks(IAddressManager addressManager);
    }

    public interface IPortAddressableSystemComponent : ISystemComponent
    {
        void RegisterPortAddresses(IPortManager portManager);
    }

    public interface IInteruptableSystemComponent : ISystemComponent
    {
        void RegisterInteruptHandlers(IInteruptManager interuptManager);
    }

    public interface IStatefulSystemComponent : ISystemComponent
    {
        void SerializeComponentState(BinaryWriter writer);
        void DeserializeComponentState(BinaryReader reader);
    }
}
