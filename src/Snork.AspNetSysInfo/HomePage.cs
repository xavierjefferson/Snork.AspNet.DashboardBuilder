using System.IO;
using System.Web.UI;
using Snork.AspNet.DashboardBuilder;

namespace Snork.AspNetSysInfo
{
    public class HomePage : RazorPage
    {
        public override void Execute()
        {
            var page = new P2();
            using (var s = new StringWriter())
            {
                using (var hx = new HtmlTextWriter(s))
                {
                    page.RenderControl(hx);
                    hx.Flush();
                    WriteLiteral(s.ToString());
                }
            }
        }
    }
}