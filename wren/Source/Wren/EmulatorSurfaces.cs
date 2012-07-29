using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Interop;

namespace Wren
{
    public class EmulatorSurfaces : Wren.IEmulatorSurfaces
    {
        IDictionary<Guid, InteropBitmap> _surfaces;

        public EmulatorSurfaces()
        {
            _surfaces = new Dictionary<Guid, InteropBitmap>();
        }

        public InteropBitmap GetSurface(Guid emulationRunnerId)
        {
            if (!_surfaces.ContainsKey(emulationRunnerId))
                throw new ApplicationException();

            return _surfaces[emulationRunnerId];
        }

        public void SetSurface(Guid emulationRunnerId, InteropBitmap bitmap)
        {
            _surfaces.Add(emulationRunnerId, bitmap);
        }
    }
}
