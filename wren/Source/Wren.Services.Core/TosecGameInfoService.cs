using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;
using Wren.Data;
using Wren.Data.Entities;

namespace Wren.Services.Core.Tosec
{
    public class TosecGameInfoService
    {
        ISessionManager _sessionManager;

        public TosecGameInfoService(ISessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        public Wren.Transport.DataObjects.TosecInfoDto GetGameInfo(String romMd5)
        {
            try
            {
                using (var session = _sessionManager.GetSession())
                {
                    var data = session.Load<TosecInfo>(romMd5);

                    if (data == null)
                        return null;

                    var result = new Wren.Transport.DataObjects.TosecInfoDto()
                    {
                        Country = data.Country,
                        Id = data.Id,
                        Name = data.Name,
                        Publisher = data.Publisher,
                        System = data.System,
                        Year = data.Year
                    };

                    return result;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
