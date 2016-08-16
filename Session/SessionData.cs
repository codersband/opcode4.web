using System;
using System.Globalization;
using System.Linq;
using System.Web;
using opcode4.core.Model.Log;
using opcode4.utilities;
using opcode4.web.Membership;

namespace opcode4.web.Session
{
    public class SessionData
    {
        public static void Create(ApplicationUser user)
        {
            if(string.IsNullOrEmpty(user?.UserName))
                throw new ArgumentException(nameof(user.UserName));

            if (user.Roles == null)
                throw new ArgumentException(nameof(user.Roles));

            UserId = user.Id;
            UserName = user.UserName;
            UserRoles = user.Roles?.Select(r=>r.Name).ToArray();
            ProviderId = user.ProviderId;
        }

        public static string SessionId => HttpContext.Current.Session.SessionID;

        public static bool IsInitialized => HttpContext.Current.Session["__userRoles__"] != null;

        public static long UserId
        {
            private set { HttpContext.Current.Session["__userId__"] = value; }
            get { return CommonUtils.Value2Long(HttpContext.Current.Session["__userId__"]); }
        }

        public static string UserName
        {
            private set { HttpContext.Current.Session["__userName__"] = value; }
            get { return CommonUtils.Value2Str(HttpContext.Current.Session["__userName__"]); }
        }

        public static string[] UserRoles
        {
            private set { HttpContext.Current.Session["__userRoles__"] = value; }
            get { return CommonUtils.Value2StrArray(HttpContext.Current.Session["__userRoles__"]); }
        }

        public static bool IsInRole(string role)
        {
            return UserRoles.FirstOrDefault(r => r.Equals(role)) != null;
        }

        public static long ProviderId
        {
            private set { HttpContext.Current.Session["__providerId__"] = value; }
            get { return CommonUtils.Value2Long(HttpContext.Current.Session["__providerId__"]); }
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
