using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wren.Web.Infrastructure;
using Wren.Data.Entities;
using Wren.Services.Core;

namespace Wren.Web.Controllers
{
    public class AdminController : Controller
    {
        //
        // GET: /Admin/

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult InitializeTosecData()
        {
            TosecInitialization tosec = new TosecInitialization(new SessionManager());
            tosec.InitializeDatabase();

            return base.Redirect("Home");
        }


        [HttpGet]
        public ActionResult CreateAchievementDefinitions()
        {
            var sm = new SessionManager();

            using (var s = sm.GetSession())
            {

                using (var tx = s.BeginTransaction())
                {
                    AchievementDefinition ad = new AchievementDefinition();
                    ad.Code = @"
ProgramTriggers:

MemoryTriggers:
livesChanged = 0xC001;

JS:

var lives = 10;

function initialize(state)
{
}

function getState()
{
    return lives;
}

function reset()
{
    lives = 11;
}

function livesChanged(val)
{
    lives= val;
}
";
                    ad.Description = "This is a test";
                    ad.Id = Guid.NewGuid().ToString("N");
                    ad.LockedImageUrl = "Lock.png";
                    ad.LongDisplayFormat = "<Something></Something>";
                    ad.RomMd5 = "47d736575f9a22f7490d62c76ed7c933";
                    ad.ShortDisplayFormat = "<TextBlock></TextBlock>";
                    ad.TimeStamp = DateTime.UtcNow;
                    ad.UnlockedImageUrl = "Unlocked.png";
                    ad.Title = "A Winner Is You!";

                    s.SaveOrUpdate(ad);
                    tx.Commit();
                }
            }

            return base.Redirect("Home");
        }
    }
}
