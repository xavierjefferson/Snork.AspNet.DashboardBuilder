using System.Reflection;
using Snork.AspNet.DashboardBuilder;

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
            Routes.AddRazorPage("/", x => new HomePage()); Routes.AddRazorPage("/ok", x => new HomePage());
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