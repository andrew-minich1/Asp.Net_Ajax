using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;

namespace ReadJsonFile.Authentication
{
    public class CustomAuthenticationModule : IHttpModule
    {
        public void Dispose()
        {

        }

        public void Init(HttpApplication context)
        {
            context.AuthenticateRequest += new EventHandler(this.OnAuthenticate);
            context.EndRequest += new EventHandler(this.OnEndRequest);
        }

        private void OnAuthenticate(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;
            HttpRequest request = app.Request;
            HttpContext context = app.Context;

            onAuthenticateCalled = true;
            if (context.User != null)
            {
                return;
            }

            HttpCookie authCookie = request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                if (string.Empty == authCookie.Value || null == authCookie.Value)
                {
                    context.Request.Cookies.Remove(FormsAuthentication.FormsCookieName);
                    return;
                }
                FormsAuthenticationTicket authTicket = null;
                try
                {
                    authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                }
                catch (ArgumentException)
                {
                    context.Request.Cookies.Remove(FormsAuthentication.FormsCookieName);
                    return;
                    //throw;
                }

                if (authTicket == null)
                    return;

                if (authTicket != null && authTicket.Expired)
                {
                    context.Request.Cookies.Remove(FormsAuthentication.FormsCookieName);
                    return;
                }

                FormsAuthenticationTicket authTicket2 = authTicket;
                if (FormsAuthentication.SlidingExpiration)
                    authTicket2 = FormsAuthentication.RenewTicketIfOld(authTicket);

                CustomPrincipalSerializeModel serializeModel = JsonConvert.DeserializeObject<CustomPrincipalSerializeModel>(authTicket2.UserData);
                CustomPrincipal newUser = new CustomPrincipal(authTicket2.Name);
                newUser.UserId = serializeModel.UserId;
                newUser.Email = serializeModel.Email;
                newUser.roles = serializeModel.roles;
                context.User = newUser;

                if (authTicket2 != authTicket)
                {
                    String strEnc = FormsAuthentication.Encrypt(authTicket2);
                    if (authCookie != null)
                        authCookie = context.Request.Cookies[FormsAuthentication.FormsCookieName];

                    if (authCookie == null)
                    {
                        authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, strEnc);
                        authCookie.Path = authTicket2.CookiePath;
                    }
                    if (authTicket2.IsPersistent)
                        authCookie.Expires = authTicket2.Expiration;
                    authCookie.Value = strEnc;
                    if (FormsAuthentication.CookieDomain != null)
                        authCookie.Domain = FormsAuthentication.CookieDomain;
                    authCookie.HttpOnly = true;
                    context.Response.Cookies.Remove(authCookie.Name);
                    context.Response.Cookies.Add(authCookie);
                }
            }

        }

        private void OnEndRequest(object sender, EventArgs e)
        {
            if (onAuthenticateCalled)
                onAuthenticateCalled = false;
            else
                return;

            HttpApplication app = (HttpApplication)sender;
            HttpContext context = app.Context;

            if (context.Response.StatusCode != 401)
                return;

            String strUrl = context.Request.RawUrl;
            String loginUrl = WebConfigurationManager.AppSettings["loginUrl"];
            if (null == loginUrl || loginUrl.Length < 1)
                loginUrl = "~/Account/Login";
            String strRedirect;
            strRedirect = loginUrl + "?" + "returnUrl" + "=" + HttpUtility.UrlEncode(strUrl, context.Request.ContentEncoding);
            context.Response.Redirect(strRedirect, false);
        }

        private bool onAuthenticateCalled;
    }
}