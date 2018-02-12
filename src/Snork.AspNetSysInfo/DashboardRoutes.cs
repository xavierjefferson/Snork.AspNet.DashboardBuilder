using System.Reflection;
using Snork.AspNet.Dashboard.SysInfo.Properties;
using Snork.AspNet.DashboardBuilder;

namespace Snork.AspNet.Dashboard.SysInfo
{
    public class DashboardRoutes : IRouteSource
    {
        static DashboardRoutes()
        {
            Routes = new RouteCollection();
            Routes.AddRazorPage("/", x => new HomePage());
            Routes.AddStringResource("/" + nameof(Resource1.sysinfojs),
                x => new StringResource(Resource1.sysinfojs, "text/javascript"));
            Routes.AddStringResource("/" + nameof(Resource1.default_css),
                x => new StringResource(Resource1.default_css, "text/css"));
        }

        public static RouteCollection Routes { get; }

        public RouteCollection GetRoutes()
        {
            return Routes;
        }

        internal static string GetContentFolderNamespace(string contentFolder)
        {
            return $"{typeof(DashboardRoutes).Namespace}.Content.{contentFolder}";
        }

        internal static string GetContentResourceName(string contentFolder, string resourceName)
        {
            return $"{GetContentFolderNamespace(contentFolder)}.{resourceName}";
        }


        private static Assembly GetExecutingAssembly()
        {
            return typeof(DashboardRoutes).GetTypeInfo().Assembly;
        }
    }
}