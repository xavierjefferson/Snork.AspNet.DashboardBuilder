﻿
# System Information Dashboard
[![Latest version](https://img.shields.io/nuget/v/Snork.AspNet.Dashboard.SysInfo.svg)](https://www.nuget.org/packages/Snork.AspNet.Dashboard.SysInfo/) 


This is a plug-in dashboard for ASP.NET applications that outputs a large amount of information about the application's current state. This includes information about compilation options and extensions, the .NET version, server information and environment, the environment, OS version information, paths, master and local values of configuration options, and HTTP headers.

## Adding Dashboard
The [OWIN Startup class](https://docs.microsoft.com/en-us/aspnet/aspnet/overview/owin-and-katana/owin-startup-class-detection) is intended to keep web application bootstrap logic in a single place. In Visual Studio 2013 you can add it by right clicking on the project and choosing the Add / OWIN Startup Class menu item.

![Adding startup class](docs/add-owin-startup.png)

If you have Visual Studio 2012 or earlier, just create a regular class in the root folder of your application, name it `Startup` and place the following contents:
```
using Microsoft.Owin;
using MvcSample;
using Owin;
using Snork.AspNet.Dashboard.SysInfo;

[assembly: OwinStartup(typeof(Startup))]

namespace MyWebApplication
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
```
After performing these steps, open your browser and hit the *http://your-app/sysinfo* URL to see the Dashboard.
