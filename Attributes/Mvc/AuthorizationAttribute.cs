using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using opcode4.core.Model.Identity;
using opcode4.web.Membership;
using opcode4.web.Response;
using opcode4.web.Response.Json;
using opcode4.web.Session;

namespace opcode4.web.Attributes.Mvc
{
    public class AuthorizationAttribute : AuthorizeAttribute
    {
        public string RedirectToController { set; get; }
        public string RedirectToAction { set; get; }

        public const string TempDataMessageKey = "AuthKeyErrorMessage";

        public bool AllowRedirect { set; get; } = true;

        public sealed override void OnAuthorization(AuthorizationContext filterContext)
        {
            try
            {
                if (SkipAuthorization(filterContext))
                    return;

                var httpContext = filterContext.HttpContext;
                var authType = httpContext.User.Identity.AuthenticationType;

                if (authType.Equals("NTLM") || authType.Equals("Negotiate"))
                {
                    var identity = new CustomIdentity(0, httpContext.User.Identity.Name, 0, new [] { IdentityRoles.ROOT });
                    HttpContext.Current.User = new CustomPrincipal(identity);

                    if (!httpContext.User.Identity.IsAuthenticated || !SessionData.IsInitialized)
                    {
                        SessionData.Create(new ApplicationUser
                        {
                            UserName = httpContext.User.Identity.Name,
                            Roles = new List<IdentityUserRole> { new IdentityUserRole { Name = IdentityRoles.ROOT } }
                        });

                        filterContext.Result = new RedirectToRouteResult(
                            new RouteValueDictionary(
                                new
                                {
                                    controller = filterContext.RouteData.Values["controller"],
                                    action = filterContext.RouteData.Values["action"]
                                }));
                    }      
                }
                else
                {
                    if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
                        throw new Exception("user_not_authenticated");

                    if (!SessionData.IsInitialized)
                        throw new Exception("authentication_required");

                    var identity = new CustomIdentity(SessionData.UserId, SessionData.UserName,
                        SessionData.ProviderId, SessionData.UserRoles);

                    HttpContext.Current.User = new CustomPrincipal(identity);

                    //var ci = SessionData.Culture;
                    //Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(ci.Name);
                    //Thread.CurrentThread.CurrentUICulture = ci;

                    base.OnAuthorization(filterContext);
                }
            }
            catch (Exception e)
            {
                HandleUnauthorizedRequest(filterContext, e);
            }
        }

        protected virtual bool SkipAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
                throw new ArgumentNullException(nameof(filterContext));

            return filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true)
                || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true);

        }

        protected void HandleUnauthorizedRequest(AuthorizationContext filterContext, Exception ex)
        {
            var httpContext = filterContext.HttpContext;
            var request = httpContext.Request;
            var user = httpContext.User;

            var msg =
                $"PERMISSION_DENIED: User:{user.Identity.Name ?? request.UserHostAddress} does not have permission to perform this action";
            if (ex != null)
                msg = $"PERMISSION_DENIED: User:{user.Identity.Name ?? request.UserHostAddress} - {ex.Message}";

            var acceptType = (httpContext.Request.AcceptTypes != null) ? httpContext.Request.AcceptTypes.FirstOrDefault() : "";
            if (httpContext.Request.IsAjaxRequest() || (!string.IsNullOrEmpty(acceptType) && acceptType.Contains("application/json")))
            {
                filterContext.Result = new JsonNetResult
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = new
                    {
                        status = ResponseStatus.UNAUTHORIZED.ToString(),
                        details = "cannot_process_request"
                    }
                    // Data = new ErrorData((int)ExceptionCode.Security.AuthenticationError, ResponseStatus.UNAUTHORIZED.ToString(), GetMessage("cannot_process_request"))
                };
                //httpContext.Items.Add(SuppressFormsAuthenticationRedirectModule.SuppressFormsAuthenticationKey, "true");
                //response.End();
            }
            else
            {
                filterContext.Result = new ContentResult { Content = msg };
                if (AllowRedirect)
                {
                    filterContext.Controller.TempData[TempDataMessageKey] = msg;
                    base.HandleUnauthorizedRequest(filterContext);
                }
                //filterContext.Result = new HttpUnauthorizedResult(string.Format("PERMISSION_DENIED: User:{0} does not have permission to perform this action",
                //user.Identity.Name ?? request.UserHostAddress));
            }

        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            HandleUnauthorizedRequest(filterContext, null);
        }
    }
}