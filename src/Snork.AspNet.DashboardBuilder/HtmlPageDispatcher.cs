using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Snork.AspNet.DashboardBuilder
{
    internal class HtmlPageDispatcher : IDashboardDispatcher
    {
        private readonly Func<Match, HtmlPage> _pageFunc;

        public HtmlPageDispatcher(Func<Match, HtmlPage> pageFunc)
        {
            _pageFunc = pageFunc;
        }

        public Task Dispatch(DashboardContext context)
        {
            context.Response.ContentType = "text/html";

            var page = _pageFunc(context.UriMatch);
            page.Assign(context);

            return context.Response.WriteAsync(page.ToString());
        }
    }
}