using System;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.Web.Http;
using System.Web.Http.Filters;
using opcode4.core.Exceptions;
using opcode4.log;
using opcode4.utilities;
using opcode4.wcf.Clients;
using opcode4.wcf.Results;

namespace opcode4.web.Attributes.Http
{
    public class ExceptionHandlingAttribute : ExceptionFilterAttribute
    {
        public bool WriteToDb { set; get; }

        public override void OnException(HttpActionExecutedContext context)
        {
            if (context.Exception == null) return;
            var controller = "";

            var values = context.Request.GetRouteData().Values;
            
            if(values.ContainsKey("controller"))
                controller = values["controller"].ToString();

            values = context.Request.GetRouteData().Values;
            var action = context.Request.Method.Method;
            if(values.ContainsKey("action"))
                action = values["action"].ToString();

            var dbgmsg = ConfigUtils.IsDebugMode ? context.Exception.StackTrace : "";
            var message = $"[API.{controller}Controller.{action}]: {context.Exception.Message} {dbgmsg}";

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

            if (context.Exception is CustomException)
            {
                var exception = context.Exception as CustomException;
                context.Response = context.Request.CreateResponse(HttpStatusCode.OK, new ErrorDataDto(exception.ErrorCode, exception.Tag, message));
                return;
            }
            if (context.Exception is FaultException<ServiceFaultResult>)
            {
                var exception = context.Exception as FaultException<ServiceFaultResult>;
                context.Response = context.Request.CreateResponse(HttpStatusCode.OK, new ErrorDataDto(exception.Detail.Code, exception.Detail.CodeName, message));
                return;
            }


            if (context.Exception is HttpResponseException) // input validation error
            {
                var ex = context.Exception as HttpResponseException;
                throw ex;
                //throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                //{
                //    Content = new StringContent(context.Exception.Message),
                //    ReasonPhrase = "Exception"
                //});
                
                //context.Response = context.Request.CreateResponse(ex.Response.StatusCode, new ErrorDataDto(-1, ex.Response.ReasonPhrase, ex.Response.Content.ToString()));
            }

            context.Response = context.Request.CreateResponse(HttpStatusCode.InternalServerError, new ErrorDataDto((int)ExceptionCode.INTERNAL_ERROR, "UnhandledError", message));
            //throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
            //{
            //    Content = new StringContent(context.Exception.Message),
            //    ReasonPhrase = "Exception"
            //});

        }
    }
}