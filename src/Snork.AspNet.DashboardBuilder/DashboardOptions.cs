using System.Collections.Generic;

namespace Snork.AspNet.DashboardBuilder
{
    public interface IDashboardOptions
    {
        /// <summary>
        ///     The path for the Back To Site link. Set to <see langword="null" /> in order to hide the Back To Site link.
        /// </summary>
        string AppPath { get; set; }

        IEnumerable<IDashboardAuthorizationFilter> Authorization { get; set; }
    }

    public class DashboardOptions : IDashboardOptions
    {
        public DashboardOptions()
        {
            AppPath = "/";
            Authorization = new[] { new LocalRequestsOnlyAuthorizationFilter() };
        }

        /// <summary>
        ///     The path for the Back To Site link. Set to <see langword="null" /> in order to hide the Back To Site link.
        /// </summary>
        public string AppPath { get; set; }


        public IEnumerable<IDashboardAuthorizationFilter> Authorization { get; set; }
    }
}