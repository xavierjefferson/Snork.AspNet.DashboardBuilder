using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Snork.AspNet.DashboardBuilder
{
    internal class ByteResourceDispatcher : IDashboardDispatcher
    {
        private readonly Func<Match, ByteResource> _pageFunc;

        public ByteResourceDispatcher(Func<Match, ByteResource> pageFunc)
        {
            _pageFunc = pageFunc;
        }

        public Task Dispatch(DashboardContext context)
        {
            var resource = _pageFunc(context.UriMatch);
            context.Response.ContentType = resource.ContentType;
            return context.Response.Body.WriteAsync(resource.Value, 0, resource.Value.Length);
        }
    }
}