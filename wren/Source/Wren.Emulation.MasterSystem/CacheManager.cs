using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Emulation.MasterSystem
{
    public class CacheManager
    {
        Int32[] _cache;

        public CacheManager(CpuData cpu)
        {
            _cache = cpu.CpuCache;
        }

        public void InvalidateRegion(Int32 startAddress, Int32 length)
        {
            for (Int32 index = startAddress; index < startAddress + length; index++)
            {
                _cache[index] = -1;
            }
        }
    }
}
