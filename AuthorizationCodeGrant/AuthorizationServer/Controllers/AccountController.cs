using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace AuthorizationServer.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Login()
        {
            var authentication = HttpContext.GetOwinContext().Authentication;
            if (Request.HttpMethod == "POST")
            {
                if (!string.IsNullOrEmpty(Request.Form.Get("submit.Signin")))
                {
                    var username = Request.Form["username"];
                    var pass = Request.Form["password"];
                    // ここでログインしていいユーザーかどうかチェックします

                    // OKであればSignIn()
                    authentication.SignIn(
                        new ClaimsIdentity(new[] { new Claim(ClaimsIdentity.DefaultNameClaimType, username) }, "Application"));
                }
            }

            return View();
        }

    }
}