using Microsoft.Owin;
using MvcSample;
using Owin;
using Snork.AspNetSysInfo;

[assembly: OwinStartup(typeof(Startup))]

namespace MvcSample
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseSysInfoDashboard();
        }
    }
}