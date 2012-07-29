using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Transport.DataObjects;
using Wren.Core.Services;

namespace Wren.Module.GameInfoProviders.Tosec
{
    public class TosecService : Service
    {
        public TosecInfoDto GetGameInfo(String md5)
        {
            return base.WebGetById<TosecInfoDto>(md5, "GameInfo/Tosec");
        }
    }
}
