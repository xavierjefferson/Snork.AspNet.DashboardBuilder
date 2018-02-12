using System.IO;
using System.Web;
using Snork.AspNet.DashboardBuilder;

namespace Snork.AspNet.Dashboard.SysInfo
{
    public class HomePage : HtmlPage
    {
        public override void Execute()
        {
            var page = new WebformsHomePage(this) { AppRelativeVirtualPath = "~/sysinfo" };

            using (var stringWriter = new StringWriter())
            {
                HttpContext.Current.Server.Execute(page, stringWriter, false);
                WriteLiteral(stringWriter.ToString());
            }
        }
    }
}