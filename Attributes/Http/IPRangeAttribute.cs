using System;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using opcode4.core.Exceptions;
using opcode4.utilities;

namespace opcode4.web.Attributes.Http
{
    /// <summary>
    /// There is constraint by IP range
    /// <para>Read reanges from config or from Ranges property</para>
    /// </summary>
    public class IPRangeAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Optionally represents allowed IP ranges. By default read from config by key: IP_CRITERION
        /// <para>Example:  IP:[212.143.244.194,212.143.244.201,192.168.10.1-192.168.10.2],INVERT:[False]</para> 
        /// </summary>
        public string Ranges { set; get; }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            string ip = string.Empty;
            try
            {
                var httpContext = HttpContext.Current;

                var ranges = Ranges;
                if (string.IsNullOrEmpty(ranges))
                    ranges = ConfigUtils.ReadString("IP_CRITERION");


                ip = GetIPAddress(httpContext.Request);
                if (ip == null)
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden,
                        new ErrorDataDto((int)ExceptionCode.ACCESS_DENIED, "FORBIDDEN", "ip_not_recognized"));
                else
                {
                    var proxyIP = IPAddress.Parse(ip);
                    if (!proxyIP.IsInRange(ranges))
                    {
                        actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden,
                        new ErrorDataDto((int)ExceptionCode.ACCESS_DENIED, "FORBIDDEN",
                            $"[{ip}] {"ip_not_allowed"}"));
                    }

                }
            }
            catch (Exception)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden,
                    new ErrorDataDto((int)ExceptionCode.ACCESS_DENIED, "FORBIDDEN",
                        $"[{ip}] {"ip_validation_error"}"));
            }
        }

        protected static string GetIPAddress(HttpRequest request)
        {
            string ip;
            try
            {
                ip = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (!string.IsNullOrEmpty(ip))
                {
                    if (ip.IndexOf(",", StringComparison.Ordinal) > 0)
                    {
                        var ipRange = ip.Split(',');
                        var le = ipRange.Length - 1;
                        ip = le < 0 ? null : ipRange[le];
                    }
                }
                else
                {
                    ip = request.ServerVariables["REMOTE_ADDR"];
                }
            }
            catch { ip = null; }

            return ip;
        }
    }

}