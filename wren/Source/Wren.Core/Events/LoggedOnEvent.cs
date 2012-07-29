using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Core;

namespace Wren.Core.Events
{
    public class LoggedOnEvent : IEvent
    {
        public String UserId { get; private set; }
        
        public LoggedOnEvent(String userId)
        {
            UserId = userId;
        }
    }
}
