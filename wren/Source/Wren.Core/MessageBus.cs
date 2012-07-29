using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Core
{
    public class MessageBus
    {
        Object _lock = new Object();
        Queue<IEmulatorCommand> _commands;

        public MessageBus()
        {
            _commands = new Queue<IEmulatorCommand>();
        }

        public Boolean HasMessages
        {
            get { return _commands.Count > 0; }
        }

        public void SendCommand(IEmulatorCommand command)
        {
            lock (_lock)
            {
                _commands.Enqueue(command);
            }
        }

        public IEmulatorCommand GetCommand()
        {
            lock (_lock)
            {
                return _commands.Dequeue();
            }
        }
    }
}
