using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Wren.Web.Models;
using Wren.Services.Core;
using Wren.Web.Infrastructure;
using Wren.Data;
using Wren.Data.Entities;

namespace Wren.Web.Controllers
{

    [HandleError]
    public class AccountController : Controller
    {

        public IFormsAuthenticationService FormsService { get; set; }
        public IMembershipService MembershipService { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }

            base.Initialize(requestContext);
        }

        // **************************************
        // URL: /Account/LogOn
        // **************************************
        public ActionResult LogOn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogOn(FormCollection values)
        {
            var svc = new AccountService(new SessionManager());

            var result = svc.LogOn(values["username"], values["password"]);

            if (result.IsSuccessful)
            {
                FormsService.SignIn(values["username"], true);
                Session["UserId"] = result.UserId;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // **************************************
        // URL: /Account/LogOff
        // **************************************
        public ActionResult LogOff()
        {
            FormsService.SignOut();

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult Register(FormCollection values)
        {
            var svc = new AccountService(new SessionManager());
            var result = svc.Register(new User() { Id = values["userid"], Username = values["username"], Password = values["password"], EmailAddress = values["email"] });

            if (result)
            {
                FormsService.SignIn(values["username"], true);
                Session["UserId"] = values["userid"];
            }            

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult IsRegistrationValid(FormCollection values)
        {
            var svc = new AccountService(new SessionManager());
            return Json(svc.IsRegistrationValid(values["username"], values["password"], values["email"]), JsonRequestBehavior.AllowGet);
        }

        // **************************************
        // URL: /Account/ChangePassword
        // **************************************
        [Authorize]
        public ActionResult ChangePassword()
        {
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                if (MembershipService.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword))
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            return View(model);
        }

        // **************************************
        // URL: /Account/ChangePasswordSuccess
        // **************************************

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }
    }
}
