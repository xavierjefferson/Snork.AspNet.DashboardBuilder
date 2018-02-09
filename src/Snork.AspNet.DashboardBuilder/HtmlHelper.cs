using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Snork.AspNet.DashboardBuilder
{
    public class HtmlHelper
    {
        private readonly RazorPage _page;

        public HtmlHelper([NotNull] RazorPage page)
        {
            if (page == null) throw new ArgumentNullException(nameof(page));
            _page = page;
        }


        public NonEscapedString RenderPartial(RazorPage partialPage)
        {
            partialPage.Assign(_page);
            return new NonEscapedString(partialPage.ToString());
        }

        public NonEscapedString Raw(string value)
        {
            return new NonEscapedString(value);
        }


        public string FormatProperties(IDictionary<string, string> properties)
        {
            return string.Join(", ", properties.Select(x => $"{x.Key}: \"{x.Value}\""));
        }


        public string HtmlEncode(string text)
        {
            return WebUtility.HtmlEncode(text);
        }
    }
}