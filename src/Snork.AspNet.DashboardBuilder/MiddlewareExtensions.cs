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


    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class MiddlewareExtensions
    {
        public static IAppBuilder UseDashboard(this IAppBuilder app, string pathMatch, IDashboardOptions options,
            IRouteSource routeSource)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));
            if (pathMatch == null) throw new ArgumentNullException(nameof(pathMatch));
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (routeSource == null) throw new ArgumentException(nameof(routeSource));

            SignatureConversions.AddConversions(app);


            app.Map(pathMatch, subApp =>
            {
                var routes = routeSource.GetRoutes();
                void TempQualifier(Func<IDictionary<string, object>, MidFunc> middleware) => subApp.Use(middleware(subApp.Properties));


                if (routes == null) throw new ArgumentNullException(nameof(routes));

                TempQualifier(_ => UseDashboard(options, routes));
            });
            return app;
        }

        private static MidFunc UseDashboard([NotNull] IDashboardOptions options, [NotNull] RouteCollection routes)
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