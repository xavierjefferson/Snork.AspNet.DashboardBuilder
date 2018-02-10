using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Snork.AspNet.DashboardBuilder
{
    internal class StringResourceDispatcher : IDashboardDispatcher
    {
        private readonly Func<Match, StringResource> _pageFunc;

        public StringResourceDispatcher(Func<Match, StringResource> pageFunc)
        {
            _pageFunc = pageFunc;
        }

        public Task Dispatch(DashboardContext context)
        {
            var resource = _pageFunc(context.UriMatch);
            context.Response.ContentType = resource.ContentType;
            return context.Response.WriteAsync(resource.Value);
        }
    }
}