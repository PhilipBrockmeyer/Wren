using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wren.Services.Core.Tosec;
using Wren.Web.Infrastructure;
using Wren.Services.Core;
using System.Globalization;
using Wren.Transport.DataObjects;
using Wren.Data.Entities;

namespace Wren.Web.Controllers
{
    public class GameInfoController : Controller
    {
        //
        // GET: /GameInfo/

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Tosec(String id)
        {
            var svc = new TosecGameInfoService(new SessionManager());
            return Json(svc.GetGameInfo(id), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AchievementDefinitions(String lastUpdated)
        {
            var svc = new AchievementsService(new SessionManager());
            return Json(svc.GetAchievementDefinitions(DateTime.Parse(lastUpdated, CultureInfo.InvariantCulture.DateTimeFormat)), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AchievementState(String id, String lastUpdated)
        {
            if (String.IsNullOrEmpty(id))
                return Redirect("~/Home");

            if (String.IsNullOrEmpty(lastUpdated))
                return Redirect("~/Home");

            var svc = new AchievementsService(new SessionManager());
            return Json(svc.GetAchievementState(id, DateTime.Parse(lastUpdated, CultureInfo.InvariantCulture.DateTimeFormat)), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        // [Authorize]
        public ActionResult AchievementState(FormCollection formCollection)
        {
            //if ((String)Session["UserId"] != (String)formCollection["userid"])
            //    return Json(false);

            var localAchievements = Newtonsoft.Json.JsonConvert.DeserializeObject<AchievementStateDto[]>(formCollection["localachievementstate"]);

            var svc = new AchievementsService(new SessionManager());
            return Json(svc.UploadAchievementState(formCollection["userid"], localAchievements));
        }
    }
}
