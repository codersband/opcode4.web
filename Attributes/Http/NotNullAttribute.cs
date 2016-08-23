using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using opcode4.core.Exceptions;

namespace opcode4.web.Attributes.Http
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class NotNullAttribute : ActionFilterAttribute
    {
        private readonly Func<Dictionary<string, object>, List<string>> _validate;

        public NotNullAttribute()
            : this(arguments => arguments.Where(a => a.Value == null).Select(a => a.Key).ToList())
        {
        }

        public NotNullAttribute(Func<Dictionary<string, object>, List<string>> checkCondition)
        {
            _validate = checkCondition;
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var args = _validate(actionContext.ActionArguments);
            if (args.Count > 0)
            {
                var s = new StringBuilder(NotNullMessage);
                foreach (var arg in args)
                {
                    s.Append(" " + arg);
                }


                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.BadRequest,
                    new ErrorDataDto((int)ExceptionCode.INVALID_PARAMETER, ValidationErrorMessage, s.ToString()));

            }
        }

        protected virtual string NotNullMessage 
        {
            get { return "NotNullArgument"; }
        }

        protected virtual string ValidationErrorMessage
        {
            get { return "ValidationError"; }
        }
    }
}