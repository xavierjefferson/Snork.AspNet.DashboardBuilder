using Microsoft.Owin;
using MvcSample;
using Owin;
using Snork.AspNet.Dashboard.SysInfo;

[assembly: OwinStartup(typeof(Startup))]

namespace MvcSample
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Map Dashboard to the `http://<your-app>/sysinfo` URL.
            app.UseSysInfoDashboard();
        }
    }
}