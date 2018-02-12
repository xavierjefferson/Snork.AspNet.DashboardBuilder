using System;
using System.Collections.Generic;

namespace Snork.AspNet.DashboardBuilder
{
    public sealed class OwinDashboardContext : DashboardContext
    {
        public OwinDashboardContext([NotNull] IDashboardOptions options,
            [NotNull] IDictionary<string, object> environment)
            : base(options)
        {
            if (environment == null) throw new ArgumentNullException(nameof(environment));

            Environment = environment;
            Request = new OwinDashboardRequest(environment);
            Response = new OwinDashboardResponse(environment);
        }

        public IDictionary<string, object> Environment { get; }
    }
}