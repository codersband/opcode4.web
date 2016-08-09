using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using opcode4.core.Exceptions;
using opcode4.core.Model.Identity;
using opcode4.web.Session;

namespace opcode4.web.Attributes.Http
{
    public class AuthorizationAttribute : ActionFilterAttribute
    {
        public string RedirectToController { set; get; }
        public string RedirectToAction { set; get; }

        public string Roles { set; get; }

        public sealed override void OnActionExecuting(HttpActionContext actionContext)
        {
            try
            {
                if (SkipAuthorization(actionContext))
                    return;

                var httpContext = HttpContext.Current;
                var authType = httpContext.User.Identity.AuthenticationType;
                if (authType.Equals("NTLM") || authType.Equals("Negotiate"))
                {
                    var identity = new CustomIdentity(0, httpContext.User.Identity.Name, 0, new string[0]);
                    HttpContext.Current.User = Thread.CurrentPrincipal = new CustomPrincipal(identity);
                }
                else
                {
                    if (!httpContext.User.Identity.IsAuthenticated)
                        throw new Exception("user_not_authenticated");

                    if (!SessionData.IsInitialized)
                        throw new Exception("authentication_required");

                    var identity = new CustomIdentity(SessionData.UserId, SessionData.UserName, SessionData.ProviderId, SessionData.UserRoles);

                    Thread.CurrentPrincipal = new CustomPrincipal(identity);
                    HttpContext.Current.User = Thread.CurrentPrincipal;

                    //var ci = SessionData.Culture;
                    //Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(ci.Name);
                    //Thread.CurrentThread.CurrentUICulture = ci;

                    var isAllowed = Roles == null || Roles.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Any(role => Thread.CurrentPrincipal.IsInRole(role));

                    if (!isAllowed)
                    {
                        actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden,
                        new { errorId = (int)ExceptionCode.ACCESS_DENIED, reason = "FORBIDDEN", description = "does_not_have_permission_action" });
                    }

                    base.OnActionExecuting(actionContext);
                }
            }
            catch (Exception)
            {
                HandleUnauthorizedRequest(actionContext);
            }
        }

        protected static bool SkipAuthorization(HttpActionContext actionContext)
        {
            if (actionContext == null)
                throw new Exception("actionContext");

            return actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any()
                   || actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();
        }

        protected void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized,
                    new { errorId = (int)ExceptionCode.AUTHENTICATION_ERROR,
                        reason = "AUTHENTICATION REQUIRED", description = "must_be_authenticated"});
        }
    }
}