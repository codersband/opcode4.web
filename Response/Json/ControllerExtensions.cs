using System.Text;
using System.Web.Mvc;

namespace opcode4.web.Response.Json
{
    public static class ControllerExtensions
    {
        public static JsonNetResult JsonNet(this Controller controller, object data)
        {
            return JsonNet(data, null /* contentType */, null /* contentEncoding */, JsonRequestBehavior.DenyGet);
        }

        public static JsonNetResult JsonNet(this Controller controller, object data, string contentType)
        {
            return JsonNet(data, contentType, null /* contentEncoding */, JsonRequestBehavior.DenyGet);
        }

        public static JsonNetResult JsonNet(this Controller controller, object data, string contentType, Encoding contentEncoding)
        {
            return JsonNet(data, contentType, contentEncoding, JsonRequestBehavior.DenyGet);
        }

        public static JsonNetResult JsonNet(this Controller controller, object data, JsonRequestBehavior behavior)
        {
            return JsonNet(data, null /* contentType */, null /* contentEncoding */, behavior);
        }

        public static JsonNetResult JsonNet(this Controller controller, object data, string contentType, JsonRequestBehavior behavior)
        {
            return JsonNet(data, contentType, null /* contentEncoding */, behavior);
        }

       public static JsonNetResult JsonNet(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonNetResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior
            };
        }





    }
}