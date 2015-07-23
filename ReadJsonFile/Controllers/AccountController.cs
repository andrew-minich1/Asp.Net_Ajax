
using Newtonsoft.Json;
using ReadJsonFile.Authentication;
using ReadJsonFile.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Repository;
using ORM;

namespace ReadJsonFile.Controllers
{
    public class AccountController : Controller
    {
        private IRepository repository;

        public AccountController()
        {
            this.repository = new UserRepository(new UserAccountEntityModel());
        }

        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = repository.GetByLogin(model.Login);
                if (user != null)
                {
                    var roles = user.Roles.Select(m => m.Name).ToArray();

                    CustomPrincipalSerializeModel serializeModel = new CustomPrincipalSerializeModel();
                    serializeModel.UserId = user.Id;
                    serializeModel.Email = user.Email;
                    serializeModel.roles = roles;

                    string userData = JsonConvert.SerializeObject(serializeModel);
                    FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                    1,
                    user.Login,
                    DateTime.Now,
                    DateTime.Now.AddMinutes(15),
                    model.RememberMe,
                    userData);

                    string encTicket = FormsAuthentication.Encrypt(authTicket);
                    HttpCookie faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                    Response.Cookies.Add(faCookie);

                    if (Request.IsAjaxRequest())
                    {
                        return Content(model.Login);
                    }
                    else
                    {
                        if (returnUrl != null)
                        {
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }
                ModelState.AddModelError("", "Incorrect username and/or password");
            }

            return View(model);
        }

        [AllowAnonymous]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Registration(RegistrationViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User()
                {
                    Login = model.UserName,
                    Email = model.UserEmail,
                    CreateDate = DateTime.Now.Date,
                    Password = model.UserPassword,
                };
                repository.Add(user);
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }
    }
}

