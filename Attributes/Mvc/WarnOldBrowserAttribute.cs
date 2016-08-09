using System.Web.Mvc;
using opcode4.utilities;

namespace opcode4.web.Attributes.Mvc
{
    public class WarnOldBrowserAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
            //this will be true when it's their first visit to the site (will happen again if they clear cookies)

            if (request.Browser.Browser.Trim().ToUpperInvariant().Equals("IE") && request.Browser.MajorVersion <= ConfigUtils.ReadIntDef("IE_BROWSER_MIN_VER", 10))
            {
                if (request.Url != null)
                    filterContext.Controller.ViewData["RequestedUrl"] = request.Url.PathAndQuery;

                filterContext.Result = new ViewResult { ViewName = ConfigUtils.ReadStringDef("IE_BROWSER_OLD_VIEW", "InternetExplorerOldWarning") };
            }
        }
    }
}