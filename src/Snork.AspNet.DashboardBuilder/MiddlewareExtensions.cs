using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Infrastructure;
using Owin;

namespace Snork.AspNet.DashboardBuilder
{
    using MidFunc = Func<
        Func<IDictionary<string, object>, Task>,
        Func<IDictionary<string, object>, Task>>;
    using BuildFunc = Action<
        Func<
            IDictionary<string, object>,
            Func<
                Func<IDictionary<string, object>, Task>,
                Func<IDictionary<string, object>, Task>
            >>>;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class MiddlewareExtensions
    {
        //public static BuildFunc UseDashboard([NotNull] BuildFunc builder, [NotNull] DashboardOptions options,
        //    [NotNull] RouteCollection routes)
        //{
        //    if (builder == null) throw new ArgumentNullException(nameof(builder));
        //    if (options == null) throw new ArgumentNullException(nameof(options));

        //    if (routes == null) throw new ArgumentNullException(nameof(routes));

        //    builder(_ => UseDashboard(options, routes));

        //    return builder;
        //}

        public static IAppBuilder UseDashboard(this IAppBuilder builder, string pathMatch, DashboardOptions options,
            IRouteSource routeSource)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (pathMatch == null) throw new ArgumentNullException(nameof(pathMatch));
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (routeSource == null) throw new ArgumentException(nameof(routeSource));

            SignatureConversions.AddConversions(builder);
            builder.Map("/okok", app => app.Run(async context =>
            {
                await context.Response.WriteAsync("Returning from Map2");
            }));

            builder.Map(pathMatch, subApp => { UseDashboard(options, routeSource.GetRoutes()); });


            return builder;
        }

        private static MidFunc UseDashboard([NotNull] DashboardOptions options, [NotNull] RouteCollection routes)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (routes == null) throw new ArgumentNullException(nameof(routes));

            Func<IDictionary<string, object>, Task> OuterFunc(Func<IDictionary<string, object>, Task> next)
            {
                Task InnerFunc(IDictionary<string, object> env)
                {
                    var owinContext = new OwinContext(env);
                    var context = new OwinDashboardContext(options, env);


                    if (options.Authorization.Any(filter => !filter.Authorize(context)))
                    {
                        return Unauthorized(owinContext);
                    }


                    var findResult = routes.FindDispatcher(owinContext.Request.Path.Value);

                    if (findResult == null)
                    {
                        return next(env);
                    }

                    context.UriMatch = findResult.Item2;

                    return findResult.Item1.Dispatch(context);
                }

                return InnerFunc;
            }

            return
                OuterFunc;
        }

 
        private static Task Unauthorized(IOwinContext owinContext)
        {
            var isAuthenticated = owinContext.Authentication?.User?.Identity?.IsAuthenticated;

            owinContext.Response.StatusCode = isAuthenticated == true
                ? (int) HttpStatusCode.Forbidden
                : (int) HttpStatusCode.Unauthorized;

            return Task.FromResult(0);
        }
    }
}