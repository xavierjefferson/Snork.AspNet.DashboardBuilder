using System;
using System.ComponentModel;
using Owin;
using Snork.AspNet.DashboardBuilder;

namespace Snork.AspNetSysInfo
{
    /// <threadsafety static="true" instance="false" />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class AppBuilderExtensions
    {
        private static void MyDelegate(IAppBuilder app)
        {
            app.Run(async context =>
            {
                await context.Response.WriteAsync("Returning from Map");
            });
        }
        /// <summary>
        ///     Adds Dashboard UI middleware to the OWIN request processing pipeline
        /// </summary>
        /// <param name="builder">OWIN application builder.</param>
        /// <param name="options"></param>
        /// <exception cref="ArgumentNullException"><paramref name="builder" /> is null.</exception>
        /// <remarks>
        ///     Please see <see cref="AppBuilderExtensions" /> for details and examples.
        /// </remarks>
        public static IAppBuilder UseSysInfoDashboard([NotNull] this IAppBuilder builder,
            DashboardOptions options = null)
        {
            
            builder.Map("/MyDelegate", MyDelegate);
            options = options ?? new DashboardOptions();
            var routeSource = new DashboardRoutes();
            return builder.UseDashboard("/sysinfo", options, routeSource);

        }

        
    }
}