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
            var page = new P2(this) { AppRelativeVirtualPath = "~/sysinfo" };



            using (var s = new StringWriter())
            {
                HttpContext.Current.Server.Execute(page, s, false);
                WriteLiteral(s.ToString());
            }
        }
    }
}