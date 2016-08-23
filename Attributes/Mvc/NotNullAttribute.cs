using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using opcode4.core.Exceptions;
using opcode4.web.Response;
using opcode4.web.Response.Json;

namespace opcode4.web.Attributes.Mvc
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class NotNullAttribute : ActionFilterAttribute
    {
        private readonly Func<IDictionary<string, object>, List<string>> _validate;

        public NotNullAttribute()
            : this(arguments => arguments.Where(a => a.Value == null).Select(a => a.Key).ToList())
        {
        }

        public NotNullAttribute(Func<IDictionary<string, object>, List<string>> checkCondition)
        {
            _validate = checkCondition;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var args = _validate(filterContext.ActionParameters);
            if (args.Count > 0)
            {
                var s = new StringBuilder(NotNullMessage);
                foreach (var arg in args)
                {
                    s.Append(" " + arg);
                }
                
                filterContext.Result = new JsonNetResult
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = new
                    {
                        status = ResponseStatus.ERROR.ToString(),
                        details = new ErrorDataDto((int) ExceptionCode.INVALID_PARAMETER, ValidationErrorMessage, s.ToString())
                    }
                };
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