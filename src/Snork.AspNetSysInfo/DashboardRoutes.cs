using System.Reflection;
using Snork.AspNet.DashboardBuilder;
using Snork.AspNetSysInfo.Properties;

namespace Snork.AspNetSysInfo
{
    public class DashboardRoutes : IRouteSource
    {
        private static readonly string[] Javascripts =
        {
            "jquery-2.1.4.min.js",
            "bootstrap.min.js",
            "moment.min.js",
            "moment-with-locales.min.js",
            "d3.min.js",
            "d3.layout.min.js",
            "rickshaw.min.js",
            "hangfire.js"
        };

        private static readonly string[] Stylesheets =
        {
            "bootstrap.min.css",
            "rickshaw.min.css",
            "hangfire.css"
        };

        static DashboardRoutes()
        {
            Routes = new RouteCollection();
            Routes.AddRazorPage("/", x => new HomePage());
            Routes.AddRazorPage("/ok", x => new HomePage());
            Routes.AddStringResource("/" + nameof(Resource1.kendo_all_min) , x => new StringResource(Resource1.kendo_all_min, "text/javascript"));
            Routes.AddStringResource("/" + nameof(Resource1.kendo_common_min), x => new StringResource(Resource1.kendo_common_min, "text/css"));
            Routes.AddStringResource("/" + nameof(Resource1.jquery_2_2_4_min), x => new StringResource(Resource1.jquery_2_2_4_min, "text/javascript"));
            Routes.AddStringResource("/" + nameof(Resource1.sysinfojs), x => new StringResource(Resource1.sysinfojs, "text/javascript"));

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