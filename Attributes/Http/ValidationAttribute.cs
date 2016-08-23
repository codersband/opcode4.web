using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using opcode4.utilities;

namespace opcode4.web.Attributes.Http
{
    public class ValidateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!actionContext.ModelState.IsValid)
            {
                var request = actionContext.ActionArguments.ToJSON();
                var message = string.Format("{0}\r\nRequest:\r\n{1}", actionContext.ModelState.ToDictionary(item =>
                    item.Key, val => val.Value.Errors.Select(t => t.Exception != null ? t.Exception.Message : t.ErrorMessage).ToList()).ToJSON(), request);

                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, actionContext.ModelState);

                
            }
        }
    }
}
