using System;
using System.ComponentModel;
using Owin;
using Snork.AspNet.DashboardBuilder;

namespace Snork.AspNet.Dashboard.SysInfo
{
    /// <threadsafety static="true" instance="false" />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class AppBuilderExtensions
    {
        /// <summary>
        ///     Adds Dashboard UI middleware to the OWIN request processing pipeline
        /// </summary>
        /// <param name="builder">OWIN application builder.</param>
        /// <param name="mappedPath">mapped path to the dashboard</param>
        /// <param name="options"></param>
        /// <exception cref="ArgumentNullException"><paramref name="builder" /> is null.</exception>
        /// <remarks>
        ///     Please see <see cref="AppBuilderExtensions" /> for details and examples.
        /// </remarks>
        public static IAppBuilder UseSysInfoDashboard([NotNull] this IAppBuilder builder,
            string mappedPath = "/sysinfo",
            DashboardOptions options = null)
        {
            options = options ?? new DashboardOptions();
            var routeSource = new DashboardRoutes();
            return builder.UseDashboard(mappedPath, options, routeSource);
        }
    }
}