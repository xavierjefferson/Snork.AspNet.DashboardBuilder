using System;
using System.Text.RegularExpressions;

namespace Snork.AspNet.DashboardBuilder
{
    public abstract class DashboardContext
    {
        protected DashboardContext([NotNull] IDashboardOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));


            Options = options;
        }


        public IDashboardOptions Options { get; }

        public Match UriMatch { get; set; }

        public DashboardRequest Request { get; protected set; }
        public DashboardResponse Response { get; protected set; }
    }
}