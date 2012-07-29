using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Core.Services
{
    public interface IEncryptionService
    {
        String EncryptString(String inputString);
    }
}
