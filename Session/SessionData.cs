using System.Globalization;
using System.Linq;
using System.Web;
using opcode4.core.Model;
using opcode4.utilities;
using opcode4.web.Membership;

namespace opcode4.web.Session
{
    public class SessionData
    {
        public static void Create(ApplicationUser user)
        {
            UserId = user.Id;
            UserName = user.UserName;
            UserRoles = user.Roles?.Select(r=>r.Name).ToArray();
            ProviderId = user.ProviderId;
        }

        public static string SessionId => HttpContext.Current.Session.SessionID;

        public static bool IsInitialized => HttpContext.Current.Session["_ActorId"] != null;

        public static ulong UserId
        {
            set { HttpContext.Current.Session["_ActorId"] = value; }
            get { return CommonUtils.Value2uLong(HttpContext.Current.Session["_ActorId"]); }
        }

        public static string UserName
        {
            private set { HttpContext.Current.Session["_ActorName"] = value; }
            get { return CommonUtils.Value2Str(HttpContext.Current.Session["_ActorName"]); }
        }

        public static string[] UserRoles
        {
            private set { HttpContext.Current.Session["UserRoles"] = value; }
            get { return CommonUtils.Value2StrArray(HttpContext.Current.Session["UserRoles"]); }
        }

        public static ulong ProviderId
        {
            private set { HttpContext.Current.Session["_ProviderId"] = value; }
            get { return CommonUtils.Value2uLong(HttpContext.Current.Session["_ProviderId"]); }
        }

        public static CultureInfo Culture
        {
            set { HttpContext.Current.Session["Culture"] = value; }
            get
            {
                var c = HttpContext.Current.Session["Culture"];
                if (c != null)
                    return (CultureInfo) c;
                return new CultureInfo(ConfigUtils.ReadStringDef("DEFAULT_CULTURE", IPInfo.IsIsraelLocation(
                    HttpContext.Current.Request.UserHostAddress)
                    ? "he-il"
                    : "en-us"));
            }
        }

        public static LogEventType LogLevel
        {
            set { HttpContext.Current.Session["LogLevel"] = value; }
            get
            {
                var ll = HttpContext.Current.Session["LogLevel"];
                if (ll == null)
                    return LogEventType.CriticalError;
                return (LogEventType) ll;
            }
        }

        public static void Set<T>(string key, T obj)
        {
            HttpContext.Current.Session[key] = obj;
        }

        public static T Get<T>(string key)
        {
            var t = HttpContext.Current.Session[key];
            if (t == null)
                return default(T);
            return (T) t;
        }

        public static void Abandon()
        {
            //HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.Abandon();
        }
    }
}
