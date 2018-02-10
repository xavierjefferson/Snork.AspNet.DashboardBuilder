using System.IO;
using System.Runtime.Remoting.Contexts;
using System.Web;
using System.Web.Compilation;
using System.Web.UI;
using Snork.AspNet.DashboardBuilder;

namespace Snork.AspNetSysInfo
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