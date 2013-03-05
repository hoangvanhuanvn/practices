using System.Linq;
using System.Web;

namespace GdNet.Core.Web.Extensions
{
    public static class HttpApplicationExtensions
    {
        public static T GetModule<T>(this HttpApplication application) where T : IHttpModule
        {
            return (T)application.Modules.Cast<IHttpModule>().FirstOrDefault(x => x is T);
        }
    }
}
