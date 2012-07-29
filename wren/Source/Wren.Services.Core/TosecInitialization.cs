using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;
using Wren.Services.Core;
using Wren.Data;
using Wren.Data.Entities;

namespace Wren.Services.Core
{
    public class TosecInitialization
    {
        ISessionManager _sessionManager;

        static String NesData;
        static String SmsData;

        static TosecInitialization()
        {
            LoadGameData("Wren.Services.Core.Data.Nintendo Famicom & Entertainment System - Games - [NES] (TOSEC-v2007-02-14_CM).dat", "NES");
            LoadGameData("Wren.Services.Core.Data.Sega Mark III & Master System - Games (TOSEC-v2009-12-10_CM).dat", "SMS");
        }

        public TosecInitialization(ISessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        private static void LoadGameData(String resource, String system)
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
            {
                using (var reader = new StreamReader(stream))
                {
                    switch (system)
                    {
                        case "NES":
                            NesData = reader.ReadToEnd();
                            break;

                        case "SMS":
                            SmsData = reader.ReadToEnd();
                            break;
                    }
                }
            }
        }

        public void InitializeDatabase()
        {
            LoadGameInfo("NES");
            LoadGameInfo("SMS");
        }

        private void LoadGameInfo(String system)
        {
            String data = String.Empty;

            switch (system)
            {
                case "NES":
                    data = NesData;
                    break;

                case "SMS":
                    data = SmsData;
                    break;
            }

            MatchCollection matches = Regex.Matches(data, @"game \([\s]*?name \""(?<name>.*?)\s*?(Rev)?\s*?(PRG[0-9]+)?\s*?\((?<year>[0-9x]+).*?\)\((?<publisher>[^\)]*)\)(\((?<country>(US)|(Jp)|(US-Jp)|(Jp-US))\))?(.*)\""");
            List<String> usedIds = new List<String>();

            using (var session = _sessionManager.GetSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    foreach (Match match in matches)
                    {
                        var info = new TosecInfo();

                        Match md5Match;
                        md5Match = Regex.Match(data.Substring(match.Index), @".*md5 (?<md5>[0-9a-f]+)", RegexOptions.Multiline);

                        info.Id = md5Match.Groups["md5"].Value;

                        if (usedIds.Contains(info.Id))
                            continue;

                        usedIds.Add(info.Id);

                        info.Name = match.Groups["name"].Value;
                        info.Year = match.Groups["year"].Value;
                        info.Publisher = match.Groups["publisher"].Value;
                        info.Country = match.Groups["country"].Value;
                        info.System = system;

                        session.SaveOrUpdate(info);
                    }

                    tx.Commit();
                }
            }
        }
    }
}
