using System;
using System.Linq;
using System.Web.Mvc;
using opcode4.core.Model.Log;
using opcode4.log;
using opcode4.utilities;
using opcode4.wcf.Clients;
using opcode4.web.Response;
using opcode4.web.Response.Json;
using opcode4.web.Session;

namespace opcode4.web.Attributes.Mvc
{
    public class ExceptionHandlingAttribute : HandleErrorAttribute
    {
        public bool WriteToDb { set; get; }

        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception == null) return;

            var httpContext = filterContext.HttpContext;

            var controller = filterContext.RouteData.Values["controller"].ToString();
            var action = filterContext.RouteData.Values["action"].ToString();

            var dbgmsg = ConfigUtils.IsDebugMode ? filterContext.Exception.StackTrace : "";
            var message = $"[WEB.{controller}Controller.{action}]: {filterContext.Exception.Message} {dbgmsg}";

            try
            {
                if (WriteToDb)
                    DBLogger.Error(message);
                else
                    (new LogSvcClient()).Error(message);
            }
            catch (Exception ex)
            {
                TextLogger.Error("Can't write to LogService: " + ex.Message);
                TextLogger.Error(message);
            }

            var acceptType = (httpContext.Request.AcceptTypes != null) ? httpContext.Request.AcceptTypes.FirstOrDefault() : "";
            if (httpContext.Request.IsAjaxRequest() || (!string.IsNullOrEmpty(acceptType) && acceptType.Contains("application/json")))
            {
                filterContext.Result = new JsonNetResult
                    {
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                        Data = new
                            {
                                status = ResponseStatus.ERROR.ToString(),
                                details = ConfigUtils.IsDebugMode ? message : (SessionData.LogLevel == LogEventType.Debug? filterContext.Exception.Message: CannotProcessRequestMessage)
                            }
                    };
                filterContext.ExceptionHandled = true;
            }
            else
            {
                if (ConfigUtils.IsDebugMode)
                    filterContext.Controller.TempData["ErrorMessage"] = message;

                base.OnException(filterContext);
                return;
            }

            filterContext.ExceptionHandled = true;

        }

        protected virtual string CannotProcessRequestMessage
        {
            get { return "CannotProcessRequest"; }
            
        }
    }
}