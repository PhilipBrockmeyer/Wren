using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Emulation.MasterSystem
{
    public delegate void InteruptDelegate();

    public interface IInteruptManager
    {
        void RegisterInteruptHandler(Interupts interupt, InteruptDelegate handler);
        void RunInteruptHandlers(Interupts interupt);

        void Clear();
    }
}
