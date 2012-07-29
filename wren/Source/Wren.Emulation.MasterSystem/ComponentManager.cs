using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Wren.Emulation.MasterSystem
{
    public class ComponentManager
    {
        List<ISystemComponent> _components;
        Dictionary<IRunnableSystemComponent, RunDelegate> _runDelegates;
        IInteruptManager _interuptManager;
        IAddressManager _addressManager;
        IPortManager _portManager;

        public ComponentManager(
            IInteruptManager interuptManager,
            IAddressManager addressManager,
            IPortManager portManager
            )
        {
            _runDelegates = new Dictionary<IRunnableSystemComponent, RunDelegate>();
            _interuptManager = interuptManager;
            _addressManager = addressManager;
            _portManager = portManager;
            _components = new List<ISystemComponent>();
        }

        public void RunFrame(Int32 cycles, ISystemBus systemBus)
        {
            foreach (var runner in _runDelegates.Values)
            {
                runner.Invoke(cycles, systemBus);
            }
        }

        public void AttachComponent(ISystemComponent component)
        {
            _components.Add(component);
        }

        public void DetachComponent(ISystemComponent component)
        {
            _components.Remove(component);

            if (component is IRunnableSystemComponent)
            {
                _runDelegates.Remove((IRunnableSystemComponent)component);
            }
        }

        public void InitializeComponents(ISystemBus systemBus)
        {
            _runDelegates.Clear();
            _addressManager.Clear();
            _interuptManager.Clear();
            _portManager.Clear();
            foreach (var component in _components)
            {
                if (component is IInteruptableSystemComponent)
                {
                    ((IInteruptableSystemComponent)component).RegisterInteruptHandlers(_interuptManager);
                }

                if (component is IAddressableSystemComponent)
                {
                    ((IAddressableSystemComponent)component).RegisterAddressBlocks(_addressManager);
                }

                if (component is IPortAddressableSystemComponent)
                {
                    ((IPortAddressableSystemComponent)component).RegisterPortAddresses(_portManager);
                }
            }

            foreach (var component in _components)
            {
                if (component is IRunnableSystemComponent)
                {
                    _runDelegates.Add((IRunnableSystemComponent)component, ((IRunnableSystemComponent)component).GetRunMethod(systemBus));
                }
            }            
        }

        public void SaveComponentState(BinaryWriter writer)
        {
            foreach (var component in _components)
            {
                if (component is IStatefulSystemComponent)
                {
                    ((IStatefulSystemComponent)component).SerializeComponentState(writer);
                }
            }
        }

        public void LoadComponentState(BinaryReader reader)
        {
            foreach (var component in _components)
            {
                if (component is IStatefulSystemComponent)
                {
                    ((IStatefulSystemComponent)component).DeserializeComponentState(reader);
                }
            }
        }

    }
}
